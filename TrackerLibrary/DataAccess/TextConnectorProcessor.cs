using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using TrackerLibrary.Models;


// Load the text file
// convert the text to List<PrizeModel>
// Find the max ID
// Add the new record with the new ID (max + 1)
// Convert the prize to the List<string>
// Save the List<string> to the text file


namespace TrackerLibrary.DataAccess.TextHelpers
{
    public static class TextConnectorProcessor
    {
        public static string FulFilePath(this string fileName)
        {
            return $"{ConfigurationManager.AppSettings["filePath"]}\\{fileName}";
        }

        public static List<string> LoadFile(this string file)
        {
            // if file does not exists
            if (!File.Exists(file))
            {
                return new List<string>();
            }

            // read all lines from the file
            return File.ReadAllLines(file).ToList();
        }

        public static List<PrizeModel> ConvertToPrizeModel(this List<string> lines)
        {
            List<PrizeModel> output = new List<PrizeModel>();

            foreach(string line in lines)
            {
                string[] cols = line.Split(',');

                PrizeModel p = new PrizeModel();
                p.Id = int.Parse(cols[0]);
                p.PlaceNumber = int.Parse(cols[1]);
                p.PlaceName = cols[2];
                p.PlaceAmount = decimal.Parse(cols[3]);
                p.PlacePercentage = double.Parse(cols[4]);
                output.Add(p);
            }
            return output;
        }
        public static List<PersonModel> ConvertToPersonModel(this List<string> lines)
        {
            List<PersonModel> output = new List<PersonModel>();

            foreach (string line in lines)
            {
                string[] cols = line.Split(',');

                PersonModel p = new PersonModel();
                p.Id = int.Parse(cols[0]);
                p.FirstName = cols[1];
                p.LastName = cols[2];
                p.EmailAddress = cols[3];
                p.CellPhoneNumber = cols[4];
                output.Add(p);
            }
            return output;
        }

        public static List<TeamModel> ConvertToTeamModel(this List<string> lines, string peopleFilename)
        {
            List<TeamModel> output = new List<TeamModel>();

            // get the list of people from the passed-in filename
            List<PersonModel> people = peopleFilename.FulFilePath().LoadFile().ConvertToPersonModel();

            foreach (string line in lines)
            {
                string[] cols = line.Split(',');

                TeamModel t = new TeamModel();
                t.Id = int.Parse(cols[0]);
                t.TeamName = cols[1];

                string[] personIds = cols[2].Split('|');
                foreach(string id in personIds)
                {
                    // take the list of all the people in our text file 
                    // and search for where the id is equal to the person id
                    t.TeamMembers.Add(people.Where(x => x.Id == int.Parse(id)).First());
                }
                output.Add(t);
            }
            return output;
        }
        public static List<TournamentModel> ConvertToTournamentModel(
            this List<string> lines, 
            string teamFileName, 
            string peopleFilename,
            string prizeFileName)
        {
            // id,TournamentName,EntryFee,(id|id|id - Entered Teams), (id|id|id - Prizes), (Rounds - id^id^id|id^id^id|id^id^id)
            List<TournamentModel> output = new List<TournamentModel>();
            List<TeamModel> teams = teamFileName.FulFilePath().LoadFile().ConvertToTeamModel(peopleFilename);
            List<PrizeModel> prizes = prizeFileName.FulFilePath().LoadFile().ConvertToPrizeModel();
            List<MatchupModel> matchups = GlobalConfig.MatchupFile.FulFilePath().LoadFile().ConvertToMatchupModels();

            foreach (string line in lines)
            {
                string[] cols = line.Split(',');
                // construct the tournament model form the broken string
                TournamentModel tm = new TournamentModel(); 
                tm.Id = int.Parse(cols[0]);
                tm.TournamentName = cols[1];
                tm.EntryFee = decimal.Parse(cols[2]);

                string[] teamIds = cols[3].Split('|');
                foreach(string id in teamIds)
                {
                    // find the team with the id that matches the currentId and take the first value
                    tm.EnteredTeams.Add(teams.Where(x => x.Id == int.Parse(id)).First());
                }

                if (cols[4].Length > 0)
                {
                    string[] prizeIds = cols[4].Split('|');

                    foreach (string id in prizeIds)
                    {
                        tm.Prizes.Add(prizes.Where(x => x.Id == int.Parse(id)).First());
                    }
                }

                // Capture rounds information
                string[] rounds = cols[5].Split('|');
                
                foreach(string round in rounds)
                {
                    string[] msText = round.Split('^');
                    List<MatchupModel> ms = new List<MatchupModel>();

                    foreach(string matchupModelTextid in msText)
                    {
                        if(!string.IsNullOrEmpty(matchupModelTextid))
                        {
                            ms.Add(matchups.Where(x => x.Id == int.Parse(matchupModelTextid)).First());
                        }
                        
                    }
                    tm.Round.Add(ms);
                }
                output.Add(tm);
            }

            return output;
        }

