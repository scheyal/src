using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Actions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Conditions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Generators;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Input;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Templates;
using Microsoft.Bot.Builder.LanguageGeneration;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtistChatBot
{
    public class DiscoverArtistDialog : ComponentDialog
    {

        protected readonly IConfiguration AppConfig;

        public DiscoverArtistDialog(IConfiguration Configuration)
            : base(nameof(DiscoverArtistDialog))
        {
            string[] paths = { ".", "Dialogs", "DiscoverArtistDialog.lg" };
            string fullPath = Path.Combine(paths);
            AppConfig = Configuration;
            string apiUrl = AppConfig["ApiUrl"] + "/Musician/Find/";
            string authKey = AppConfig["ApiAuthKey"];


            // Create instance of adaptive dialog. 
            var DiscoverArtistDialog = new AdaptiveDialog(nameof(AdaptiveDialog))
            {
                Generator = new TemplateEngineLanguageGenerator(new TemplateEngine().AddFile(fullPath)),
                Recognizer = RootDialog.CreateRecognizer(AppConfig),

                Triggers = new List<OnCondition>()
                {
                    // Dialog start
                    new OnBeginDialog() // consider parameters
                    {
                        Actions = new List<Dialog> ()
                        {
                            new SendActivity("@{WelcomeDiscoverArtist()}"),
                            new TextInput()
                            {
                                Prompt = new ActivityTemplate("@{AskForName()}"),
                                Property = "user.userProfile.Name"
                            },
                            new TextInput()
                            {
                                Property = "turn.Artist",
                                Prompt = new ActivityTemplate("@{GetArtist()}")
                            },
                            new SetProperty() {
                                Property = "dialog.Artist",
                                Value = "turn.Artist"
                            },
                            RootDialog.DebugAction("Future: use show Artist: [name]"),

                            new IfCondition()
                            {
                                Condition = "turn.Artist != null",
                                Actions = new List<Dialog>()
                                {
                                    // new SendActivity("Querying artist **@{dialog.Artist}** ..."),
                                    new Microsoft.Bot.Builder.Dialogs.Adaptive.Actions.HttpRequest()
                                    {
                                        // Set response from the http request to turn.httpResponse property in memory.
                                        ResultProperty = "dialog.httpResponse",
                                        Url = apiUrl + "@{dialog.Artist}",
                                        Method = HttpRequest.HttpMethod.GET,
                                        Headers = new Dictionary<string,string> ()
                                        {
                                            { "AuthKey", authKey }
                                        },
                                        ResponseType = HttpRequest.ResponseTypes.Json
                                    },
                                    new CodeAction(ProcessArtistResponse),
                                    new SendActivity("@{dialog.ArtistResponse}"),
                                    new EndDialog()
                                },
                                ElseActions = new List<Dialog>()
                                {
                                    new SendActivity("Future: use Show Artist: <name> to discover if the artist is known here. Or use Add Artist to add your favorite artist.")
                                }
                                // We do not need to specify an else block here since if user said no,
                                // the control flow will automatically return to the last active step (if any)
                            }
                        },
                    },
                    // Add intents
                    new OnUnknownIntent()
                    {
                        Actions = new List<Dialog> ()
                        {
                            RootDialog.DebugAction("Add Artist Unkown Intent."),
                            new SendActivity("@{Placeholder()}"),
                        }
                    }
                }
            };

            AddDialog(DiscoverArtistDialog);
            InitialDialogId = nameof(AdaptiveDialog);
        }

        private async Task<DialogTurnResult> ProcessArtistResponse(DialogContext dc, System.Object options)
        {

            try
            {
                JObject JResponse = dc.GetState().GetValue<JObject>("dialog.httpResponse");

                JToken first = JResponse.First;
                if(!first.HasValues || first.Count() != 1)
                {
                    throw new Exception("Invalid BotArtBe response.");
                }

                string httpCode = (string)JResponse["statusCode"];
                if (httpCode != "200")
                {
                    System.Diagnostics.Trace.TraceInformation($"BotArtBe error: httpCode = {httpCode}");
                    dc.GetState().SetValue("dialog.ArtistResponse", "Artist not found");
                }
                else
                {

                    StringBuilder response = new StringBuilder();

                    string artist = (string)JResponse["content"]["name"];
                    string artistVotes = (string)JResponse["content"]["votes"];
                    response.Append($"Artist: **{artist}** (Votes = {artistVotes})\n");

                    // BUGBUG check for existence first
                    response.Append($"- Albums\n");
                    foreach (JToken jAlbum in JResponse["content"]["properties"]["favoriteAlbums"])
                    {

                        string album = (string)jAlbum["name"];
                        string votes = (string)jAlbum["votes"];

                        response.Append($"  - **{album}**. Votes = {votes}.");

                        string users = String.Empty;
                        if(jAlbum["users"].HasValues)
                        {
                            foreach(JToken user in jAlbum["users"])
                            {
                                string u = (string)user;
                                users += $"{u};";
                            }
                            response.Append($" Liked by { users}\n");                         
                        }
                        else
                        {
                            response.Append("\n");
                        }
                    }
                    response.Append($"- Songs\n");
                    foreach (JToken jSong in JResponse["content"]["properties"]["favoriteSongs"])
                    {
                        string song = (string)jSong["name"];
                        string votes = (string)jSong["votes"];

                        response.Append($"  - **{song}**. Votes = {votes}.");

                        string users = String.Empty;
                        if (jSong["users"].HasValues)
                        {
                            foreach (JToken user in jSong["users"])
                            {
                                string u = (string)user;
                                users += $"{u};";
                            }
                            response.Append($" Liked by { users}\n");
                        }
                        else
                        {
                            response.Append("\n");
                        }
                    }
                    response.Append($"- Reviews\n");
                    foreach (JToken jRev in JResponse["content"]["properties"]["reviews"])
                    {
                        string review = (string)jRev;
                        response.Append($"  - {review}\n");
                    }

                    dc.GetState().SetValue("dialog.ArtistResponse", response.ToString());
                }
            }
            catch(Exception e)
            {
                throw new Exception($"Error: cannot parse server response. {e.Message}");

            }

            return new DialogTurnResult(DialogTurnStatus.Complete, options);
        }


    }
}
