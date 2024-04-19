using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JudgePlacement.JSON
{
    public class TabroomHTTPClient
    {
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

        public async Task<HttpResponseMessage?> TournamentDataToFile(string filePath, string tournId)
        {
            Task<HttpResponseMessage?> response = GetTournamentData(tournId);

            if (response.Result == null)
                return null;

            Stream content = await response.Result.Content.ReadAsStreamAsync();

            FileStream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 256000000, true);

            await content.CopyToAsync(stream);

            return response.Result;
        }
    }
}
