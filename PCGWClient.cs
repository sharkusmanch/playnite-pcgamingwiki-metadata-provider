using Playnite.SDK;
using RestSharp;
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace PCGamingWikiMetadata
{
    public class PCGWClient
    {
        private readonly ILogger logger = LogManager.GetLogger();
        private readonly string baseUrl = @"https://www.pcgamingwiki.com/w/api.php";
        private RestClient client;

        public PCGWClient()
        {
            client = new RestClient(baseUrl);
        }

        public JObject ExecuteRequest(RestRequest request)
        {
            request.AddParameter("format", "json", ParameterType.QueryString);
            request.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };
            var fullUrl = client.BuildUri(request);
            logger.Info(fullUrl.ToString());
            var response = client.Execute(request);
            
            if (response.ErrorException != null)
            {
                const string message = "Error retrieving response.  Check inner details for more info.";
                var e = new Exception(message, response.ErrorException);
                throw e;
            }
            var content = response.Content;

            logger.Debug(content);
            return JObject.Parse(content);
        }

        private string NormalizeSearchString(string search)
        {
            string updated = search.Replace("-", " ");

            return updated;
        }

        public List<GenericItemOption> SearchGames(string searchName)
        {
            List<GenericItemOption> gameResults = new List<GenericItemOption>();
            logger.Info(searchName);

            var request = new RestRequest("/", Method.GET);
            request.AddParameter("action", "query", ParameterType.QueryString);
            request.AddParameter("list", "search", ParameterType.QueryString);
            request.AddParameter("srsearch", NormalizeSearchString(searchName), ParameterType.QueryString);

            try 
            {
                JObject searchResults = ExecuteRequest(request);
                JToken error;

                if (searchResults.TryGetValue("error", out error))
                {
                    logger.Error($"Encountered API error: {error.ToString()}");
                    return gameResults;
                }

                logger.Debug($"SearchGames {searchResults["query"]["searchinfo"]["totalhits"]} results for {searchName}");

                foreach (dynamic game in searchResults["query"]["search"])
                {
                    if (!((string)game.snippet).Contains("#REDIRECT"))
                    {
                        PCGWGame g = new PCGWGame((string)game.title, (int)game.pageid);
                        gameResults.Add(g);
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error(e, "Error performing search");
            }

            return gameResults;
        }
    }
}