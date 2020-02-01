using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Conditions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Actions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Input;
using Microsoft.Bot.Builder.LanguageGeneration;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Templates;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Generators;
using Microsoft.Extensions.Configuration;
using Microsoft.Bot.Builder.AI.Luis;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtistChatBot
{
    public class RootDialog : ComponentDialog
    {

        protected readonly IConfiguration AppConfig;

        public RootDialog(IConfiguration Configuration)
            : base(nameof(RootDialog))
        {
            string[] paths = { ".", "Dialogs", "RootDialog.lg" };
            string fullPath = Path.Combine(paths);
            AppConfig = Configuration;
            string apiListUrl = AppConfig["ApiUrl"] + "/Musician/List/";
            string authKey = AppConfig["ApiAuthKey"];


            // Create instance of adaptive dialog. 
            var rootDialog = new AdaptiveDialog(nameof(AdaptiveDialog))
            {
                Generator = new TemplateEngineLanguageGenerator(LGParser.ParseFile(fullPath)),
                Recognizer = CreateRecognizer(AppConfig),

                Triggers = new List<OnCondition>()
                {
                    // Add a rule to welcome user
                    new OnConversationUpdateActivity()
                    {
                        Actions = WelcomeUserSteps(),
                    },
                    // Add intents
                    new OnIntent()
                    {
                        Intent = "ShowArtist",
                        Condition = "#ShowArtist.Score >= 0.7",
                        Actions = new List<Dialog> ()
                        {

                            new SetProperty() {
                                Property = "turn.Artist",
                                Value = "@ArtistName"
                            },
                            // RootDialog.DebugAction("Checking more on **@{turn.Artist}**"),
                            new BeginDialog("DiscoverArtistDialog"),
                        }
                    },
                    new OnIntent()
                    {
                        Intent = "AddArtist",
                        Condition = "#AddArtist.Score >= 0.7",
                        Actions = new List<Dialog> ()
                        {
                            new SetProperty() {
                                Property = "turn.Artist",
                                Value = "@ArtistName"
                            },
                            //RootDialog.DebugAction("Got Artist @{turn.Artist}"),
                            new BeginDialog("AddArtistDialog"),
                        }
                    },
                    new OnIntent()
                    {
                        Intent = "ListArtists",
                        Condition = "#ListArtists.Score >= 0.7",
                        Actions = new List<Dialog> ()
                        {
                            // new SendActivity("Querying musicians list..."),
                            new Microsoft.Bot.Builder.Dialogs.Adaptive.Actions.HttpRequest()
                            {
                                // Set response from the http request to turn.httpResponse property in memory.
                                ResultProperty = "dialog.httpResponse",
                                Url = apiListUrl,
                                Method = HttpRequest.HttpMethod.GET,
                                Headers = new Dictionary<string,Microsoft.Bot.Expressions.Properties.StringExpression> ()
                                {
                                    { "AuthKey", "@settings.ApiAuthKey"}
                                },
                                ResponseType = HttpRequest.ResponseTypes.Json
                            },
                            new SetProperty() {
                                Property = "turn.Top",
                                Value = "@Top"
                            },
                            new CodeAction(ProcessListResponse),
                            new SendActivity("@{dialog.ArtistResponse}"),
                        }
                    },
                    new OnIntent()
                    {
                        Intent = "StartHere",
                        Condition = "#StartHere.Score >= 0.7",
                        Actions = new List<Dialog> ()
                        {
                            new BeginDialog("AddArtistDialog"),
                        }
                    },
                    new OnIntent()
                    {
                        Intent = "EatStats",
                        Condition = "#EatStats.Score >= 0.7",
                        Actions = new List<Dialog> ()
                        {
                            new BeginDialog("DiscoverArtistDialog"),
                        }
                    },
                    new OnIntent()
                    {
                        Intent = "FeedInterview",
                        Condition = "#FeedInterview.Score >= 0.7",
                        Actions = new List<Dialog> ()
                        {
                            new BeginDialog("AddArtistDialog"),
                        }
                    },
                    new OnIntent()
                    {
                        Intent = "Shortcuts",
                        Condition = "#Shortcuts.Score >= 0.7",
                        Actions = new List<Dialog> ()
                        {
                            new SendActivity("@{Shortcuts()}"),
                        }
                    },
                    new OnIntent()
                    {
                        Intent = "Complain",
                        Condition = "#Complain.Score >= 0.7",
                        Actions = new List<Dialog> ()
                        {
                            new SendActivity("@{Placeholder()}"),
                        }
                    },
                    new OnIntent()
                    {
                        Intent = "Login",
                        Condition = "#Login.Score >= 0.7",
                        Actions = new List<Dialog> ()
                        {
                            new SendActivity("@{Placeholder()}"),
                        }
                    },
                    new OnIntent()
                    {
                        Intent = "Utilities_Cancel",
                        Condition = "#Utilities_Cancel.Score >= 0.7",
                        Actions = new List<Dialog>()
                        {
                            new SendActivity("@{CancelConfirmation()}"),
                            new CancelAllDialogs(),
                            new EndDialog()
                        }
                    },
                    new OnIntent()
                    {
                        Intent = "Utilities_Help",
                        Condition = "#Utilities_Help.Score >= 0.7",
                        Actions = new List<Dialog> ()
                        {
                            new SendActivity("@{BotOverviewHelp()}")
                        }
                    },
                    new OnIntent()
                    {
                        Intent = "Restart",
                        Condition = "#Restart.Score >= 0.7",
                        Actions = new List<Dialog> ()
                        {
                            new SendActivity("@{Restart()}"),
                            new SendActivity("@{WelcomeCard()}"),
                            new CancelAllDialogs(),
                            new EndDialog()
                        }
                    },
                    new OnIntent()
                    {
                        Intent = "Greetings",
                        Condition = "#Greetings.Score >= 0.7",
                        Actions = new List<Dialog> ()
                        {
                            new SendActivity("@{Greetings()}"),
                            new TextInput()
                            {
                                Prompt = new ActivityTemplate("@{AskForName()}"),
                                Property = "user.userProfile.Name"
                            },
                            new SendActivity("@{HiName()}"),
                            // new SendActivity("@{NextPrompt()}"),
                            new SendActivity("@{BotOverviewHelp()}")
                        }
                    },
                    // Add unhandled intent
                    new OnUnknownIntent()
                    {
                        Actions = OnUnknownIntent()
                    }
                }
            };

            // Add named dialogs to the DialogSet. These names are saved in the dialog state.
            AddDialog(rootDialog);

            // create and add all other dialogs
            // var addArtistDialog = new AddArtistDialog(Configuration);
            // BUGBUG: Why do we adddialog here and in the child?
            AddDialog(new AddArtistDialog(Configuration));
            AddDialog(new DiscoverArtistDialog(Configuration));

            // The initial child Dialog to run.
            InitialDialogId = nameof(AdaptiveDialog);
        }


        private async Task<DialogTurnResult> ProcessListResponse(DialogContext dc, System.Object options)
        {

            try
            {
                JObject JResponse = dc.GetState().GetValue<JObject>("dialog.httpResponse");

                JToken first = JResponse.First;
                if (!first.HasValues || first.Count() != 1)
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

                    string Top = dc.GetState().GetValue<string>("turn.top");
                    int limit = !String.IsNullOrEmpty(Top)? 10 : 100;
                    int counter = 0;

                    response.Append($"**Artists Listings:**\n");

                    // BUGBUG check for existence first
                    foreach (JToken jMusician in JResponse["content"]["musicians"])
                    {
                        if(++counter == limit)
                        {
                            break;
                        }
                        string name = (string)jMusician["name"];
                        string votes = (string)jMusician["votes"];

                        response.Append($"  - **{name}**. Likes = {votes}.\n");
                    }
                    if(counter == limit)
                    {
                        response.Append($"\nReached max limit {limit}.\n");
                    }
                    response.Append("\nType *Show Artist: <name>* for more details.\n");

                    dc.GetState().SetValue("dialog.ArtistResponse", response.ToString());
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Error: cannot parse server response. {e.Message}");

            }

            return await dc.EndDialogAsync();
        }

        public static SendActivity DebugAction(string text)
        {
            string message = $"DBG: {text}";
            return new SendActivity(message);
        }

        public static TraceActivity TraceAction(string text)
        {
            string message = $"DBG: {text}";

            // BUGBUG: how do we trace?

            return new TraceActivity()
            {
                // Name of the trace event.
                Name = message,
                ValueType = "text",

                // Property from memory to include in the trace
                Value = "turn.debug"
            };
        }


        public static Recognizer CreateRecognizer(IConfiguration configuration)
        {
            if (string.IsNullOrEmpty(configuration["LuisAppId"]) 
                || string.IsNullOrEmpty(configuration["LuisAPIKey"]) 
                || string.IsNullOrEmpty(configuration["LuisEndpointUrl"]))
            {
                throw new Exception("NOTE: LUIS is not configured.");
            }
            // Create the LUIS settings from configuration.
            var luisApplication = new LuisApplication(
                configuration["LuisAppId"],
                configuration["LuisAPIKey"],
                configuration["LuisEndpointUrl"]
            );

            LuisRecognizerOptionsV2 luisOpts = new LuisRecognizerOptionsV2(luisApplication);
            return new LuisRecognizer(luisOpts);
        }

        private static List<Dialog> WelcomeUserSteps()
        {
            return new List<Dialog>()
            {
                // Iterate through membersAdded list and greet user added to the conversation.
                new Foreach()
                {
                    ItemsProperty = "turn.activity.membersAdded",
                    Actions = new List<Dialog>()
                    {
                        // Note: Some channels send two conversation update events - one for the Bot added to the conversation and another for user.
                        // Filter cases where the bot itself is the recipient of the message. 
                        new IfCondition()
                        {
                            Condition = "$foreach.value.name != turn.activity.recipient.name",
                            Actions = new List<Dialog>()
                            {
                                new SendActivity("@{WelcomeCard()}"),
                                new SetProperty() {
                                    Property = "turn.Welcome",
                                    Value = "settings.EnableTopWelcome"
                                },
                                // RootDialog.DebugAction("Welcome? @{settings.EnableTopWelcome}"),
                            }
                        }
                    }
                }
            };
        }

        static private List<Dialog> OnUnknownIntent()
        {
            return new List<Dialog>()
            {
                new SendActivity("@{UnknownIntent()}"),
                TraceAction("Intent not found")
                // BUGBUG:  add help button
            };
        }

    }
}
