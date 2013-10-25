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
        string[] BRSeperator = new string[] { "<br>" };
        string[] TDTDSeperator = new string[] { "</td><td>" };
        string[] THTHSeperator = new string[] { "</th><th>" };
        string[] THTRSeperator = new string[] { "</th></tr>" };
        string[] TDTRSeperator = new string[] { "</td></tr>" };
        string[] DIVDIVSeperator = new string[] { "</div></div>" };
        string[] DashSeperator = new string[] { "-" };
        string[] SlashSeperator = new string[] { "/" };
        
        
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

            for (int i = 0; i < WeekList.Count; i++)
            {
                GetSource("http://scores.espn.go.com/nfl/boxscore?gameId=" + WeekList[i].ESPNGameNumber, 2, WeekList[i].ESPNGameNumber);
            }
        }

        private void GetNHLGameList_Click(object sender, RoutedEventArgs e)
        {
            int date = GetNHLDateCode();
            GetSource("http://scores.espn.go.com/nhl/scoreboard?date=" + date, 3, date);
        }

        private async void GetNHLGameStats_Click(object sender, RoutedEventArgs e)
        {
            var table = App.MobileService.GetTable<NHLDay>();
            List<NHLDay> DayList = await table.Where(day => day.DateCode == GetNHLDateCode()).OrderBy(game => game.ESPNGameNumber).ToListAsync();

            for (int i = 0; i < DayList.Count - 1; i++)
            {
                GetSource("http://scores.espn.go.com/nhl/boxscore?gameId=" + DayList[i].ESPNGameNumber, 4, DayList[i].ESPNGameNumber);
            }
        }

        private void GetNBAGameList_Click(object sender, RoutedEventArgs e)
        {
            int date = GetNBADateCode();
            GetSource("http://scores.espn.go.com/nba/scoreboard?date=" + date, 5, date);
        }

        private async void GetNBAGameStats_Click(object sender, RoutedEventArgs e)
        {
            var table = App.MobileService.GetTable<NBADay>();
            List<NBADay> DayList = await table.Where(day => day.DateCode == GetNBADateCode()).OrderBy(game => game.ESPNGameNumber).ToListAsync();

            for (int i = 0; i < DayList.Count; i++)
            {
                GetSource("http://scores.espn.go.com/nba/boxscore?gameId=" + DayList[i].ESPNGameNumber, 6, DayList[i].ESPNGameNumber);
            }
        }

        private int GetNHLDateCode()
        {
            string month = NHLMonth.Text;
            if (month.Length == 1) month = "0" + month;
            string day = NHLDate.Text;
            if (day.Length == 1) day = "0" + day;
            int date = Int32.Parse(NHLYear.Text + month + day);
            return date;
        }

        private int GetNBADateCode()
        {
            string month = NBAMonth.Text;
            if (month.Length == 1) month = "0" + month;
            string day = NBADate.Text;
            if (day.Length == 1) day = "0" + day;
            int date = Int32.Parse(NBAYear.Text + month + day);
            return date;
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
                case 4:
                    ParseNHLGameStats(html, other);
                    break;
                case 5:
                    ParseNBAGameList(html, other);
                    break;
                case 6:
                    ParseNBAGameStats(html, other);
                    break;
                default:
                    break;
            }
        }

        private async void ParseNFLGameList(string html)
        {
            string[] Seperator = new string[] {"var thisGame = new gameObj(\""};
            string[] quoteSeperator = new string[] {"\""};
            string[] initialSplit;
            string master = html;
            List<NFLWeek> WeekList = new List<NFLWeek>();
            initialSplit = html.Split(Seperator, System.StringSplitOptions.RemoveEmptyEntries);

            for (int i = 1; i < initialSplit.Length; i++)
            {
                var split = initialSplit[i].Split(quoteSeperator, System.StringSplitOptions.RemoveEmptyEntries);
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


                //HTML SeperatorS
                string[] FirstDownSeperator = new string[] { ">1st Downs</td><td>" };
                string[] TotalPlaysSeperator = new string[] { ">Total Plays</td><td>" };
                string[] TotalYardsSeperator = new string[] { ">Total Yards</td><td>" };
                string[] PassingSeperator = new string[] { ">Passing</td><td>" };
                string[] PassingCompletionsSeperator = new string[] { "Comp - Att</div></td><td>" };
                string[] InterceptionsSeperator = new string[] { "Interceptions thrown</div></td><td>" };
                string[] SacksSeperator = new string[] { "Sacks - Yards Lost</div></td><td>" };
                string[] RushingSeperator = new string[] { ">Rushing</td><td>" };
                string[] RushingAttemptsSeperator = new string[] { "Rushing Attempts</div></td><td>" };
                string[] RedZoneSeperator = new string[] { ">Red Zone (Made-Att)</td><td>" };
                string[] PenaltySeperator = new string[] { ">Penalties</td><td>" };
                string[] FumbleSeperator = new string[] { "Fumbles lost</div></td><td>" };
                string[] DefensiveTDSeperator = new string[] { ">Defensive / Special Teams TDs</td><td>" };
                string[] TimeOfPossessionSeperator = new string[] { ">Possession</td><td>" };
                string[] AttendanceSeperator = new string[] { "Attendance:</span> " };
                string[] TeamAbbrSeperator = new string[] { "<div class=\"team-info\"><h3><a href=\"http://espn.go.com/nfl/team/_/name/" };
                string[] RushingTDSeperator = new string[] { "<th>CAR</th><th>YDS</th><th>AVG</th><th>TD</th><th>LG</th>" };
                string[] ReceivingTDSeperator = new string[] { "<th>REC</th><th>YDS</th><th>AVG</th><th>TD</th><th>LG</th><th>TGTS</th>" };
                string[] DefenseSeperator = new string[] { "<th>TOT</th><th>SOLO</th><th>SACKS</th><th>TFL</th><th>PD</th><th>QB HTS</th><th>TD</th>" };
                string[] KickingSeperator = new string[] { "<th>FG</th><th>PCT</th><th>LONG</th><th>XP</th><th>PTS</th>" };
                string[] PuntingSeperator = new string[] { "<th>TOT</th><th>YDS</th><th>AVG</th><th>TB</th><th>-20</th><th>LG</th>" };
                string[] TeamSeperator = new string[] { "Team</th><th>" };

                var temp = new string[1];
                var splitter = new string[1];
                var dashsplitter = new string[1];
                var slashsplitter = new string[1];

                //FIRST DOWNS
                temp = html.Split(FirstDownSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                temp = temp[1].ToString().Split(TDTDSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.AwayFirstDowns = Int32.Parse(temp[0]);
                temp = temp[1].Split(TDTRSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.HomeFirstDowns = Int32.Parse(temp[0]);

                //TOTAL PLAYS
                temp = html.Split(TotalPlaysSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                temp = temp[1].ToString().Split(TDTDSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.AwayPlayTotal = Int32.Parse(temp[0]);
                temp = temp[1].Split(TDTRSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.HomePlayTotal = Int32.Parse(temp[0]);

                //TOTAL YARDS
                temp = html.Split(TotalYardsSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                temp = temp[1].ToString().Split(TDTDSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.AwayYardTotal = Int32.Parse(temp[0]);
                temp = temp[1].Split(TDTRSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.HomeYardTotal = Int32.Parse(temp[0]);

                //PASSING
                temp = html.Split(PassingSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                temp = temp[1].ToString().Split(TDTDSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.AwayPassingYards = Int32.Parse(temp[0]);
                temp = temp[1].Split(TDTRSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.HomePassingYards = Int32.Parse(temp[0]);

                //PASSING COMPLETIONS & ATTEMPTS
                temp = html.Split(PassingCompletionsSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                temp = temp[1].ToString().Split(TDTDSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                dashsplitter = temp[0].Split(DashSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.AwayPassingCompletions = Int32.Parse(dashsplitter[0]);
                Game.AwayPassingAttempts = Int32.Parse(dashsplitter[1]);
                temp = temp[1].Split(TDTRSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                dashsplitter = temp[0].Split(DashSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.HomePassingCompletions = Int32.Parse(dashsplitter[0]);
                Game.HomePassingAttempts = Int32.Parse(dashsplitter[1]);

                //INTERCEPTIONS
                temp = html.Split(InterceptionsSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                temp = temp[1].ToString().Split(TDTDSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.AwayInterceptions = Int32.Parse(temp[0]);
                temp = temp[1].Split(TDTRSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.HomeInterceptions = Int32.Parse(temp[0]);

                //SACKS & YARDS LOST
                temp = html.Split(SacksSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                temp = temp[1].ToString().Split(TDTDSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                dashsplitter = temp[0].Split(DashSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.AwaySackTotal = Int32.Parse(dashsplitter[0]);
                Game.AwaySackYardsLost = Int32.Parse(dashsplitter[1]);
                temp = temp[1].Split(TDTRSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                dashsplitter = temp[0].Split(DashSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.HomeSackTotal = Int32.Parse(dashsplitter[0]);
                Game.HomeSackYardsLost = Int32.Parse(dashsplitter[1]);

                //RUSHING YARDS
                temp = html.Split(RushingSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                temp = temp[1].ToString().Split(TDTDSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.AwayRushingYards = Int32.Parse(temp[0]);
                temp = temp[1].Split(TDTRSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.HomeRushingYards = Int32.Parse(temp[0]);

                //RUSHING YARDS
                temp = html.Split(RushingAttemptsSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                temp = temp[1].ToString().Split(TDTDSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.AwayRushingAttempts = Int32.Parse(temp[0]);
                temp = temp[1].Split(TDTRSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.HomeRushingAttempts = Int32.Parse(temp[0]);

                //RED ZONE SCORES & ATTEMPTS
                temp = html.Split(RedZoneSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                temp = temp[1].ToString().Split(TDTDSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                dashsplitter = temp[0].Split(DashSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.AwayRedZoneScores = Int32.Parse(dashsplitter[0]);
                Game.AwayRedZoneAttempts = Int32.Parse(dashsplitter[1]);
                temp = temp[1].Split(TDTRSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                dashsplitter = temp[0].Split(DashSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.HomeRedZoneScores = Int32.Parse(dashsplitter[0]);
                Game.HomeRedZoneAttempts = Int32.Parse(dashsplitter[1]);

                //PENALTIES
                temp = html.Split(PenaltySeperator, System.StringSplitOptions.RemoveEmptyEntries);
                temp = temp[1].ToString().Split(TDTDSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                dashsplitter = temp[0].Split(DashSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.AwayPenaltyTotal = Int32.Parse(dashsplitter[0]);
                Game.AwayPenaltyYards = Int32.Parse(dashsplitter[1]);
                temp = temp[1].Split(TDTRSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                dashsplitter = temp[0].Split(DashSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.HomePenaltyTotal = Int32.Parse(dashsplitter[0]);
                Game.HomePenaltyYards = Int32.Parse(dashsplitter[1]);

                //FUMBLES
                temp = html.Split(FumbleSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                temp = temp[1].ToString().Split(TDTDSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.AwayFumblesLost = Int32.Parse(temp[0]);
                temp = temp[1].Split(TDTRSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.HomeFumblesLost = Int32.Parse(temp[0]);

                //DEFENSIVE & SPECIAL TEAMS TOUCHDOWNS
                temp = html.Split(DefensiveTDSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                temp = temp[1].ToString().Split(TDTDSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.AwayDefensiveOrSpecialTeamsTouchdowns = Int32.Parse(temp[0]);
                temp = temp[1].Split(TDTRSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.HomeDefensiveOrSpecialTeamsTouchdowns = Int32.Parse(temp[0]);

                //TIME OF POSSESSION
                temp = html.Split(TimeOfPossessionSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                temp = temp[1].ToString().Split(TDTDSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.AwayTimeOfPossession = temp[0];
                temp = temp[1].Split(TDTRSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.HomeTimeOfPossession = temp[0];

                //ATTENDANCE
                temp = html.Split(AttendanceSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                if (temp.Length > 1)
                {
                    temp = temp[1].ToString().Split(DIVDIVSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                    Game.Attendance = Int32.Parse(temp[0].Replace(",", ""));
                }

                //TIME OF POSSESSION
                temp = html.Split(TeamAbbrSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                slashsplitter = temp[1].ToString().Split(SlashSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.ESPNAwayTeamAbbr = slashsplitter[0].ToUpper();
                slashsplitter = temp[2].ToString().Split(SlashSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.ESPNHomeTeamAbbr = slashsplitter[0].ToUpper();

                //RUSHING TOUCHDOWNS
                temp = html.Split(RushingTDSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                splitter = temp[1].ToString().Split(TeamSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                splitter = splitter[1].ToString().Split(THTHSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.AwayRushingTouchdowns = Int32.Parse(splitter[3]);
                splitter = temp[2].ToString().Split(TeamSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                splitter = splitter[1].ToString().Split(THTHSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.HomeRushingTouchdowns = Int32.Parse(splitter[3]);

                //RECEIVING TOUCHDOWNS
                temp = html.Split(ReceivingTDSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                splitter = temp[1].ToString().Split(TeamSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                splitter = splitter[1].ToString().Split(THTHSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.AwayReceivingTouchdowns = Int32.Parse(splitter[3]);
                splitter = temp[2].ToString().Split(TeamSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                splitter = splitter[1].ToString().Split(THTHSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.HomeReceivingTouchdowns = Int32.Parse(splitter[3]);

                //DEFENSIVE TACKLES & SACKS & TACKLES FOR LOSS
                temp = html.Split(DefenseSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                splitter = temp[1].ToString().Split(TeamSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                splitter = splitter[1].ToString().Split(THTHSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.AwayDefensiveTackles = Int32.Parse(splitter[0]);
                Game.AwayDefensiveSacks = Int32.Parse(splitter[2]);
                Game.AwayDefensiveTacklesForLoss = Int32.Parse(splitter[3]);
                splitter = temp[2].ToString().Split(TeamSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                splitter = splitter[1].ToString().Split(THTHSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.HomeDefensiveTackles = Int32.Parse(splitter[0]);
                Game.HomeDefensiveSacks = Int32.Parse(splitter[2]);
                Game.HomeDefensiveTacklesForLoss = Int32.Parse(splitter[3]);

                //FIELD GOALS MADE & ATTEMPTED, EXTRA POINTS MADE & ATTEMPTED, LONGEST FIELD GOAL 
                temp = html.Split(KickingSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                splitter = temp[1].ToString().Split(TeamSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                splitter = splitter[1].ToString().Split(THTHSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                if (splitter[2] != "--") Game.AwayFieldGoalLongest = Int32.Parse(splitter[2]);
                slashsplitter = splitter[0].Split(SlashSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.AwayFieldGoalsMade = Int32.Parse(slashsplitter[0]);
                Game.AwayFieldGoalsAttempted = Int32.Parse(slashsplitter[1]);
                slashsplitter = splitter[3].Split(SlashSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.AwayExtraPointsMade = Int32.Parse(slashsplitter[0]);
                Game.AwayExtraPointsAttempted = Int32.Parse(slashsplitter[1]);
                splitter = temp[2].ToString().Split(TeamSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                splitter = splitter[1].ToString().Split(THTHSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                if (splitter[2] != "--") Game.HomeFieldGoalLongest = Int32.Parse(splitter[2]);
                slashsplitter = splitter[0].Split(SlashSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.HomeFieldGoalsMade = Int32.Parse(slashsplitter[0]);
                Game.HomeFieldGoalsAttempted = Int32.Parse(slashsplitter[1]);
                slashsplitter = splitter[3].Split(SlashSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.HomeExtraPointsMade = Int32.Parse(slashsplitter[0]);
                Game.HomeExtraPointsAttempted = Int32.Parse(slashsplitter[1]);

                //PUNT TOTAL & YARDAGE & LONGEST
                temp = html.Split(PuntingSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                splitter = temp[1].ToString().Split(TeamSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                splitter = splitter[1].ToString().Split(THTHSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.AwayPuntYardage = Int32.Parse(splitter[0]);
                Game.AwayPuntTotal = Int32.Parse(splitter[1]);
                splitter = splitter[5].Split(THTRSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.AwayPuntLongest = Int32.Parse(splitter[0]);
                splitter = temp[2].ToString().Split(TeamSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                splitter = splitter[1].ToString().Split(THTHSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.HomePuntYardage = Int32.Parse(splitter[0]);
                Game.HomePuntTotal = Int32.Parse(splitter[1]);
                splitter = splitter[5].Split(THTRSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.HomePuntLongest = Int32.Parse(splitter[0]);

                //TOTAL TOUCHDOWNS
                Game.HomeTouchdowns = Game.HomeRushingTouchdowns + Game.HomeReceivingTouchdowns + Game.HomeDefensiveOrSpecialTeamsTouchdowns;
                Game.AwayTouchdowns = Game.AwayRushingTouchdowns + Game.AwayReceivingTouchdowns + Game.AwayDefensiveOrSpecialTeamsTouchdowns;

                await App.MobileService.GetTable<NFLGame>().InsertAsync(Game);
            }

        }

        private async void ParseNHLGameList(string html, int datecode)
        {
            string[] Seperator = new string[] { "var thisGame = new gameObj(\"" };
            string[] quoteSeperator = new string[] { "\"" };
            string[] initialSplit;
            string master = html;
            List<NHLDay> WeekList = new List<NHLDay>();
            initialSplit = html.Split(Seperator, System.StringSplitOptions.RemoveEmptyEntries);

            for (int i = 1; i < initialSplit.Length; i++)
            {
                var split = initialSplit[i].Split(quoteSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                await App.MobileService.GetTable<NHLDay>().InsertAsync(new NHLDay(datecode, Int32.Parse(split[0]), Int32.Parse(split[2]), Int32.Parse(split[4])));
            }
        }

        private async void ParseNHLGameStats(string html, int ESPNGameNumber)
        {
            NHLGame Game = new NHLGame();
        }

        private async void ParseNBAGameList(string html, int datecode)
        {
            string[] Seperator = new string[] { "var thisGame = new gameObj(\"" };
            string[] quoteSeperator = new string[] { "\"" };
            string[] initialSplit;
            string master = html;
            List<NBADay> WeekList = new List<NBADay>();
            initialSplit = html.Split(Seperator, System.StringSplitOptions.RemoveEmptyEntries);

            for (int i = 1; i < initialSplit.Length; i++)
            {
                var split = initialSplit[i].Split(quoteSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                await App.MobileService.GetTable<NBADay>().InsertAsync(new NBADay(datecode, Int32.Parse(split[0]), Int32.Parse(split[2]), Int32.Parse(split[4])));
            }
        }

        private async void ParseNBAGameStats(String html, int ESPNGameNumber)
        {
            if (!html.Contains("Box Score not available"))
            {

                NBAGame Game = new NBAGame();
                Game.ESPNGameNumber = ESPNGameNumber;
                Game.DateCode = GetNBADateCode();

                var temp = new string[1];
                var splitter = new string[1];
                var dashsplitter = new string[1];
                var slashsplitter = new string[1];

                string[] StatsSeperator = new string[] { "</tr></thead><tbody><tr align=right class=\"even\"><td style=\"text-align:left\" colspan=2></td><td>" };
                string[] AttendanceSeperator = new string[] { "<strong>&nbsp;Attendance:</strong> " };
                string[] TimeOfGameSeperator = new string[] { "<strong>&nbsp;Time of Game:</strong> " };
                string[] AwayTeamSeperator = new string[] {"</div><div class=\"team-info\"><h3 abbr=\"false\"><a href=\"http://espn.go.com/nba/team/_/name/"};
                string[] HomeTeamSeperator = new string[] { "</div><div class=\"team-info\"><h3><a href=\"http://espn.go.com/nba/team/_/name/" };

                //AWAY STATS
                temp = html.Replace("<strong>","").Replace("</strong>","").Split(StatsSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                temp = temp[1].ToString().Split(TDTDSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                dashsplitter = temp[0].Split(DashSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.AwayFieldGoalsMade = Int32.Parse(dashsplitter[0]);
                Game.AwayFieldGoalsAttempted = Int32.Parse(dashsplitter[1]);
                dashsplitter = temp[1].Split(DashSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.AwayThreePointersMade = Int32.Parse(dashsplitter[0]);
                Game.AwayThreePointersAttempted = Int32.Parse(dashsplitter[1]);
                dashsplitter = temp[2].Split(DashSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.AwayFreeThrowsMade = Int32.Parse(dashsplitter[0]);
                Game.AwayFreeThrowsAttempted = Int32.Parse(dashsplitter[1]);

                Game.AwayOffensiveRebounds = Int32.Parse(temp[3]);
                Game.AwayDefensiveRebounds = Int32.Parse(temp[4]);
                Game.AwayTotalRebounds = Int32.Parse(temp[5]);
                Game.AwayAssists = Int32.Parse(temp[6]);
                Game.AwaySteals = Int32.Parse(temp[7]);
                Game.AwayBlockedShots = Int32.Parse(temp[8]);
                Game.AwayTurnovers = Int32.Parse(temp[9]);
                Game.AwayPersonalFouls = Int32.Parse(temp[10]);
                splitter = temp[12].Split(TDTRSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.AwayPoints = Int32.Parse(splitter[0]);

                //HOME STATS
                temp = html.Replace("<strong>", "").Replace("</strong>", "").Split(StatsSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                temp = temp[2].ToString().Split(TDTDSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                dashsplitter = temp[0].Split(DashSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.HomeFieldGoalsMade = Int32.Parse(dashsplitter[0]);
                Game.HomeFieldGoalsAttempted = Int32.Parse(dashsplitter[1]);
                dashsplitter = temp[1].Split(DashSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.HomeThreePointersMade = Int32.Parse(dashsplitter[0]);
                Game.HomeThreePointersAttempted = Int32.Parse(dashsplitter[1]);
                dashsplitter = temp[2].Split(DashSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.HomeFreeThrowsMade = Int32.Parse(dashsplitter[0]);
                Game.HomeFreeThrowsAttempted = Int32.Parse(dashsplitter[1]);

                Game.HomeOffensiveRebounds = Int32.Parse(temp[3]);
                Game.HomeDefensiveRebounds = Int32.Parse(temp[4]);
                Game.HomeTotalRebounds = Int32.Parse(temp[5]);
                Game.HomeAssists = Int32.Parse(temp[6]);
                Game.HomeSteals = Int32.Parse(temp[7]);
                Game.HomeBlockedShots = Int32.Parse(temp[8]);
                Game.HomeTurnovers = Int32.Parse(temp[9]);
                Game.HomePersonalFouls = Int32.Parse(temp[10]);
                splitter = temp[12].Split(TDTRSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.HomePoints = Int32.Parse(splitter[0]);

                //ATTENDANCE
                temp = html.Split(AttendanceSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                if (temp.Length > 1)
                {
                    splitter = temp[1].ToString().Replace(",", "").Split(BRSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                    if (splitter[0].Length < 6) Game.Attendance = Int32.Parse(splitter[0]);
                }

                //GAME LENGTH
                temp = html.Split(TimeOfGameSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                if (temp.Length > 1)
                {
                    splitter = temp[1].ToString().Replace(",", "").Split(BRSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                    if (splitter[0].Length < 6) Game.GameLength = splitter[0];
                }

                //AWAY TEAM ABBREVIATION
                temp = html.Split(AwayTeamSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                splitter = temp[1].ToString().Split(SlashSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.AwayTeamAbbr = splitter[0].ToUpper();

                //HOME TEAM ABBREVIATION
                temp = html.Split(HomeTeamSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                splitter = temp[1].ToString().Split(SlashSeperator, System.StringSplitOptions.RemoveEmptyEntries);
                Game.HomeTeamAbbr = splitter[0].ToUpper();

                await App.MobileService.GetTable<NBAGame>().InsertAsync(Game);
                
            }
        }

    }
}
