using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using MyGolfHandicapMVC.Data;
using MyGolfHandicapCore.Models;

namespace MyGolfHandicapMVC.Controllers
{
    public class ScoreCardsController : Controller
    {
        private IClientAPI _clientAPI;

        public ScoreCardsController(IClientAPI clientAPI)
        {
            _clientAPI = clientAPI;
        }
        public async Task<IActionResult> Scorecards()
        {
            int UserId = HttpContext.Session.GetInt32("userid") ?? 0;

            if (UserId != 0)
            {
                var scorecards = await _clientAPI.GetScoreCardsAsync(UserId);
                ViewBag.ScoreCards = scorecards;
                ViewBag.menuHeader = "Scorecards";
                ViewBag.menu1Class = "unselected";
                ViewBag.menu2Class = "selected";
                ViewBag.menu3Class = "unselected";
                return View();
            }
            else 
            {
                return RedirectToAction("Logout", "Home");
            }
        }

        //[HttpGet("scorecards/addscorecard/{id}/{teeid}")]
        //[Route("api/scorecards/addscorecard/id/teeid")]
        public async Task<IActionResult> AddScoreCard(int id, int id2, int id3)
        {
            int courseid = id;
            int teeid = id2;
            int roundType = id3;
            GolfCourse course = null;
            Tee tee = null;
            IEnumerable<GolfCourse> golfCourses = await _clientAPI.GetGolfCoursesAsync();

            if (courseid == 0)
            {
                course = course = await _clientAPI.GetGolfCourseAsync(1); // Get default golf course
                tee = course.Tees.Where(t => t.TeeColour == "Yellow").FirstOrDefault();
            }
            else
            {
                course = await _clientAPI.GetGolfCourseAsync(courseid);

                if (teeid == 0)
                {
                    tee = course.Tees.Where(t => t.TeeColour == "Yellow").FirstOrDefault();

                    if (tee == null)
                    {
                        tee = course.Tees.FirstOrDefault();
                    }
                }
                else
                {
                    tee = course.Tees.Where(t => t.TeeId == teeid).FirstOrDefault();
                }
            }

            ViewBag.courseSelectList = new SelectList(golfCourses, "GolfCourseId", "Name", course.GolfCourseId);
            ViewBag.teeSelectList = new SelectList(course.Tees, "TeeId", "TeeColour", tee.TeeId);

            // round type 1 = 18, 2 = front 9, 3 = back 9
            switch (roundType)
            {
                case 1:
                    ViewBag.holeInfos = tee.Holes.OrderBy(h => h.HoleNumber).ToList();
                    ViewBag.roundType = 1;
                    ViewBag.score = tee.Holes.Sum(h => h.Par);
                    ViewBag.yards = tee.Holes.Sum(h => h.Yards);
                    break;
                case 2:
                    ViewBag.holeInfos = tee.Holes.Where(h => h.HoleNumber >= 1 && h.HoleNumber <= 9).OrderBy(h => h.HoleNumber).ToList();
                    ViewBag.roundType = 2;
                    ViewBag.score = tee.Holes.Where(h => h.HoleNumber >= 1 && h.HoleNumber <= 9).Sum(h => h.Par);
                    ViewBag.yards = tee.Holes.Where(h => h.HoleNumber >= 1 && h.HoleNumber <= 9).Sum(h => h.Yards);
                    break;
                case 3:
                    ViewBag.holeInfos = tee.Holes.Where(h => h.HoleNumber >= 10 && h.HoleNumber <= 18).OrderBy(h => h.HoleNumber).ToList();
                    ViewBag.roundType = 3;
                    ViewBag.score = tee.Holes.Where(h => h.HoleNumber >= 10 && h.HoleNumber <= 18).Sum(h => h.Par);
                    ViewBag.yards = tee.Holes.Where(h => h.HoleNumber >= 10 && h.HoleNumber <= 18).Sum(h => h.Yards);
                    break;
                default:
                    ViewBag.holeInfos = tee.Holes.OrderBy(h => h.HoleNumber).ToList();
                    ViewBag.roundType = 1;
                    ViewBag.score = tee.Holes.Sum(h => h.Par);
                    ViewBag.yards = tee.Holes.Sum(h => h.Yards);
                    break;
            }

            ViewBag.menuHeader = "Submit Scorecard";
            ViewBag.menu1Class = "unselected";
            ViewBag.menu2Class = "unselected";
            ViewBag.menu3Class = "selected";
            ViewBag.scorecard_id = 5;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddScoreCardSubmit(AddScoreCardForm scoreCard)
        {
            int UserId = HttpContext.Session.GetInt32("userid") ?? 0;

            if (UserId != 0)
            {
                scoreCard.user_id = UserId;
                await _clientAPI.AddScoreCardAsync(scoreCard);
                ViewBag.menuHeader = "Submit Scorecard";
                ViewBag.handicapIndex = await _clientAPI.GetHandicapIndexAsync(UserId);
                ViewBag.menu1Class = "unselected";
                ViewBag.menu2Class = "unselected";
                ViewBag.menu3Class = "selected";
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateScoreCard(int scorecardid)
        {
            ScoreCard scoreCard = await _clientAPI.GetScoreCardAsync(scorecardid);
            GolfCourse course = await _clientAPI.GetGolfCourseAsync(scoreCard.GolfCourseId);
            ViewBag.course = course;
            ViewBag.scoreCard = scoreCard;
            ViewBag.date = scoreCard.Date.ToString("dd'/'MM'/'yyyy");
            ViewBag.score = scoreCard.HoleScores.Sum(h => h.Score);
            ViewBag.yards = scoreCard.HoleScores.Sum(h => h.Yards);
            ViewBag.menuHeader = "Scorecard Details";
            ViewBag.menu1Class = "unselected";
            ViewBag.menu2Class = "unselected";
            ViewBag.menu3Class = "unselected";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateScoreCardSubmit(UpdateScoreCardForm scoreCard)
        {
            int UserId = HttpContext.Session.GetInt32("userid") ?? 0;

            if (UserId != 0)
            {
                scoreCard.user_id = UserId;
                await _clientAPI.UpdateScoreCardAsync(scoreCard);
                ViewBag.menuHeader = "Submit Scorecard";
                ViewBag.handicapIndex = await _clientAPI.GetHandicapIndexAsync(UserId);
                ViewBag.menu1Class = "unselected";
                ViewBag.menu2Class = "unselected";
                ViewBag.menu3Class = "unselected";
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteScorecard(int scorecardid)
        {
            await _clientAPI.DeleteScoreCardAsync(scorecardid);
            return RedirectToAction("Scorecards", "Scorecards");
        }
    }
}
