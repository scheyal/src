using System;
using System.Text.Json;
using System.Threading.Tasks;


namespace CosmosDocStore
{
    class Program
    {
        // Initialized with sample access
        static string Key;
        static string Url;
        static string DatabaseId;
        static string CollectionId;
        static string DocumentId;
        static string OperationType;


        /// <summary>
        /// Initialize defaults
        /// </summary>
        static Program()
        {
            Key = "<cosmos DB key secret>"; 
            Url = "https://somedb.documents.azure.com:443"; 
            DatabaseId = "<database id>";
            CollectionId = "<collection id";
            DocumentId = "";
            OperationType = "GET";//  "GET"; // "POST"; // "DELETE";
        }

        /// <summary>
        /// Helper utility to beautify json string
        /// </summary>
        /// <param name="unPrettyJson">Json string to format</param>
        /// <returns></returns>
        public static string PrettyJson(string unPrettyJson)
        {
            var options = new JsonSerializerOptions()
            {
                WriteIndented = true
            };

            var jsonElement = JsonSerializer.Deserialize<JsonElement>(unPrettyJson);

            return JsonSerializer.Serialize(jsonElement, options);
        }

        /// <summary>
        /// Process command line
        /// </summary>
        /// <param name="args">command line args</param>
        /// <returns></returns>
        static bool ProcessCommandLine(string[] args)
        {

            try
            {
                if (args.Length > 0)
                {
                    int i = 0;
                    bool fVal = false;
                    foreach (string a in args)
                    {
                        ++i;
                        // Console.WriteLine($"> {a}");
                        switch (a)
                        {
                            case "-h":
                                Console.WriteLine("Usage:\nCosmosify -k key -u url -d db -c coll -i id -o opType (GET|POST|DELETE|REPLACE)");
                                return false;
                            case "-k":
                                Key = args[i];
                                fVal = true;
                                break;
                            case "-u":
                                Url = args[i];
                                fVal = true;
                                break;
                            case "-d":
                                DatabaseId = args[i];
                                fVal = true;
                                break;
                            case "-c":
                                CollectionId = args[i];
                                fVal = true;
                                break;
                            case "-i":
                                DocumentId = args[i];
                                fVal = true;
                                break;
                            case "-o":
                                OperationType = args[i];
                                fVal = true;
                                break;
                            default:
                                if (!fVal)
                                {
                                    Console.WriteLine($"Ignoring unknown option: {a}");
                                }
                                else
                                {
                                    fVal = !fVal;
                                }

                                break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: command line parsing error: " + e.Message);
                return false;
            }

            return true;
        }


        /// <summary>
        /// Program entry point
        /// </summary>
        /// <param name="args">Command line arguments</param>
        /// <returns></returns>
        static async Task Main(string[] args)
        {

            if (!ProcessCommandLine(args))
            {
                return;
            }


            try
            {
                string response;

                StoreAPI api = new StoreAPI(Key, Url, DatabaseId, CollectionId);

                // Execute Cosmos operation

                if (OperationType == "POST")
                {
                    if (String.IsNullOrEmpty(DocumentId))
                    {
                        Console.WriteLine("Error: id cannot be null for a post");
                        return;
                    }

                    string t = !String.IsNullOrEmpty(DocumentId) ? DocumentId : DateTime.UtcNow.ToString("s");
                    string jc = $"{{ \"id\": \"{t}\", \"content\": \"Some stuff\"}}";
                    jc = PrettyJson(jc);

                    Console.WriteLine(">> Creating Document:");
                    response = await api.CreateAsync(jc);
                }
                else if (OperationType == "REPLACE")
                {
                    if (String.IsNullOrEmpty(DocumentId))
                    {
                        Console.WriteLine("Error: id cannot be null for a post");
                        return;
                    }

                    string ts = DateTime.UtcNow.ToString("s");
                    string id = !String.IsNullOrEmpty(DocumentId) ? DocumentId : ts;
                    string jc = $"{{ \"id\": \"{id}\", \"content\": \"Some stuff: {ts}\"}}";
                    jc = PrettyJson(jc);

                    Console.WriteLine(">> Replacing Document:");
                    response = await api.ReplaceAsync(jc);
                }
                else if (OperationType == "DELETE")
                {
                    if (String.IsNullOrEmpty(DocumentId))
                    {
                        Console.WriteLine("Error: id cannot be null for a post");
                        return;
                    }

                    Console.WriteLine(">> Deleting Document: " + DocumentId);
                    response = await api.DeleteAsync(DocumentId);

                }
                else if (!String.IsNullOrEmpty(DocumentId))
                {
                    Console.WriteLine(">> Getting Document:");
                    response = api.Get(DocumentId);
                }
                else
                {
                    Console.WriteLine(">> Getting Collection:");
                    response = api.Get();
                }

                Console.WriteLine("---\nRequest:");
                Console.WriteLine($"  Token: {api.Token}");
                Console.WriteLine($"  Date: {api.RequestDate}");
                Console.WriteLine($"  URL: {api.FullUrl}");
                Console.WriteLine("---");

                response = PrettyJson(response);

                Console.WriteLine($"Response =\n{response}");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: cannot execute Cosmos action: " + e.Message);

            }


        }
    }
}
