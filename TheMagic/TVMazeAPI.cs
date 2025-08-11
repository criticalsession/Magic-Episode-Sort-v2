using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace TheMagic {
    public class TVMazeAPI
    {
        internal class ShowSearchApiModel
        {
            public int Id { get; set; }
            public required string Name { get; set; }
        }

        public class EpisodeListApiModel
        {
            public int Season { get; set; }
            public int Number { get; set; }
            public required string Name { get; set; }
        }

        HttpClient client = new();
        const string showSearchUrl = "https://api.tvmaze.com/singlesearch/shows?q={0}";
        const string episodeSearchUrl = "https://api.tvmaze.com/shows/{0}/episodes";

        public TVMazeAPI()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public (string?, int?) GetSeriesTitle(string seriesTitle)
        {
            var response = client.GetAsync(string.Format(showSearchUrl, seriesTitle)).Result;
            if (!response.IsSuccessStatusCode) return ("error", null);
            else {
                var seriesResponse = response.Content.ReadAsAsync<ShowSearchApiModel>().Result;
                if (seriesResponse != null) return (seriesResponse.Name, seriesResponse.Id);
            }

            return (null, null);
        }

        public EpisodeListApiModel[]? GetEpisodeList(int seriesId)
        {
            var response = client.GetAsync(string.Format(episodeSearchUrl, seriesId)).Result;
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                if (string.IsNullOrEmpty(result)) return null;

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
