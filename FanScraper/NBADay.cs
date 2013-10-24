using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanScraper
{
    public class NBADay
    {
        public int Id { get; set; }
        public int DateCode { get; set; }
        public int ESPNGameNumber { get; set;}
        public int ESPNHomeTeamID { get; set; }
        public int ESPNAwayTeamID { get; set; }

        public NBADay(int datecode, int game, int home, int away)
        {
            DateCode = datecode;
            ESPNGameNumber = game;
            ESPNHomeTeamID = home;
            ESPNAwayTeamID = away;
        }
    }
}
