using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;

namespace TrackerLibrary
{
    public static class TournamentLoginc
    {        
        public static void CreateRounds(TournamentModel model)
        {
            List<TeamModel> randomizedTeams = RamdomizeTeamOrder(model.EnteredTeams);
            int rounds = FindNumberOfRounds(randomizedTeams.Count);
            int byes = NumberOfByes(rounds, randomizedTeams.Count);

            model.Round.Add(CreateFirstRound(byes, randomizedTeams));

            CreateOtherRounds(model, rounds);

        }
        public static void UpdateTournamentResults(TournamentModel model)
        {
            int startingRound = model.CheckCurrentRound();
            List<MatchupModel> toScore = new List<MatchupModel>();
            foreach (List<MatchupModel> round in model.Round)
            {
                foreach (MatchupModel rm in round)
                {
                    if(rm.Winner == null && (rm.Entries.Any(x => x.Score != 0) || rm.Entries.Count == 1))
                    {
                        toScore.Add(rm);
                    }
                }
            }

            MarkWinnersInMatchups(toScore);
            AdvanceWinners(toScore, model);

            foreach(MatchupModel m in toScore)
            {
                if (GlobalConfig.Connections.Count > 1)
                {
                    GlobalConfig.Connections[0].UpdateMatchup(m);
                }
                else
                {
                    if (GlobalConfig.Connections[0].ToString() == "TrackerLibrary.DataAccess.TextConnector")
                        GlobalConfig.Connections[0].UpdateMatchup(m);
                    else
                        GlobalConfig.Connections[0].UpdateMatchup(m);
                }
            }

            int endingRound = model.CheckCurrentRound();

            if(endingRound > startingRound)
            {
                model.AlertUsersToNewRound();
            }
        }

        public static void AlertUsersToNewRound(this TournamentModel model)
        {
            int currentRoundNumber = model.CheckCurrentRound();
            List<MatchupModel> currentRound = model.Round.Where(x => x.First().MatchopRound == currentRoundNumber).First();

            foreach(MatchupModel matchup in currentRound)
            {
                foreach(MatchupEntryModel me in matchup.Entries)
                {
                    foreach(PersonModel p in me.TeamCompeting.TeamMembers)
                    {
                        AlertPersonToNewRound(p, me.TeamCompeting.TeamName, matchup.Entries.Where(x => x.TeamCompeting != me.TeamCompeting).FirstOrDefault());
                    }
                }
            }
        }

        private static void AlertPersonToNewRound(PersonModel p, string teamName, MatchupEntryModel competitor)
        {
            if(p.EmailAddress.Length == 0)
            {
                return;
            }

            string to = "";
            string subject = "";
            StringBuilder body = new StringBuilder();

            if(competitor != null)
            {
                subject = $"You have a new matchup with { competitor.TeamCompeting.TeamName }";
                body.AppendLine("<h1>You have a new matchup</h1>");
                body.Append("<strong>Competitor: </strong>");
                body.Append(competitor.TeamCompeting.TeamName);
                body.AppendLine();
                body.AppendLine();
                body.AppendLine("Have a greate day!");
                body.AppendLine("~Tournament Tracker~");
            }
            else
            {
                subject = "You have a bye week this round";
                body.AppendLine("Entry your round off!");
                body.AppendLine("~Tournament Tracker~");
            }
            to = p.EmailAddress;
            EmailLogic.SendEmail(to, subject, body.ToString());
        }

