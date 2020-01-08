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
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ArtistChatBot
{
    public class AddArtistDialog : ComponentDialog
    {

        protected readonly IConfiguration AppConfig;

        public AddArtistDialog(IConfiguration Configuration)
            : base(nameof(AddArtistDialog))
        {
            string[] paths = { ".", "Dialogs", "AddArtistDialog.lg" };
            string fullPath = Path.Combine(paths);
            AppConfig = Configuration;
            string apiUrl = AppConfig["ApiUrl"] + "/Musician/CreateOrUpdate/";
            string authKey = AppConfig["ApiAuthKey"];


            // Create instance of adaptive dialog. 
            var AddArtistDialog = new AdaptiveDialog(nameof(AdaptiveDialog))
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
                            new SendActivity("@{WelcomeAddArtist()}"),
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
                                Property = "conversation.Artist",
                                Value = "turn.Artist"
                            },
                            RootDialog.DebugAction("Future: use Add Artist: [name]"),

                            new TextInput()
                            {
                                Property = "turn.Album",
                                Prompt = new ActivityTemplate("@{GetAlbum()}")
                            },
                            new SetProperty() {
                                Property = "conversation.Album",
                                Value = "turn.Album"
                            },
                            RootDialog.DebugAction("Future: use Add Album: [name]"),

                            new TextInput()
                            {
                                Property = "turn.Song",
                                Prompt = new ActivityTemplate("@{GetSong()}")
                            },
                            new SetProperty() {
                                Property = "conversation.Song",
                                Value = "turn.Song"
                            },
                            RootDialog.DebugAction("Future: use Add Song: [name]"),

                            new TextInput()
                            {
                                Property = "turn.Review",
                                Prompt = new ActivityTemplate("@{GetReview()}")
                            },
                            new SetProperty() {
                                Property = "conversation.Review",
                                Value = "turn.Review"
                            },
                            RootDialog.DebugAction("Future: use Add Review: [name]"),

                            new SendActivity("@{SubmissionRecord()}"),
                            new ConfirmInput()
                            {
                                Property = "turn.ConfirmSubmitArtist",
                                Prompt = new ActivityTemplate("@{ConfirmPromptSubmitArtist()}")
                            },
                            new IfCondition()
                            {
                                // All conditions are expressed using the common expression language.
                                // See https://github.com/Microsoft/BotBuilder-Samples/tree/master/experimental/common-expression-language to learn more
                                Condition = "turn.ConfirmSubmitArtist == true",
                                Actions = new List<Dialog>()
                                {
                                    new SendActivity("Submitting artist **@{conversation.Artist}** ..."),
                                    new CodeAction(GenerateArtistBody),
                                    new TraceActivity()
                                    {
                                        Name = "dialog.httprequest.body",
                                        ValueType = "Object",
                                        Value = "dialog"
                                    },
                                    new Microsoft.Bot.Builder.Dialogs.Adaptive.Actions.HttpRequest()
                                    {
                                        // Set response from the http request to turn.httpResponse property in memory.
                                        ResultProperty = "dialog.httpResponse",
                                        Url = apiUrl,
                                        Method = HttpRequest.HttpMethod.POST,
                                        Headers = new Dictionary<string,string> ()
                                        {
                                            { "AuthKey", authKey }
                                        },

                                        Body = "@{dialog.httpbody}",
                                        ResponseType = HttpRequest.ResponseTypes.None
                                    },
                                    RootDialog.DebugAction("BP @HttpAction"),
                                    new SendActivity("Submitted. (Result = @{dialog.httpResponse}.)"),
                                    new EndDialog()
                                },
                                ElseActions = new List<Dialog>()
                                {
                                    new SendActivity("Submit aborted (you can still submit by typing 'submit artist'. Additional commands delete, reset, edit artist (Future or Never)")
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

            AddDialog(AddArtistDialog);
            InitialDialogId = nameof(AdaptiveDialog);
        }

        
        private async Task<DialogTurnResult> GenerateArtistBody(DialogContext dc, System.Object options)

        {
            string artist = dc.GetState().GetValue<string>("conversation.Artist");
            string album = dc.GetState().GetValue<string>("conversation.Album");
            string song  = dc.GetState().GetValue<string>("conversation.Song");
            string review = dc.GetState().GetValue<string>("conversation.Review");
            string submitter = dc.GetState().GetValue<string>("user.userProfile.Name");

            submitter = String.IsNullOrEmpty(submitter) ? "Anonymous" : submitter;

            JObject JB = ArtistSubmissionBody(artist, album, song, review, submitter);

            // dc.GetState().SetValue("dialog.httpbody", JsonBody);

            // Fake test
            //JObject JB = new JObject(
            //        new JProperty("Body",
            //          new JObject(
            //            new JProperty("Name", artist),
            //            new JProperty("Properties", "Debug Test")
            //          )
            //        )
            //    );

            dc.GetState().SetValue("dialog.httpbody", JB);

            return new DialogTurnResult(DialogTurnStatus.Complete, options);
        }



        /// <summary>
        /// Produces Json for artist submission body.
        /// Example:
        /// {
        //  "Name": "john bonham",
        //  "Properties": {
        //    "FavoriteAlbums": [
        //      {
        //        "Name": "led zeppelin 1",
        //        "Votes": "1",
        //        "Users": "eyal"
        //      }
        //    ],
        //    "FavoriteSongs": [
        //      {
        //        "Name": "rocknroll",
        //        "Votes": "1",
        //        "Users": "eyal"
        //      }
        //    ],
        //    "Reviews": "that's the way I like it"
        //  },
        //  "Votes": "1"
        //}
        /// </summary>
        /// <param name="artist"></param>
        /// <param name="album"></param>
        /// <param name="song"></param>
        /// <param name="review"></param>
        /// <param name="submmiter"></param>
        /// <returns></returns>
        private static JObject ArtistSubmissionBody(string artist, string album, string song, string review, string submmiter)
        {
            string body = string.Empty;

            JObject JB = new JObject(
                new JProperty("Name", artist),
                new JProperty("Properties",
                    new JObject(
                       new JProperty("FavoriteAlbums",
                            new JArray(
                                new JObject(
                                    new JProperty("Name", album),
                                    new JProperty("Votes", "1"),
                                    new JProperty("Users", submmiter)
                                )
                            )
                       ),
                        new JProperty("FavoriteSongs",
                            new JArray(
                                new JObject(
                                    new JProperty("Name", song),
                                    new JProperty("Votes", "1"),
                                    new JProperty("Users", submmiter)
                               )
                            )
                       ),
                        new JProperty("Reviews",
                            new JValue(review)
                       )
                   )
                ),
                new JProperty("Votes", "1")
            );

            // Fake test
            //JB = new JObject(
            //        new JProperty("Body", 
            //          new JObject(
            //            new JProperty("Name", artist),
            //            new JProperty("Properties", "Debug Test")
            //          )
            //        )
            //    );

            return JB;
        }
    }
}

