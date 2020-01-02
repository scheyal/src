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

        private static List<Dialog> AddArtistInterview()
        {
            return new List<Dialog>()
            {
                RootDialog.DebugAction("Start Add Artist Interview Dialog"),
                new SendActivity("@{WelcomeCard()}")
            };
        }


    }
}

