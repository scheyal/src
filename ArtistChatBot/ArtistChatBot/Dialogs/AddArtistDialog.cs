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
                            // RootDialog.DebugAction("AddArtistDialog: turn.Artist = @{turn.Artist}"),
                            new TextInput()
                            {
                                Property = "turn.Artist",
                                Prompt = new ActivityTemplate("@{GetArtist()}")
                            },
                            new SetProperty() {
                                Property = "dialog.Artist",
                                Value = "turn.Artist"
                            },
                            new TextInput()
                            {
                                Property = "turn.Album",
                                Prompt = new ActivityTemplate("@{GetAlbum()}")
                            },
                            new SetProperty() {
                                Property = "dialog.Album",
                                Value = "turn.Album"
                            },
                            // RootDialog.DebugAction("Future: use Add Album: [name]"),

                            new TextInput()
                            {
                                Property = "turn.Song",
                                Prompt = new ActivityTemplate("@{GetSong()}")
                            },
                            new SetProperty() {
                                Property = "dialog.Song",
                                Value = "turn.Song"
                            },

                            // BUGBUG RootDialog.DebugAction("Coming soon: In the future you will be able to add a brief review. Waiting for a bug fix."),

                            /* BUGBUG: Waiting for bug fix
                            new TextInput()
                            {
                                Property = "turn.UserReview",
                                Prompt = new ActivityTemplate("@{GetReview()}")
                            },
                            new SetProperty() {
                                Property = "dialog.UserReview",
                                Value = "turn.UserReview"
                            },
                            */
                            // RootDialog.DebugAction("Future: use Add Review: [name]"),
                            new TextInput()
                            {
                                Prompt = new ActivityTemplate("@{AskForName()}"),
                                Property = "user.userProfile.Name"
                            },

                            new SendActivity("@{SubmissionRecord()}"),
                            new TextInput()
                            {
                                Property = "turn.ConfirmSubmitArtist",
                                Prompt = new ActivityTemplate("@{ConfirmPromptSubmitArtist()}")
                            },
                            new IfCondition()
                            {
                                // All conditions are expressed using the common expression language.
                                // See https://github.com/Microsoft/BotBuilder-Samples/tree/master/experimental/common-expression-language to learn more
                                Condition = "toLower(turn.ConfirmSubmitArtist) == 'yes'",
                                Actions = new List<Dialog>()
                                {
                                    // new SendActivity("Submitting artist **@{dialog.Artist}** ..."),
                                    // BUGBUG.Waiting for bug fix. using inline code in the meantime. 
                                    // new CodeAction(GenerateArtistBody),
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

                                        Body = ArtistSubmissionBodyInline()
                                        // ResponseType = HttpRequest.ResponseTypes.None
                                    },
                                    // RootDialog.DebugAction("BP @HttpAction"),
                                    // new SendActivity("Submitted. (Result = @{dialog.httpResponse}.)"),
                                    new CodeAction(ProcessArtistResponse),
                                    new IfCondition()
                                    {
                                        Condition = "dialog.ArtistResponse == 'OK200'",
                                        Actions = new List<Dialog>()
                                        {
                                            new SendActivity("@{SubmissionAccepted()}"),
                                        },
                                        ElseActions = new List<Dialog>()
                                        {
                                            new SendActivity("@{dialog.ArtistResponse}"),
                                        }
                                    },
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
            string artist = dc.GetState().GetValue<string>("dialog.Artist");
            string album = dc.GetState().GetValue<string>("dialog.Album");
            string song  = dc.GetState().GetValue<string>("dialog.Song");
            string review = dc.GetState().GetValue<string>("dialog.UserReview");
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


            return await dc.EndDialogAsync();
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
                                    new JProperty("Users",
                                        new JArray(
                                            new JValue(submmiter)
                                        )
                                    )
                                )
                            )
                        ),
                        new JProperty("FavoriteSongs",
                            new JArray(
                                new JObject(
                                    new JProperty("Name", song),
                                    new JProperty("Votes", "1"),
                                    new JProperty("Users",
                                        new JArray(
                                            new JValue(submmiter)
                                        )
                                    )
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

        private static JObject ArtistSubmissionBodyInline()
        {
 
            JObject JB = new JObject(
                new JProperty("Name", "@{dialog.Artist}"),
                    new JProperty("Properties",
                        new JObject(
                            new JProperty("FavoriteAlbums",
                                new JArray(
                                    new JObject(
                                        new JProperty("Name", "@{dialog.Album}"),
                                        new JProperty("Votes", "1"),
                                        new JProperty("Users", null)
                                    )
                                )
                            ),
                            new JProperty("FavoriteSongs",
                                new JArray(
                                    new JObject(
                                        new JProperty("Name", "@{dialog.Song}"),
                                        new JProperty("Votes", "1"),
                                        new JProperty("Users",null)
                                    )
                                )
                            ),
                        new JProperty("Reviews",
                            new JArray(
                                new JValue("*n/a") // ("@{dialog.UserReview}") 
                            )
                        )
                    )
                ),
                new JProperty("Votes", "1"),
                new JProperty("Submitter", "@{coalesce(user.userProfile.Name, 'Anonymous')}")
            );

//            string jsonStr = JB.ToString(Newtonsoft.Json.Formatting.Indented);
//            System.Diagnostics.Trace.TraceInformation(jsonStr);
            return JB;
        }

        private static JObject XXArtistSubmissionBodyInline()
        {
            JObject JB = new JObject(
                new JProperty("Name", "@{dialog.Artist}"),
                    new JProperty("Properties",
                        new JObject(
                            new JProperty("FavoriteAlbums",
                                new JArray(
                                    new JObject(
                                        new JProperty("Name", "@{dialog.Album}"),
                                        new JProperty("Votes", "1"),
                                        new JProperty("Users",
                                            new JArray(
                                                new JValue("@{coalesce(user.userProfile.Name, 'Anonymous')}")
                                            )
                                        )
                                    )
                                )
                            ),
                            new JProperty("FavoriteSongs",
                                new JArray(
                                    new JObject(
                                        new JProperty("Name", "@{dialog.Song}"),
                                        new JProperty("Votes", "1"),
                                        new JProperty("Users",
                                            new JArray(
                                                new JValue("@{coalesce(user.userProfile.Name, 'Anonymous')}")
                                            )
                                        )
                                    )
                                )
                            ),
                        new JProperty("Reviews",
                            new JValue("@{dialog.UserReview}")
                        )
                    )
                ),
                new JProperty("Votes", "1")
            );
            return JB;
        }


        private async Task<DialogTurnResult> ProcessArtistResponse(DialogContext dc, System.Object options)
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
                    string errMsg = $"Backend Server Error. Http Code = {httpCode}";
                    System.Diagnostics.Trace.TraceInformation(errMsg);
                    dc.GetState().SetValue("dialog.ArtistResponse", errMsg);
                }
                else
                {
                    string msg = $"Submission Accepted. (Server Code = {httpCode})";
                    System.Diagnostics.Trace.TraceInformation(msg);
                    dc.GetState().SetValue("dialog.ArtistResponse", "OK200");
                }

            }
            catch (Exception e)
            {
                throw new Exception($"Error: cannot parse server response. {e.Message}");

            }

            return await dc.EndDialogAsync();
        }
    }
}