        public static void SaveToPrizeFile(this List<PrizeModel> models, string fileName)
        {
            List<string> lines = new List<string>();

            foreach(var p in models)
            {
                lines.Add($"{p.Id}, {p.PlaceNumber}, {p.PlaceName},{p.PlaceAmount},{p.PlacePercentage}");
            }

            File.WriteAllLines(fileName.FulFilePath(), lines);
        } 

        public static void SaveToPeopleFile(this List<PersonModel> models, string fileName)
        {
            List<string> lines = new List<string>();
            foreach(var p in models)
            {
                lines.Add($"{p.Id}, {p.FirstName}, {p.LastName}, {p.EmailAddress}, {p.CellPhoneNumber}");
            }

            File.WriteAllLines(fileName.FulFilePath(), lines);
        }

        public static void SaveToTeamFile(this List<TeamModel> models, string fileName)
        {
            List<string> lines = new List<string>();

            foreach(TeamModel t in models)
            {
                lines.Add($"{ t.Id },{ t.TeamName},{ ConvertPeopleListToString(t.TeamMembers) }");
            }
            File.WriteAllLines(fileName.FulFilePath(), lines);
        }

        public static void SaveRoundsToFile(this TournamentModel model, string matchupFile, string matchupEntryFile)
        {
            // Loop through each round
            // Loop through each mactchup
            // Get the id for the new matchup and save the recored
            // Loop through each entry, Get the Id and save it

            foreach(List<MatchupModel> round in model.Round)
            {
                foreach(MatchupModel matchup in round)
                {
                    // load all of the matchuo from file
                    // get the top id and add 1 to it
                    // store the id
                    // save the matchup record
                    matchup.SaveMatchupToFile(matchupFile, matchupEntryFile);
                    
                }
            }
        }  

        public static List<MatchupEntryModel> ConvertToMatchupEntryModels(this List<string> lines) 
        {
            List<MatchupEntryModel> output = new List<MatchupEntryModel>();
            foreach(string line in lines)
            {
                string[] cols = line.Split(',');
                MatchupEntryModel me = new MatchupEntryModel();
                me.Id = int.Parse(cols[0]);
                if(cols[1].Length == 0)
                {
                    me.TeamCompeting = null;
                }
                else
                {
                    me.TeamCompeting = LookupTeamById(int.Parse(cols[1]));
                }
                me.Score = double.Parse(cols[2]);
                int parentId = 0;
                if(int.TryParse(cols[3], out parentId))
                {
                    me.ParentMatchup = LookupMatchupById(parentId);
                }
                else
                {
                    me.ParentMatchup = null;
                }

                output.Add(me);
            }
            return output;
        }

        private static List<MatchupEntryModel> ConvertStringToMatchupEntryModels(string input)
        {
            string[] ids = input.Split('|');
            List<MatchupEntryModel> output = new List<MatchupEntryModel>();
            List<string> entries = GlobalConfig.MatchupEntryFile.FulFilePath().LoadFile();
            List<string> matchingEntries = new List<string>();

            foreach(string id in ids)
            {
                foreach(string entry in entries)
                {
                    string[] cols = entry.Split(',');
                    if(cols[0] == id)
                    {
                        matchingEntries.Add(entry);
                    }
                }
            }
            output = matchingEntries.ConvertToMatchupEntryModels();

            return output;
        }

        private static TeamModel LookupTeamById(int id)
        {
            List<string> teams = GlobalConfig.TeamFile.FulFilePath().LoadFile(); 
            foreach(string team in teams)
            {
                string[] cols = team.Split(',');
                if(cols[0] == id.ToString())
                {
                    List<string> matchingTeams = new List<string>();
                    matchingTeams.Add(team);
                    return matchingTeams.ConvertToTeamModel(GlobalConfig.PeopleFile).First();
                } 
            }
            return null;
        }

        private static MatchupModel LookupMatchupById(int id)
        {
            List<string> matchups = GlobalConfig.MatchupFile.FulFilePath().LoadFile();
            foreach (string matchup in matchups)
            {
                string[] cols = matchup.Split(',');
                if (cols[0] == id.ToString())
                {
                    List<string> matchingMatchups = new List<string>();
                    matchingMatchups.Add(matchup);
                    return matchingMatchups.ConvertToMatchupModels().First();
                }
            }
            return null;
        }

        public static List<MatchupModel> ConvertToMatchupModels(this List<string> lines)
        {
            List<MatchupModel> output = new List<MatchupModel>();
            foreach (string line in lines)
            {
                string[] cols = line.Split(',');
                MatchupModel p = new MatchupModel();
                p.Id = int.Parse(cols[0]);
                p.Entries = ConvertStringToMatchupEntryModels(cols[1]);
                if(cols[2].Length == 0)
                {
                    p.Winner = null;
                }
                else
                {
                    p.Winner = LookupTeamById(int.Parse(cols[2]));
                }
                
                p.MatchopRound = int.Parse(cols[3]);
                output.Add(p);
            }
            return output;
        }

