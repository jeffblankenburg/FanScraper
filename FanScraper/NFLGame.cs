using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanScraper
{
    public class NFLGame
    {
        public int Id { get; set; }
        public int ESPNGameNumber { get; set; }
        public int WeekNumber { get; set; }
        public int Attendance { get; set; }

        public string ESPNHomeTeamAbbr { get; set; }
        public int HomeTouchdowns { get; set; }
        public int HomeRushingTouchdowns { get; set; }
        public int HomeReceivingTouchdowns { get; set; }
        public int HomeFieldGoalsMade { get; set; }
        public int HomeExtraPointsMade { get; set; }
        public int HomeSafeties { get; set; }
        public int HomeFirstDowns { get; set; }
        public int HomePlayTotal { get; set; }
        public int HomeYardTotal { get; set; }
        public int HomePassingYards { get; set; }
        public int HomePassingCompletions { get; set; }
        public int HomePassingAttempts { get; set; }
        public int HomeInterceptions { get; set; }
        public int HomeSackTotal { get; set; }
        public int HomeSackYardsLost { get; set; }
        public int HomeRushingYards { get; set; }
        public int HomeRushingAttempts { get; set; }
        public int HomeRedZoneScores { get; set; }
        public int HomeRedZoneAttempts { get; set; }
        public int HomePenaltyTotal { get; set; }
        public int HomePenaltyYards { get; set; }
        public int HomeFumblesLost { get; set; }
        public int HomeDefensiveTackles { get; set; }
        public int HomeDefensiveSacks { get; set; }
        public int HomeDefensiveTacklesForLoss { get; set; }
        public int HomeDefensiveOrSpecialTeamsTouchdowns { get; set; }
        public int HomeFieldGoalsAttempted { get; set; }
        public int HomeExtraPointsAttempted { get; set; }
        public int HomeFieldGoalLongest { get; set; }
        public int HomePuntTotal { get; set; }
        public int HomePuntYardage { get; set; }
        public int HomePuntLongest { get; set; }
        public string HomeTimeOfPossession { get; set; }

        public string ESPNAwayTeamAbbr { get; set; }
        public int AwayTouchdowns { get; set; }
        public int AwayRushingTouchdowns { get; set; }
        public int AwayReceivingTouchdowns { get; set; }
        public int AwayFieldGoalsMade { get; set; }
        public int AwayExtraPointsMade { get; set; }
        public int AwaySafeties { get; set; }
        public int AwayFirstDowns { get; set; }
        public int AwayPlayTotal { get; set; }
        public int AwayYardTotal { get; set; }
        public int AwayPassingYards { get; set; }
        public int AwayPassingCompletions { get; set; }
        public int AwayPassingAttempts { get; set; }
        public int AwayInterceptions { get; set; }
        public int AwaySackTotal { get; set; }
        public int AwaySackYardsLost { get; set; }
        public int AwayRushingYards { get; set; }
        public int AwayRushingAttempts { get; set; }
        public int AwayRedZoneScores { get; set; }
        public int AwayRedZoneAttempts { get; set; }
        public int AwayPenaltyTotal { get; set; }
        public int AwayPenaltyYards { get; set; }
        public int AwayFumblesLost { get; set; }
        public int AwayDefensiveTackles { get; set; }
        public int AwayDefensiveSacks { get; set; }
        public int AwayDefensiveTacklesForLoss { get; set; }
        public int AwayDefensiveOrSpecialTeamsTouchdowns { get; set; }
        public int AwayFieldGoalsAttempted { get; set; }
        public int AwayExtraPointsAttempted { get; set; }
        public int AwayFieldGoalLongest { get; set; }
        public int AwayPuntTotal { get; set; }
        public int AwayPuntYardage { get; set; }
        public int AwayPuntLongest { get; set; }
        public string AwayTimeOfPossession { get; set; }
    }
}
