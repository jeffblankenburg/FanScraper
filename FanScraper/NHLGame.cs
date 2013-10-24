using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanScraper
{
    public class NHLGame
    {
        public int Id { get; set; }
        public int ESPNGameNumber { get; set; }
        public int DateCode { get; set; }
        public int Attendance { get; set; }

        public string HomeTeamAbbr { get; set; }
        public int HomeTeamGoals { get; set; }
        public int HomeShotTotal { get; set; }
        public int HomePenaltyMinutes { get; set; }
        public int HomeHits { get; set; }
        public int HomeGiveaways { get; set; }
        public int HomeTakeaways { get; set; }
        public int HomeFaceoffsWon { get; set; }

    }
}
