using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanScraper
{
    public class NHLDay
    {
        public int Id { get; set; }
        public int DateCode { get; set; }
        public int ESPNGameNumber { get; set;}
        public int ESPNHomeTeamID { get; set; }
        public int ESPNAwayTeamID { get; set; }
        public int ESPNSortOrder { get; set; }

        public NHLDay(int datecode, int game, int home, int away)
        {
            DateCode = datecode;
            ESPNGameNumber = game;
            ESPNHomeTeamID = home;
            ESPNAwayTeamID = away;
        }
    }
}
