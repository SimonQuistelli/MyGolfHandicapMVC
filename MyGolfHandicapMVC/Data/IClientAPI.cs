using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyGolfHandicapCore.Models;

namespace MyGolfHandicapMVC.Data
{
    public interface IClientAPI
    {
        Task<decimal> GetHandicapIndexAsync(int userID);

        Task<int> GetUserId(string email, string password);

        Task<IEnumerable<ScoreCard>> GetScoreCardsAsync(int userID);

        Task<IEnumerable<GolfCourse>> GetGolfCoursesAsync();

        Task<GolfCourse> GetGolfCourseAsync(int golfCourseId); 

        Task<IEnumerable<Tee>> GetTeesAsync(int golfCourseId);

        Task<ScoreCard> GetScoreCardAsync(int scoreCardId);

        Task<int> AddScoreCardAsync(AddScoreCardForm scoreCard);

        Task<int> UpdateScoreCardAsync(UpdateScoreCardForm scoreCard);

        Task<int> DeleteScoreCardAsync(int scoreCardId);
    }
}
