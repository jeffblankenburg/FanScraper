using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanScraper
{
    public class NBAGame
    {
        public int Id { get; set; }
        public int ESPNGameNumber { get; set; }
        public int DateCode { get; set; }
        public int Attendance { get; set; }
        public string GameLength { get; set; }

        public string AwayTeamAbbr { get; set; }
        public int AwayPoints { get; set; }
        public int AwayFieldGoalsMade { get; set; }
        public int AwayFieldGoalsAttempted { get; set; }
        public int AwayThreePointersMade { get; set; }
        public int AwayThreePointersAttempted { get; set; }
        public int AwayFreeThrowsMade { get; set; }
        public int AwayFreeThrowsAttempted { get; set; }
        public int AwayOffensiveRebounds { get; set; }
        public int AwayDefensiveRebounds { get; set; }
        public int AwayTotalRebounds { get; set; }
        public int AwayAssists { get; set; }
        public int AwaySteals { get; set; }
        public int AwayBlockedShots { get; set; }
        public int AwayTurnovers { get; set; }
        public int AwayPersonalFouls { get; set; }

        public string HomeTeamAbbr { get; set; }
        public int HomePoints { get; set; }
        public int HomeFieldGoalsMade { get; set; }
        public int HomeFieldGoalsAttempted { get; set; }
        public int HomeThreePointersMade { get; set; }
        public int HomeThreePointersAttempted { get; set; }
        public int HomeFreeThrowsMade { get; set; }
        public int HomeFreeThrowsAttempted { get; set; }
        public int HomeOffensiveRebounds { get; set; }
        public int HomeDefensiveRebounds { get; set; }
        public int HomeTotalRebounds { get; set; }
        public int HomeAssists { get; set; }
        public int HomeSteals { get; set; }
        public int HomeBlockedShots { get; set; }
        public int HomeTurnovers { get; set; }
        public int HomePersonalFouls { get; set; }
    }
}
