using Newtonsoft.Json;
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
        internal class ShowSearchApiModel
        {
            public int id { get; set; }
            public string name { get; set; }
        }

        public class EpisodeListApiModel
        {
            public int season { get; set; }
            public int number { get; set; }
            public string name{ get; set; }
        }

        HttpClient client = new HttpClient();
        string showSearchUrl = "https://api.tvmaze.com/singlesearch/shows?q={0}";
        string episodeSearchUrl = "https://api.tvmaze.com/shows/{0}/episodes";

        public TVMazeAPI()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public (string?, int?) GetSeriesTitle(string seriesTitle)
        {
            HttpResponseMessage response = client.GetAsync(String.Format(showSearchUrl, seriesTitle)).Result;
            if (response.IsSuccessStatusCode)
            {
                ShowSearchApiModel? seriesResponse = response.Content.ReadAsAsync<ShowSearchApiModel>().Result;
                if (seriesResponse != null) return (seriesResponse.name, seriesResponse.id);
            }
            else return ("error", null);

            return (null, null);
        }

        public EpisodeListApiModel[]? GetEpisodeList(int seriesId)
        {
            HttpResponseMessage response = client.GetAsync(String.Format(episodeSearchUrl, seriesId)).Result;
            if (response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().Result;
                if (String.IsNullOrEmpty(result)) return null;

                return JsonConvert.DeserializeObject<EpisodeListApiModel[]>(result);
            }

            return null;
        }

        public void Destroy()
        {
            client.Dispose();
        }
    }
}
