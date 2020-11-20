using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CosmosDocStore
{
    /// <summary>
    /// Cosmos DB Docs manager. Perfoms CRUD + LIST operations on existing store.
    /// </summary>
    class StoreAPI
    { 

        public const string ContentType = "application/json";

        string MasterKey;

        public string RequestDate { get; set; }
        public string BaseUrl { get; set; }
        public string FullUrl { get; set; }
        public string DatabaseId { get; set; }
        public string CollectionId { get; set; }
        public string Token { get; set; }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="masterKey">Cosmos DB master access key</param>
        /// <param name="url">Cosmos DB base URL</param>
        /// <param name="databaseId">Database Id</param>
        /// <param name="collectionId">Collection Id</param>
        public StoreAPI(string masterKey, string url, string databaseId, string collectionId)
        {
            MasterKey = masterKey;
            BaseUrl = url;
            DatabaseId = databaseId;
            CollectionId = collectionId;
        }


        /// <summary>
        /// Generates a token to be presented as auth key for Cosmos DB REST API operations
        /// <seealso cref="https://docs.microsoft.com/en-us/rest/api/cosmos-db/access-control-on-cosmosdb-resources"/>
        /// <seealso cref="https://docs.microsoft.com/en-us/rest/api/cosmos-db/"/>
        /// </summary>
        /// <param name="verb">URI verb: GET, POST, DELETE, PUT</param>
        /// <param name="resourceId">Path to resource e.g. 	"dbs/ToDoList"</param>
        /// <param name="resourceType">Type of resource e.g. "dbs"</param>
        /// <param name="key">Master key permitting the operations</param>
        /// <param name="date">UTC Timestamp</param>
        /// <param name="keyType">Type of key: resource or master (default</param>
        /// <returns></returns>
        public static string GenerateToken(string verb, string resourceId, string resourceType, string key, string date, string keyType = null)
        {
            const string tokenVersion = "1.0";
            keyType = keyType ?? "master";

            string token = string.Empty;

            try
            {

                if (String.IsNullOrEmpty(key) ||
                    String.IsNullOrEmpty(verb) ||
                    String.IsNullOrEmpty(resourceType) ||
                    String.IsNullOrEmpty(date) ||
                    string.IsNullOrEmpty(resourceId))
                {
                    throw new Exception("Missing key/verb/resourceId/date param");
                }
                if (verb != "GET" && verb != "POST" && verb != "DELETE" && verb != "PUT")
                {
                    throw new Exception("Invalid verb param");
                }

                var hmacSha256 = new System.Security.Cryptography.HMACSHA256 { Key = Convert.FromBase64String(key) };


                verb = verb.ToLowerInvariant();
                resourceType = resourceType.ToLowerInvariant();
                date = date.ToLowerInvariant();

                string payLoad = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}\n{1}\n{2}\n{3}\n{4}\n",
                        verb,
                        resourceType,
                        resourceId,
                        date,
                        ""
                );

                byte[] hashPayLoad = hmacSha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(payLoad));
                string signature = Convert.ToBase64String(hashPayLoad);

                token = System.Web.HttpUtility.UrlEncode(String.Format(System.Globalization.CultureInfo.InvariantCulture, "type={0}&ver={1}&sig={2}",
                    keyType,
                    tokenVersion,
                    signature));
            }
            catch (Exception e)
            {
                token = $"Token Error: {e.Message} ({e.HResult}).";
            }

            return token;
        }

        /// <summary>
        /// Common cosmos db http client utility helper
        /// </summary>
        /// <param name="verb"></param>
        /// <param name="databaseId"></param>
        /// <param name="collectionId"></param>
        /// <param name="resourceId"></param>
        /// <param name="resourceType"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        System.Net.Http.HttpClient GetClient(string verb, string resourceId, string resourceType = "docs")
        {
            string utcDate = DateTime.UtcNow.ToString("R");
            RequestDate = utcDate;

            Token = GenerateToken(verb, resourceId, resourceType, MasterKey, utcDate);

            var client = new System.Net.Http.HttpClient();

            client.DefaultRequestHeaders.Add("x-ms-date", utcDate);
            client.DefaultRequestHeaders.Add("x-ms-version", "2017-02-22");
            client.DefaultRequestHeaders.Remove("authorization");
            client.DefaultRequestHeaders.Add("authorization", Token);

            return client;
        }


        /// <summary>
        /// Get collection or single item
        /// </summary>
        /// <param name="databaseId">db to query</param>
        /// <param name="collectionId">collection within the db</param>
        /// <param name="documentId">optional: if single item is desired</param>
        /// <returns></returns>
        public string Get(string documentId = null)
        {
            string result;

            try
            {
                const string verb = "GET";
                string resourceLink = 
                    (documentId == null) ? string.Format("dbs/{0}/colls/{1}/docs", DatabaseId, CollectionId) :   string.Format("dbs/{0}/colls/{1}/docs/{2}", DatabaseId, CollectionId, documentId);
                string resourceId = (documentId == null) ? string.Format("dbs/{0}/colls/{1}", DatabaseId, CollectionId) : resourceLink;

                Uri uri = new Uri(new Uri(BaseUrl), resourceLink);
                FullUrl = uri.ToString();
                var client = GetClient(verb, resourceId);
                result = client.GetStringAsync(uri).Result;

            }
            catch (Exception e)
            {
                result = $"{{\"Error\": \"{e.Message}\"}}";
            }

            return result;
        }


        /// <summary>
        /// Creates a document in Cosmos DB
        /// </summary>
        /// <param name="content">Valid Json with mandatory unique "Id" element and additional content</param>
        /// <returns></returns>
        public async Task<string> CreateAsync(string content)
        {
            string result;

            try
            {
                const string verb = "POST";
                string resourceId = string.Format("dbs/{0}/colls/{1}", DatabaseId, CollectionId);
                string resourceLink = string.Format("dbs/{0}/colls/{1}/docs", DatabaseId, CollectionId);

                Uri uri = new Uri(new Uri(BaseUrl), resourceLink);
                FullUrl = uri.ToString();
                var client = GetClient(verb, resourceId);

                var jsonElement = JsonSerializer.Deserialize<JsonElement>(content);
                bool fId = jsonElement.TryGetProperty("id", out JsonElement value);
                if(!fId || value.ToString() == "")
                {
                    throw new Exception("Invalid Json Content missing id.");
                }

                var stringContent = new StringContent(content, Encoding.UTF8, ContentType);
                

                var response = await client.PostAsync(uri, stringContent);

                HttpStatusCode statusCode = response.StatusCode;
                if(statusCode != HttpStatusCode.Created)
                {
                    throw new Exception($"Post Error: {statusCode}");
                }

                result = await response.Content.ReadAsStringAsync();

            }
            catch (Exception e)
            {
                result = $"{{\"Error\": \"{e.Message}\"}}";
            }

            return result;
        }

        /// <summary>
        /// Deletes document with specified Id
        /// </summary>
        /// <param name="documentId">Id of document to delete</param>
        /// <returns></returns>
        public async Task<string> DeleteAsync(string documentId)
        {
            string result;

            try
            {
                const string verb = "DELETE";
                string resourceLink = string.Format("dbs/{0}/colls/{1}/docs/{2}", DatabaseId, CollectionId, documentId);
                string resourceId = resourceLink; // string.Format("dbs/{0}/colls/{1}", DatabaseId, CollectionId);


                Uri uri = new Uri(new Uri(BaseUrl), resourceLink);
                FullUrl = uri.ToString();
                var client = GetClient(verb, resourceId);


                var response = await client.DeleteAsync(uri);

                HttpStatusCode statusCode = response.StatusCode;
                if (statusCode != HttpStatusCode.NoContent)
                {
                    throw new Exception($"Post Error: {statusCode}");
                }

                result = await response.Content.ReadAsStringAsync();
                if (String.IsNullOrEmpty(result))
                {
                    result = "{ \"Status\": \"204\" }";
                }

            }
            catch (Exception e)
            {
                result = $"{{\"Error\": \"{e.Message}\"}}";
            }

            return result;
        }

        /// <summary>
        /// Replace an existing doc
        /// </summary>
        /// <param name="content">Doc content including 'id' of document to be replaced</param>
        /// <returns></returns>
        public async Task<string> ReplaceAsync(string content)
        {
            string result;

            try
            {

                // verify new doc and extract id
                JsonElement value;
                var jsonElement = JsonSerializer.Deserialize<JsonElement>(content);
                bool fId = jsonElement.TryGetProperty("id", out value);
                if (!fId || value.ToString() == "")
                {
                    throw new Exception("Invalid Json Content missing id.");

                }
                string documentId = value.ToString();

                // retrieve document
                string currDoc = Get(documentId);

                if(String.IsNullOrEmpty(currDoc))
                {
                    throw new Exception("Cannot replace missing doc.");
                }

                /**
                // verify it is the same
                jsonElement = JsonSerializer.Deserialize<JsonElement>(currDoc);
                fId = jsonElement.TryGetProperty("id", out value);
                if (!fId || value.ToString() != documentId)
                {
                    throw new Exception("Replace doc: id verification failed.");
                }

                // verify it is the same
                jsonElement = JsonSerializer.Deserialize<JsonElement>(currDoc);
                fId = jsonElement.TryGetProperty("_self", out value);
                if (!fId || value.ToString() == "")
                {
                    throw new Exception("Replace doc: cannot find _self.");
                }
                **/

                const string verb = "PUT";
                string resourceLink = string.Format("dbs/{0}/colls/{1}/docs/{2}", DatabaseId, CollectionId, documentId);
                string resourceId = resourceLink; // string.Format("dbs/{0}/colls/{1}", DatabaseId, CollectionId);

                Uri uri = new Uri(new Uri(BaseUrl), resourceLink);
                FullUrl = uri.ToString();
                var client = GetClient(verb, resourceId);

                var stringContent = new StringContent(content, Encoding.UTF8, ContentType);
                var response = await client.PutAsync(uri, stringContent);

                HttpStatusCode statusCode = response.StatusCode;
                if (statusCode != HttpStatusCode.OK)
                {
                    throw new Exception($"Put Error: {statusCode}");
                }

                result = await response.Content.ReadAsStringAsync();

            }
            catch (Exception e)
            {
                result = $"{{\"Error\": \"{e.Message}\"}}";
            }

            return result;
        }


    }
}
