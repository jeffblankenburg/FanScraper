using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.WindowsAzure.MobileServices;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace FanScraper
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void GetNFLGameList_Click(object sender, RoutedEventArgs e)
        {
            GetSource("http://scores.espn.go.com/nfl/scoreboard?seasonYear=" + NFLYear.Text + "&seasonType=" + NFLType.Text + "&weekNumber=" + NFLWeek.Text, 1, 0);
        }

        private async void GetNFLGameStats_Click(object sender, RoutedEventArgs e)
        {
            var table = App.MobileService.GetTable<NFLWeek>();
            List<NFLWeek> WeekList = await table.Where(week => week.WeekNumber == Int32.Parse(NFLWeek.Text)).OrderBy(game => game.ESPNSortOrder).ToListAsync();

            for (int i = 0; i < WeekList.Count - 1; i++)
            {
                GetSource("http://scores.espn.go.com/nfl/boxscore?gameId=" + WeekList[i].ESPNGameNumber, 2, WeekList[i].ESPNGameNumber);
            }
        }

        private void GetNHLGameList_Click(object sender, RoutedEventArgs e)
        {
            string month = NHLMonth.Text;
            if (month.Length == 1) month = "0" + month;
            string day = NHLDay.Text;
            if (day.Length == 1) day = "0" + day;
            int date = Int32.Parse(NHLYear.Text + month + day);
            GetSource("http://scores.espn.go.com/nhl/scoreboard?date=" + date, 3, date);
        }

        private async void GetSource(string website, int function, int other)
        {
            HttpClient client = new HttpClient();
            string html = await client.GetStringAsync(website);

            switch (function)
            {
                case 1:
                    ParseNFLGameList(html);
                    break;
                case 2:
                    ParseNFLGameStats(html, other);
                    break;
                case 3:
                    ParseNHLGameList(html, other);
                    break;
                default:
                    break;
            }
        }

        private async void ParseNFLGameList(string html)
        {
            string[] Separator = new string[] {"var thisGame = new gameObj(\""};
            string[] quoteSeparator = new string[] {"\""};
            string[] initialSplit;
            string master = html;
            List<NFLWeek> WeekList = new List<NFLWeek>();
            initialSplit = html.Split(Separator, System.StringSplitOptions.RemoveEmptyEntries);

            for (int i = 1; i < initialSplit.Length; i++)
            {
                var split = initialSplit[i].Split(quoteSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                await App.MobileService.GetTable<NFLWeek>().InsertAsync(new NFLWeek(Int32.Parse(NFLWeek.Text), Int32.Parse(split[0]), Int32.Parse(split[2]), Int32.Parse(split[4]), Int32.Parse(split[6])));
            }


        }

        private async void ParseNFLGameStats(string html, int ESPNGameNumber)
        {

            if (!html.Contains("No Boxscore Available"))
            {

                NFLGame Game = new NFLGame();
                Game.ESPNGameNumber = ESPNGameNumber;
                Game.WeekNumber = Int32.Parse(NFLWeek.Text);


                //HTML SeparatorS
                string[] FirstDownSeparator = new string[] { ">1st Downs</td><td>" };
                string[] TotalPlaysSeparator = new string[] { ">Total Plays</td><td>" };
                string[] TotalYardsSeparator = new string[] { ">Total Yards</td><td>" };
                string[] PassingSeparator = new string[] { ">Passing</td><td>" };
                string[] PassingCompletionsSeparator = new string[] { "Comp - Att</div></td><td>" };
                string[] InterceptionsSeparator = new string[] { "Interceptions thrown</div></td><td>" };
                string[] SacksSeparator = new string[] { "Sacks - Yards Lost</div></td><td>" };
                string[] RushingSeparator = new string[] { ">Rushing</td><td>" };
                string[] RushingAttemptsSeparator = new string[] { "Rushing Attempts</div></td><td>" };
                string[] RedZoneSeparator = new string[] { ">Red Zone (Made-Att)</td><td>" };
                string[] PenaltySeparator = new string[] { ">Penalties</td><td>" };
                string[] FumbleSeparator = new string[] { "Fumbles lost</div></td><td>" };
                string[] DefensiveTDSeparator = new string[] { ">Defensive / Special Teams TDs</td><td>" };
                string[] TimeOfPossessionSeparator = new string[] { ">Possession</td><td>" };
                string[] AttendanceSeparator = new string[] { "Attendance:</span> " };
                string[] TeamAbbrSeparator = new string[] { "<div class=\"team-info\"><h3><a href=\"http://espn.go.com/nfl/team/_/name/" };
                string[] RushingTDSeparator = new string[] { "<th>CAR</th><th>YDS</th><th>AVG</th><th>TD</th><th>LG</th>" };
                string[] ReceivingTDSeparator = new string[] { "<th>REC</th><th>YDS</th><th>AVG</th><th>TD</th><th>LG</th><th>TGTS</th>" };
                string[] DefenseSeparator = new string[] { "<th>TOT</th><th>SOLO</th><th>SACKS</th><th>TFL</th><th>PD</th><th>QB HTS</th><th>TD</th>" };
                string[] KickingSeparator = new string[] { "<th>FG</th><th>PCT</th><th>LONG</th><th>XP</th><th>PTS</th>" };
                string[] PuntingSeparator = new string[] { "<th>TOT</th><th>YDS</th><th>AVG</th><th>TB</th><th>-20</th><th>LG</th>" };
                string[] TeamSeparator = new string[] { "Team</th><th>" };
                string[] TDTDSeparator = new string[] { "</td><td>" };
                string[] THTHSeparator = new string[] { "</th><th>" };
                string[] THTRSeparator = new string[] { "</th></tr>" };
                string[] TDTRSeparator = new string[] { "</td></tr>" };
                string[] DIVDIVSeparator = new string[] { "</div></div>" };
                string[] DashSeparator = new string[] { "-" };
                string[] SlashSeparator = new string[] { "/" };

                var temp = new string[1];
                var splitter = new string[1];
                var dashsplitter = new string[1];
                var slashsplitter = new string[1];

                //FIRST DOWNS
                temp = html.Split(FirstDownSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                temp = temp[1].ToString().Split(TDTDSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.AwayFirstDowns = Int32.Parse(temp[0]);
                temp = temp[1].Split(TDTRSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.HomeFirstDowns = Int32.Parse(temp[0]);

                //TOTAL PLAYS
                temp = html.Split(TotalPlaysSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                temp = temp[1].ToString().Split(TDTDSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.AwayPlayTotal = Int32.Parse(temp[0]);
                temp = temp[1].Split(TDTRSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.HomePlayTotal = Int32.Parse(temp[0]);

                //TOTAL YARDS
                temp = html.Split(TotalYardsSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                temp = temp[1].ToString().Split(TDTDSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.AwayYardTotal = Int32.Parse(temp[0]);
                temp = temp[1].Split(TDTRSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.HomeYardTotal = Int32.Parse(temp[0]);

                //PASSING
                temp = html.Split(PassingSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                temp = temp[1].ToString().Split(TDTDSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.AwayPassingYards = Int32.Parse(temp[0]);
                temp = temp[1].Split(TDTRSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.HomePassingYards = Int32.Parse(temp[0]);

                //PASSING COMPLETIONS & ATTEMPTS
                temp = html.Split(PassingCompletionsSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                temp = temp[1].ToString().Split(TDTDSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                dashsplitter = temp[0].Split(DashSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.AwayPassingCompletions = Int32.Parse(dashsplitter[0]);
                Game.AwayPassingAttempts = Int32.Parse(dashsplitter[1]);
                temp = temp[1].Split(TDTRSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                dashsplitter = temp[0].Split(DashSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.HomePassingCompletions = Int32.Parse(dashsplitter[0]);
                Game.HomePassingAttempts = Int32.Parse(dashsplitter[1]);

                //INTERCEPTIONS
                temp = html.Split(InterceptionsSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                temp = temp[1].ToString().Split(TDTDSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.AwayInterceptions = Int32.Parse(temp[0]);
                temp = temp[1].Split(TDTRSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.HomeInterceptions = Int32.Parse(temp[0]);

                //SACKS & YARDS LOST
                temp = html.Split(SacksSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                temp = temp[1].ToString().Split(TDTDSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                dashsplitter = temp[0].Split(DashSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.AwaySackTotal = Int32.Parse(dashsplitter[0]);
                Game.AwaySackYardsLost = Int32.Parse(dashsplitter[1]);
                temp = temp[1].Split(TDTRSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                dashsplitter = temp[0].Split(DashSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.HomeSackTotal = Int32.Parse(dashsplitter[0]);
                Game.HomeSackYardsLost = Int32.Parse(dashsplitter[1]);

                //RUSHING YARDS
                temp = html.Split(RushingSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                temp = temp[1].ToString().Split(TDTDSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.AwayRushingYards = Int32.Parse(temp[0]);
                temp = temp[1].Split(TDTRSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.HomeRushingYards = Int32.Parse(temp[0]);

                //RUSHING YARDS
                temp = html.Split(RushingAttemptsSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                temp = temp[1].ToString().Split(TDTDSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.AwayRushingAttempts = Int32.Parse(temp[0]);
                temp = temp[1].Split(TDTRSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.HomeRushingAttempts = Int32.Parse(temp[0]);

                //RED ZONE SCORES & ATTEMPTS
                temp = html.Split(RedZoneSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                temp = temp[1].ToString().Split(TDTDSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                dashsplitter = temp[0].Split(DashSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.AwayRedZoneScores = Int32.Parse(dashsplitter[0]);
                Game.AwayRedZoneAttempts = Int32.Parse(dashsplitter[1]);
                temp = temp[1].Split(TDTRSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                dashsplitter = temp[0].Split(DashSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.HomeRedZoneScores = Int32.Parse(dashsplitter[0]);
                Game.HomeRedZoneAttempts = Int32.Parse(dashsplitter[1]);

                //PENALTIES
                temp = html.Split(PenaltySeparator, System.StringSplitOptions.RemoveEmptyEntries);
                temp = temp[1].ToString().Split(TDTDSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                dashsplitter = temp[0].Split(DashSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.AwayPenaltyTotal = Int32.Parse(dashsplitter[0]);
                Game.AwayPenaltyYards = Int32.Parse(dashsplitter[1]);
                temp = temp[1].Split(TDTRSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                dashsplitter = temp[0].Split(DashSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.HomePenaltyTotal = Int32.Parse(dashsplitter[0]);
                Game.HomePenaltyYards = Int32.Parse(dashsplitter[1]);

                //FUMBLES
                temp = html.Split(FumbleSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                temp = temp[1].ToString().Split(TDTDSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.AwayFumblesLost = Int32.Parse(temp[0]);
                temp = temp[1].Split(TDTRSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.HomeFumblesLost = Int32.Parse(temp[0]);

                //DEFENSIVE & SPECIAL TEAMS TOUCHDOWNS
                temp = html.Split(DefensiveTDSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                temp = temp[1].ToString().Split(TDTDSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.AwayDefensiveOrSpecialTeamsTouchdowns = Int32.Parse(temp[0]);
                temp = temp[1].Split(TDTRSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.HomeDefensiveOrSpecialTeamsTouchdowns = Int32.Parse(temp[0]);

                //TIME OF POSSESSION
                temp = html.Split(TimeOfPossessionSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                temp = temp[1].ToString().Split(TDTDSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.AwayTimeOfPossession = temp[0];
                temp = temp[1].Split(TDTRSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.HomeTimeOfPossession = temp[0];

                //ATTENDANCE
                temp = html.Split(AttendanceSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                temp = temp[1].ToString().Split(DIVDIVSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.Attendance = Int32.Parse(temp[0].Replace(",", ""));

                //TIME OF POSSESSION
                temp = html.Split(TeamAbbrSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                slashsplitter = temp[1].ToString().Split(SlashSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.ESPNAwayTeamAbbr = slashsplitter[0].ToUpper();
                slashsplitter = temp[2].ToString().Split(SlashSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.ESPNHomeTeamAbbr = slashsplitter[0].ToUpper();

                //RUSHING TOUCHDOWNS
                temp = html.Split(RushingTDSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                splitter = temp[1].ToString().Split(TeamSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                splitter = splitter[1].ToString().Split(THTHSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.AwayRushingTouchdowns = Int32.Parse(splitter[3]);
                splitter = temp[2].ToString().Split(TeamSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                splitter = splitter[1].ToString().Split(THTHSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.HomeRushingTouchdowns = Int32.Parse(splitter[3]);

                //RECEIVING TOUCHDOWNS
                temp = html.Split(ReceivingTDSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                splitter = temp[1].ToString().Split(TeamSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                splitter = splitter[1].ToString().Split(THTHSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.AwayReceivingTouchdowns = Int32.Parse(splitter[3]);
                splitter = temp[2].ToString().Split(TeamSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                splitter = splitter[1].ToString().Split(THTHSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.HomeReceivingTouchdowns = Int32.Parse(splitter[3]);

                //DEFENSIVE TACKLES & SACKS & TACKLES FOR LOSS
                temp = html.Split(DefenseSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                splitter = temp[1].ToString().Split(TeamSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                splitter = splitter[1].ToString().Split(THTHSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.AwayDefensiveTackles = Int32.Parse(splitter[0]);
                Game.AwayDefensiveSacks = Int32.Parse(splitter[2]);
                Game.AwayDefensiveTacklesForLoss = Int32.Parse(splitter[3]);
                splitter = temp[2].ToString().Split(TeamSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                splitter = splitter[1].ToString().Split(THTHSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.HomeDefensiveTackles = Int32.Parse(splitter[0]);
                Game.HomeDefensiveSacks = Int32.Parse(splitter[2]);
                Game.HomeDefensiveTacklesForLoss = Int32.Parse(splitter[3]);

                //FIELD GOALS MADE & ATTEMPTED, EXTRA POINTS MADE & ATTEMPTED, LONGEST FIELD GOAL 
                temp = html.Split(KickingSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                splitter = temp[1].ToString().Split(TeamSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                splitter = splitter[1].ToString().Split(THTHSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                if (splitter[2] != "--") Game.AwayFieldGoalLongest = Int32.Parse(splitter[2]);
                slashsplitter = splitter[0].Split(SlashSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.AwayFieldGoalsMade = Int32.Parse(slashsplitter[0]);
                Game.AwayFieldGoalsAttempted = Int32.Parse(slashsplitter[1]);
                slashsplitter = splitter[3].Split(SlashSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.AwayExtraPointsMade = Int32.Parse(slashsplitter[0]);
                Game.AwayExtraPointsAttempted = Int32.Parse(slashsplitter[1]);
                splitter = temp[2].ToString().Split(TeamSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                splitter = splitter[1].ToString().Split(THTHSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                if (splitter[2] != "--") Game.HomeFieldGoalLongest = Int32.Parse(splitter[2]);
                slashsplitter = splitter[0].Split(SlashSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.HomeFieldGoalsMade = Int32.Parse(slashsplitter[0]);
                Game.HomeFieldGoalsAttempted = Int32.Parse(slashsplitter[1]);
                slashsplitter = splitter[3].Split(SlashSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.HomeExtraPointsMade = Int32.Parse(slashsplitter[0]);
                Game.HomeExtraPointsAttempted = Int32.Parse(slashsplitter[1]);

                //PUNT TOTAL & YARDAGE & LONGEST
                temp = html.Split(PuntingSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                splitter = temp[1].ToString().Split(TeamSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                splitter = splitter[1].ToString().Split(THTHSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.AwayPuntYardage = Int32.Parse(splitter[0]);
                Game.AwayPuntTotal = Int32.Parse(splitter[1]);
                splitter = splitter[5].Split(THTRSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.AwayPuntLongest = Int32.Parse(splitter[0]);
                splitter = temp[2].ToString().Split(TeamSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                splitter = splitter[1].ToString().Split(THTHSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.HomePuntYardage = Int32.Parse(splitter[0]);
                Game.HomePuntTotal = Int32.Parse(splitter[1]);
                splitter = splitter[5].Split(THTRSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.HomePuntLongest = Int32.Parse(splitter[0]);

                //TOTAL TOUCHDOWNS
                Game.HomeTouchdowns = Game.HomeRushingTouchdowns + Game.HomeReceivingTouchdowns + Game.HomeDefensiveOrSpecialTeamsTouchdowns;
                Game.AwayTouchdowns = Game.AwayRushingTouchdowns + Game.AwayReceivingTouchdowns + Game.AwayDefensiveOrSpecialTeamsTouchdowns;

                await App.MobileService.GetTable<NFLGame>().InsertAsync(Game);
            }

        }

        private async void ParseNHLGameList(string html, int datecode)
        {
            string[] Separator = new string[] { "var thisGame = new gameObj(\"" };
            string[] quoteSeparator = new string[] { "\"" };
            string[] initialSplit;
            string master = html;
            List<NHLDay> WeekList = new List<NHLDay>();
            initialSplit = html.Split(Separator, System.StringSplitOptions.RemoveEmptyEntries);

            for (int i = 1; i < initialSplit.Length; i++)
            {
                var split = initialSplit[i].Split(quoteSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                await App.MobileService.GetTable<NHLDay>().InsertAsync(new NHLDay(datecode, Int32.Parse(split[0]), Int32.Parse(split[2]), Int32.Parse(split[4])));
            }
        }

        private void GetNHLGameStats_Click(object sender, RoutedEventArgs e)
        {
            
        }

    }
}
