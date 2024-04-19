using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JudgePlacement.JSON
{
    public class TabroomHTTPClient
    {
        public bool HasLoggedIn { get; set; } = false;

        public HttpClient Client = new()
        {
            BaseAddress = new Uri("https://www.tabroom.com")
        };

        public async Task<HttpResponseMessage?> Login(string username, string password)
        {
            var formData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", password),
            });

            var loginResponse = await Client.PostAsync("/user/login/login_save.mhtml", formData);

            return loginResponse;
        }

        private async Task<HttpResponseMessage?> GetTournamentData(string tournId)
        {
            return await Client.GetAsync($"/api/download_data?tourn_id={tournId}");
        }

        public string TournamentDataToString(string tournId)
        {
            Task<HttpResponseMessage?> response = GetTournamentData(tournId);

            if (response.Result == null)
                return string.Empty;

            Stream content = response.Result.Content.ReadAsStream();

            StreamReader reader = new StreamReader(content);

            return reader.ReadToEnd();
        }
    }
}