        private static int CheckCurrentRound(this TournamentModel model)
        {
            int output = 1;
            foreach(List<MatchupModel> round in model.Round)
            {
                // round.All -> means if all match
                // round.Any -> means if any match
                if (round.All(x => x.Winner != null))
                {
                    output += 1;
                }
            }
            return output; 
        }
        private static void AdvanceWinners(List<MatchupModel> models, TournamentModel tournament)
        {
            foreach (MatchupModel m in models) // loop through every matchup
            {
                foreach (List<MatchupModel> round in tournament.Round) // loop through every tournament round
                {
                    foreach (MatchupModel rm in round) // loop through every  round
                    {
                        foreach (MatchupEntryModel me in rm.Entries) // loop through every entries
                        {
                            if (me.ParentMatchup != null)
                            {
                                if (me.ParentMatchup.Id == m.Id)
                                {
                                    me.TeamCompeting = m.Winner;
                                    if (GlobalConfig.Connections.Count > 1)
                                    {
                                        GlobalConfig.Connections[0].UpdateMatchup(m);
                                    }
                                    else
                                    {
                                        if (GlobalConfig.Connections[0].ToString() == "TrackerLibrary.DataAccess.TextConnector")
                                            GlobalConfig.Connections[0].UpdateMatchup(m);
                                        else
                                            GlobalConfig.Connections[0].UpdateMatchup(m);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void MarkWinnersInMatchups(List<MatchupModel> models)
        {
            // greater or lesser
            string greaterWins = ConfigurationManager.AppSettings["greaterWins"];

            foreach (MatchupModel m in models)
            { 
                // checks for bye wee entry
                if(m.Entries.Count == 1)
                {
                    m.Winner = m.Entries[0].TeamCompeting;
                    continue;
                }

                // 0 means false, or low score wins
                if (greaterWins == "0")
                {
                    if(m.Entries[0].Score < m.Entries[1].Score)
                    {
                        m.Winner = m.Entries[0].TeamCompeting;
                    }else if(m.Entries[1].Score < m.Entries[0].Score)
                    {
                        m.Winner = m.Entries[1].TeamCompeting;
                    }
                    else
                    {
                        throw new Exception("Application does not support ties.");
                    }
                }
                else
                {
                    if (m.Entries[0].Score > m.Entries[1].Score)
                    {
                        m.Winner = m.Entries[0].TeamCompeting;
                    }
                    else if (m.Entries[1].Score > m.Entries[0].Score)
                    {
                        m.Winner = m.Entries[1].TeamCompeting;
                    }
                    else
                    {
                        throw new Exception("Application does not support ties.");
                    }
                }
            }
        }

        private static void CreateOtherRounds(TournamentModel model, int rounds)
        {
            int round = 2;
            List<MatchupModel> prevRound = model.Round[0];
            List<MatchupModel> currRound = new List<MatchupModel>();
            MatchupModel currMatchup = new MatchupModel();

            while (round <= rounds)
            {
                foreach(MatchupModel match in prevRound)
                {
                    currMatchup.Entries.Add(new MatchupEntryModel { ParentMatchup = match });
                
                    if(currMatchup.Entries.Count > 1)
                    {
                        currMatchup.MatchopRound = round;
                        currRound.Add(currMatchup);
                        currMatchup = new MatchupModel();
                    }
                }

                model.Round.Add(currRound);
                prevRound = currRound;

                currRound = new List<MatchupModel>();
                round += 1;
            }
        }

        private static List<MatchupModel> CreateFirstRound(int byes, List<TeamModel> teams)
        {
            List<MatchupModel> output = new List<MatchupModel>();
            MatchupModel curr = new MatchupModel();

            foreach(TeamModel team in teams)
            {
                curr.Entries.Add(new MatchupEntryModel { TeamCompeting = team });

                if(byes > 0 || curr.Entries.Count > 1)
                {
                    curr.MatchopRound = 1;
                    output.Add(curr);
                    curr = new MatchupModel();

                    if(byes > 0)
                    {
                        byes -= 1;
                    }
                }

            }

            return output;
        }

        private static int NumberOfByes(int rounds, int numberOfTeams)
        {
            int output = 0;
            int totalTeams = 0;

            for(int i=1; i<rounds; i++)
            {
                totalTeams *= 2;
            }

            output = totalTeams - numberOfTeams;
            return output;
        }

        private static int FindNumberOfRounds(int teamCount)
        {
            int output = 1;
            int val = 2;
            while (val < teamCount)
            {
                output += 1;
                val *= 2;
            }
            return output;
        }

        private static List<TeamModel> RamdomizeTeamOrder(List<TeamModel> teams)
        {
            return teams.OrderBy(x => Guid.NewGuid()).ToList();
        }
    }
}
