using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Portfolio.Models
{
    public class Github
    {
        public string Name { get; set; }
        public string Html_url { get; set; }
        public string Language { get; set; }
        public int Stargazers_count { get; set; }

        public static IEnumerable<Github> GetStarredRepos()
        {
            var client = new RestClient("https://api.github.com");
            var request = new RestRequest("search/repositories?q=user:jbryan22&stars>=1&per_page=3");
            //request.AddParameter("q", "user:jbryan22&stars>=1&per_page=3");
            request.AddHeader("User-Agent", "jbryan22");
            var response = new RestResponse();
            Task.Run(async () =>
            {
                response = await GetResponseContentAsync(client, request) as RestResponse;
            }).Wait();
            JObject jsonResponse = JsonConvert.DeserializeObject<JObject>(response.Content);
            var starredRepos = JsonConvert.DeserializeObject<List<Github>>(jsonResponse["items"].ToString())
                .OrderByDescending(x => x.Stargazers_count);

            return starredRepos;
        }

        public static Task<IRestResponse> GetResponseContentAsync(RestClient theClient, RestRequest theRequest)
        {
            var tcs = new TaskCompletionSource<IRestResponse>();
            theClient.ExecuteAsync(theRequest, response =>
            {
                tcs.SetResult(response);
            });
            return tcs.Task;
        }
    }
}
