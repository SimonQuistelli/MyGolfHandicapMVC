using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyGolfHandicapCore.Models;
using MyGolfHandicapMVC.Data;

namespace MyGolfHandicapMVC.Controllers
{
    public class HandicapIndexController : Controller
    {
        private IClientAPI _clientAPI;

        public HandicapIndexController(IClientAPI clientAPI)
        {
            _clientAPI = clientAPI;
        }
        [Authorize]
        public async Task<IActionResult> HandicapIndex()
        {
            int UserId = HttpContext.Session.GetInt32("userid") ?? 0;

            if (UserId != 0)
            {
                decimal handicapIndex = await _clientAPI.GetHandicapIndexAsync(UserId);
                List<string> handicapTrendData = new List<string>();
                List<string> handicapTrendDataDate = new List<string>();
                var scoreCards = await _clientAPI.GetScoreCardsAsync(UserId);
                scoreCards = scoreCards.OrderBy(s => s.Date).TakeLast(10);

                foreach (ScoreCard scoreCard in scoreCards)
                {
                    handicapTrendData.Add(scoreCard.HandicapIndex.ToString("0.0"));
                    handicapTrendDataDate.Add(scoreCard.Date.ToString());
                }

                ViewBag.handicapIndex = handicapIndex;
                ViewBag.handicapTrendData = handicapTrendData;
                ViewBag.scoreCards = scoreCards;
                ViewBag.menuHeader = "Handicap Index";
                ViewBag.menu1Class = "selected";
                ViewBag.menu2Class = "unselected";
                ViewBag.menu3Class = "unselected";
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
    }
}
