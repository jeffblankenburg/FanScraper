using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanScraper
{
    public class NFLWeek
    {
        public int Id { get; set; }
        public int WeekNumber { get; set; }
        public int ESPNGameNumber { get; set;}
        public int ESPNHomeTeamID { get; set; }
        public int ESPNAwayTeamID { get; set; }
        public int ESPNSortOrder { get; set; }

        public NFLWeek(int week, int game, int home, int away, int sort)
        {
            WeekNumber = week;
            ESPNGameNumber = game;
            ESPNHomeTeamID = home;
            ESPNAwayTeamID = away;
            ESPNSortOrder = sort;
        }
    }
}
