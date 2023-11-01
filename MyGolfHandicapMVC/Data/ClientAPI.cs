using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyGolfHandicapMVC.Data;
using MyGolfHandicapCore.Models;
using System.Text;
using System.Text.Json;
using System.Net.Http.Json;

namespace MyGolfHandicapMVC.Data
{
    public class ClientAPI : IClientAPI
    {
        private HttpClient httpClient;
        private string _basicAuthenticationCredentials;
        private string _baseUrl;

        public ClientAPI()
        {
            httpClient = new HttpClient();
            _basicAuthenticationCredentials = Settings.BasicAuthenticationCredentials;
            _baseUrl = Settings.BaseURL;
        }
        public async Task<decimal> GetHandicapIndexAsync(int userID)
        {
            string content = "0.0";
            string url = _baseUrl + "/api/handicapindex/" + userID.ToString();
            var test2 = Encoding.ASCII.GetBytes(_basicAuthenticationCredentials);
            var auth = Convert.ToBase64String(test2);

            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", auth);

            HttpResponseMessage response = await httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                content = await response.Content.ReadAsStringAsync();
            }
            else
            {
                Console.WriteLine("Failed get handicapindex");
            }

            return Convert.ToDecimal(content);
        }

        public async Task<int> GetUserId(string username, string password)
        {
            var auth = Convert.ToBase64String(Encoding.ASCII.GetBytes(_basicAuthenticationCredentials));
            string url = _baseUrl + "/api/user";
            int userID = 0;

            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", auth);

            var user = new UserForm();

            user.username = username;
            user.password = password;

            var user_json = JsonSerializer.Serialize(user);

            var requestContent = new StringContent(user_json, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(url, requestContent);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                userID = Convert.ToInt32(content);
            }

            return userID;
        }

        public async Task<IEnumerable<ScoreCard>> GetScoreCardsAsync(int userID)
        {
            string url = _baseUrl + "/api/scorecards/" + userID.ToString(); 
            var auth = Convert.ToBase64String(Encoding.ASCII.GetBytes(_basicAuthenticationCredentials));

            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", auth);

            HttpResponseMessage response = await httpClient.GetAsync(url);

            var jasoString = await response.Content.ReadAsStringAsync();

            var scoreCards = JsonSerializer.Deserialize<IEnumerable<ScoreCard>>(jasoString);

            return scoreCards;
        }

        public async Task<IEnumerable<GolfCourse>> GetGolfCoursesAsync()
        {
            var auth = Convert.ToBase64String(Encoding.ASCII.GetBytes(_basicAuthenticationCredentials));

            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", auth);

            HttpResponseMessage response = await httpClient.GetAsync(_baseUrl + "/api/golfcourses");

            var jasoString = await response.Content.ReadAsStringAsync();

            List<GolfCourse> golfCourses = JsonSerializer.Deserialize<IEnumerable<GolfCourse>>(jasoString).ToList();

            return golfCourses;
        }

        public async Task<GolfCourse> GetGolfCourseAsync(int golfCourseId)
        {
            var auth = Convert.ToBase64String(Encoding.ASCII.GetBytes(_basicAuthenticationCredentials));
            string url = _baseUrl + "/api/golfcourses/" + golfCourseId.ToString();

            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", auth);

            HttpResponseMessage response = await httpClient.GetAsync(url);

            var jasoString = await response.Content.ReadAsStringAsync();

            GolfCourse course = JsonSerializer.Deserialize<GolfCourse>(jasoString);

            return course;
        }

        public async Task<IEnumerable<Tee>> GetTeesAsync(int golfCourseId)
        {
            var auth = Convert.ToBase64String(Encoding.ASCII.GetBytes(_basicAuthenticationCredentials));
            string url = _baseUrl + "/api/tees/" + golfCourseId.ToString();

            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", auth);

            HttpResponseMessage response = await httpClient.GetAsync(url);

            var jasoString = await response.Content.ReadAsStringAsync();

            List<Tee> tees = JsonSerializer.Deserialize<IEnumerable<Tee>>(jasoString).ToList();

            return tees;
        }

        public async Task<ScoreCard> GetScoreCardAsync(int scoreCardId)
        {
            var auth = Convert.ToBase64String(Encoding.ASCII.GetBytes(_basicAuthenticationCredentials));
            string url = _baseUrl + "/api/scorecard/" + scoreCardId.ToString();

            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", auth);

            HttpResponseMessage response = await httpClient.GetAsync(url);

            var jasoString = await response.Content.ReadAsStringAsync();

            ScoreCard scoreCard = JsonSerializer.Deserialize<ScoreCard>(jasoString);

            return scoreCard;
        }

        public async Task<int> AddScoreCardAsync(AddScoreCardForm scoreCard)
        {
            var auth = Convert.ToBase64String(Encoding.ASCII.GetBytes(_basicAuthenticationCredentials));
            string url = _baseUrl  + "/api/scorecard";

            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", auth);

            var scoreCard_json = JsonSerializer.Serialize(scoreCard);
            var requestContent = new StringContent(scoreCard_json, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(url, requestContent);
            response.EnsureSuccessStatusCode();
            return 1;
        }

        public async Task<int> UpdateScoreCardAsync(UpdateScoreCardForm scoreCard)
        {
            var auth = Convert.ToBase64String(Encoding.ASCII.GetBytes(_basicAuthenticationCredentials));
            string url = _baseUrl + "/api/scorecard";

            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", auth);

            var scoreCard_json = JsonSerializer.Serialize(scoreCard);
            var requestContent = new StringContent(scoreCard_json, Encoding.UTF8, "application/json");
            var response = await httpClient.PutAsync(url, requestContent);
            response.EnsureSuccessStatusCode();
            return 1;
        }

        public async Task<int> DeleteScoreCardAsync(int scoreCardId)
        {
            var auth = Convert.ToBase64String(Encoding.ASCII.GetBytes(_basicAuthenticationCredentials));
            string url = _baseUrl + "/api/scorecard/" + scoreCardId.ToString();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", auth);
            HttpResponseMessage response = await httpClient.DeleteAsync(url);
            response.EnsureSuccessStatusCode();
            return 1;
        }
    }
}
