using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace TheMagic
{
    public class TVMazeAPI
    {
        internal class ApiModel
        {
            public int id { get; set; }
            public string name { get; set; }
        }

        HttpClient client = new HttpClient();
        string url = "https://api.tvmaze.com/singlesearch/shows";
        string urlParams = "?q={0}";

        //async Task<HttpStatusCode> 

        public TVMazeAPI()
        {
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public string? GetSeriesTitle(string seriesTitle)
        {
            HttpResponseMessage response = client.GetAsync(String.Format(urlParams, seriesTitle)).Result;
            if (response.IsSuccessStatusCode)
            {
                ApiModel? seriesResponse = response.Content.ReadAsAsync<ApiModel>().Result;
                if (seriesResponse != null) return seriesResponse.name;
            }
            else return "error";

            return null;
        }

        public void Destroy()
        {
            client.Dispose();
        }
    }
}