        public static void SaveMatchupToFile(this MatchupModel matchup, string matchupFile, string matchupEntryFile)
        {
            List<MatchupModel> matchups = GlobalConfig.MatchupFile.FulFilePath().LoadFile().ConvertToMatchupModels();

            int currentId = 1;
            if(matchups.Count > 0)
            {
                currentId = matchups.OrderByDescending(x => x.Id).First().Id + 1;
            }
            matchup.Id = currentId;

            matchups.Add(matchup);

            // save to file
            List<string> lines = new List<string>();
            foreach (MatchupModel m in matchups)
            {
                string winner = "";
                if (m.Winner != null)
                {
                    winner = m.Winner.Id.ToString();
                }
                lines.Add($"{ m.Id },{ "" },{ winner },{ m.MatchopRound }");
            }

            File.WriteAllLines(GlobalConfig.MatchupFile.FulFilePath(), lines);

            foreach (MatchupEntryModel entry in matchup.Entries)
            {
                entry.SaveEntryToFile(matchupEntryFile);
            }

            // save to file
            lines = new List<string>();
            foreach(MatchupModel m in matchups)
            {
                string winner = "";
                if(m.Winner != null)
                {
                    winner = m.Winner.Id.ToString();
                }
                lines.Add($"{ m.Id },{ convertMatchupEntryListToString(m.Entries) },{ winner },{ m.MatchopRound }");
            }

            File.WriteAllLines(GlobalConfig.MatchupFile.FulFilePath(), lines);
        }

        public static void SaveEntryToFile(this MatchupEntryModel entry, string matchupEntryFile)
        {
            List<MatchupEntryModel> entries = GlobalConfig.MatchupEntryFile.FulFilePath().LoadFile().ConvertToMatchupEntryModels();
            int currentId = 1;
            if(entries.Count > 0)
            {
                currentId = entries.OrderByDescending(x => x.Id).First().Id + 1;
            }
            entry.Id = currentId;
            entries.Add(entry);
            // save to file
            List<string> lines = new List<string>();
            foreach(MatchupEntryModel e in entries)
            {
                string parent = "";
                if(e.ParentMatchup != null)
                {
                    parent = e.ParentMatchup.Id.ToString();
                }
                string teamCompeting = "";
                if(e.TeamCompeting != null)
                {
                    teamCompeting = e.TeamCompeting.Id.ToString();
                }
                lines.Add($"{ e.Id },{ teamCompeting },{ e.Score },{ parent }");
            }

            File.WriteAllLines(GlobalConfig.MatchupEntryFile.FulFilePath(), lines);
        }

        public static void SaveToTournamentFile(this List<TournamentModel> models, string fileName)
        {
            List<string> lines = new List<string>();

            foreach (TournamentModel tm in models)
            {
                lines.Add($"{ tm.Id }," +
                    $"{ tm.TournamentName}," +
                    $"{ tm.EntryFee }," +
                    $"{ convertTeamListToString(tm.EnteredTeams) }," +
                    $"{ convertPrizeListToString(tm.Prizes) }," +
                    $"{ convertRoundListToString(tm.Round) }");
            }
            File.WriteAllLines(fileName.FulFilePath(), lines);
        }

        private static string convertRoundListToString(List<List<MatchupModel>> rounds)
        {
            // Rounds - id^id^id|id^id^id|id^id^id
            string output = "";
            if (rounds.Count == 0)
            {
                return "";
            }

            foreach (List<MatchupModel> r in rounds)
            {
                output += $"{ ConvertMatchupListToString(r) }| ";
            }

            output = output.Substring(0, output.Length - 1);

            return output;
        }

        private static string ConvertMatchupListToString(List<MatchupModel> matchups)
        {
            // Round list - id^id^id

            string output = "";
            if (matchups.Count == 0)
            {
                return "";
            }

            foreach (MatchupModel m in matchups)
            {
                if(m != null)
                {
                    output += $"{ m.Id }^";
                }
                else
                {
                    output = "0";
                }
            }

            output = output.Substring(0, output.Length - 1);

            return output;
        }

        private static string convertMatchupEntryListToString(List<MatchupEntryModel> matchups)
        {
            string output = "";
            if (matchups.Count == 0)
            {
                return "";
            }

            foreach (MatchupEntryModel e in matchups)
            {
                output += $"{ e.Id }|";
            }

            output = output.Substring(0, output.Length - 1);

            return output;
        }

        private static string convertPrizeListToString(List<PrizeModel> prizes)
        {
            string output = "";
            if (prizes.Count == 0)
            {
                return "";
            }

            foreach (PrizeModel p in prizes)
            {
                output += $"{ p.Id }|";
            }

            output = output.Substring(0, output.Length - 1);

            return output;
        }

        private static string convertTeamListToString(List<TeamModel> teams)
        {
            string output = "";
            if(teams.Count == 0)
            {
                return "";
            }

            foreach(TeamModel t in teams)
            {
                output += $"{ t.Id }|";
            }

            output = output.Substring(0, output.Length - 1);

            return output;
        }

        private static string ConvertPeopleListToString(List<PersonModel> people)
        {
            string output = "";
            if(people.Count == 0)
            {
                return "";
            }

            foreach(var p in people)
            {
                output += $"{ p.Id}|";
            }

            output = output.Substring(0, output.Length - 1);
            return output;
        }
    }
}
