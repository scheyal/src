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
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;

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


            // Create instance of adaptive dialog. 
            var rootDialog = new AdaptiveDialog(nameof(AdaptiveDialog))
            {
                Generator = new TemplateEngineLanguageGenerator(new TemplateEngine().AddFile(fullPath)),
                Recognizer = CreateRecognizer(AppConfig),

                Triggers = new List<OnCondition>()
                {
                    // Add a rule to welcome user
                    new OnConversationUpdateActivity()
                    {
                        Actions = WelcomeUserSteps()
                    },
                    // Add intents
                    new OnIntent()
                    {
                        Intent = "StartHere",
                        Condition = "#StartHere.Score >= 0.7",
                        Actions = new List<Dialog> ()
                        {
                            new SendActivity("@{Placeholder()}"),
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
                            /** **
                            new IfCondition()
                            {
                                // BUGBUG: NOT WORKING. 
                                Condition = "and($user.userProfile.Name != null, length($user.userProfile.Name) > 2)",
                                Actions = new List<Dialog>()
                                {
                                    new SendActivity("@{HiName()}"),
                                },
                                ElseActions = new List<Dialog>()
                                {
                                    new TextInput()
                                    {
                                        Prompt = new ActivityTemplate("@{AskForName()}"),
                                        Property = "user.userProfile.Name"
                                    },
                                    new SendActivity("@{AckName()}"),
                                    DebugAction("BUGBUG: ElseActions for condition and($user.userProfile.Name != null, length($user.userProfile.Name) > 2)")

                                }
                            }, **/

                            new TextInput()
                            {
                                Prompt = new ActivityTemplate("@{AskForName()}"),
                                Property = "user.userProfile.Name"
                            },
                            new SendActivity("@{AckName()}"),
                            new SendActivity("@{NextPrompt()}"),
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


        public static IRecognizer CreateRecognizer(IConfiguration configuration)
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

            LuisRecognizerOptionsV3 luisOpts = new LuisRecognizerOptionsV3(luisApplication);
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
                                DebugAction("Todo: Display top stats."),
                                new SendActivity("@{WelcomeCard()}")
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

        private static List<Dialog> OnBeginDialogSteps()
        {
            return new List<Dialog>()
            {
                // Ask for user's age and set it in user.userProfile scope.
                new TextInput()
                {
                    Prompt = new ActivityTemplate("@{ModeOfTransportPrompt()}"),
                    // Set the output of the text input to this property in memory.
                    Property = "user.userProfile.Transport"
                },
                new TextInput()
                {
                    Prompt = new ActivityTemplate("@{AskForName()}"),
                    Property = "user.userProfile.Name"
                },
                // SendActivity supports full language generation resolution.
                // See here to learn more about language generation
                // https://github.com/Microsoft/BotBuilder-Samples/tree/master/experimental/language-generation
                new SendActivity("@{AckName()}"),
                new ConfirmInput()
                {
                    Prompt = new ActivityTemplate("@{AgeConfirmPrompt()}"),
                    Property = "turn.ageConfirmation"
                },
                new IfCondition()
                {
                    // All conditions are expressed using the common expression language.
                    // See https://github.com/Microsoft/BotBuilder-Samples/tree/master/experimental/common-expression-language to learn more
                    Condition = "turn.ageConfirmation == true",
                    Actions = new List<Dialog>()
                    {
                         new NumberInput()
                         {
                             Prompt = new ActivityTemplate("@{AskForAge()}"),
                             Property = "user.userProfile.Age",
                             // Add validations
                             Validations = new List<String>()
                             {
                                 // Age must be greater than or equal 1
                                 "int(this.value) >= 1",
                                 // Age must be less than 150
                                 "int(this.value) < 150"
                             },
                             InvalidPrompt = new ActivityTemplate("@{AskForAge.invalid()}"),
                             UnrecognizedPrompt = new ActivityTemplate("@{AskForAge.unRecognized()}")
                         },
                         new SendActivity("@{UserAgeReadBack()}")
                    },
                    ElseActions = new List<Dialog>()
                    {
                        new SendActivity("@{NoName()}") 
                    }
                },
                new ConfirmInput()
                {
                    Prompt = new ActivityTemplate("@{ConfirmPrompt()}"),
                    Property = "turn.finalConfirmation"
                },
                // Use LG template to come back with the final read out.
                // This LG template is a great example of what logic can be wrapped up in LG sub-system.
                new SendActivity("@{FinalUserProfileReadOut()}"), // examines turn.finalConfirmation
                new EndDialog()
            };
        }
    }
}
