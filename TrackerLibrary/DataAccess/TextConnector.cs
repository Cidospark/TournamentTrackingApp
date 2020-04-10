using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;
using TrackerLibrary.DataAccess.TextHelpers;

namespace TrackerLibrary.DataAccess
{
    public class TextConnector : IDataConnection
    {
        public void CreatePerson(PersonModel model)
        {
            List<PersonModel> people = GlobalConfig.PeopleFile.FulFilePath().LoadFile().ConvertToPersonModel();

            int currentId = 1;
            if (people.Count > 0)
            {
                currentId = people.OrderByDescending(x => x.Id).First().Id + 1;
            }
            model.Id = currentId;

            people.Add(model);

            people.SaveToPeopleFile();
        }

        // TODO - Wire up the CreatePrize for textfiles
        public void CreatePrize(PrizeModel model)
        {
            // Load the text file and convert the text to List<PrizeModel>
            List<PrizeModel> prizes = GlobalConfig.PrizesFile.FulFilePath().LoadFile().ConvertToPrizeModel();

            // Find the max ID
            int currentId = 1;
            if(prizes.Count > 0)
            {
                currentId = prizes.OrderByDescending(x => x.Id).First().Id + 1;
            }
            model.Id = currentId;
            
            // Add the new record with the new ID (max + 1)
            prizes.Add(model);

            // Convert the prize to the List<string>
            // Save the List<string> to the text file
            prizes.SaveToPrizeFile();
        }

        public void CreateTeam(TeamModel model)
        {
            // take the file addressa above
            // Load every lines in it
            List<TeamModel> teams = GlobalConfig.TeamFile.FulFilePath().LoadFile().ConvertToTeamModel();

            // Find the max ID
            int currentId = 1;
            if (teams.Count > 0)
            {
                currentId = teams.OrderByDescending(x => x.Id).First().Id + 1;
            }
            model.Id = currentId;

            teams.Add(model);

            teams.SaveToTeamFile();

        }

        public void CreateTournament(TournamentModel model)
        {
            List<TournamentModel> tournaments = GlobalConfig.TournamentFile.FulFilePath().LoadFile()
                .ConvertToTournamentModel();

            int currentId = 1;
            if(tournaments.Count > 0)
            {
                currentId = tournaments.OrderByDescending(x => x.Id).First().Id + 1;
            }

            model.Id = currentId;

            model.SaveRoundsToFile();

            tournaments.Add(model);
            tournaments.SaveToTournamentFile();

            TournamentLoginc.UpdateTournamentResults(model);
        }

        public List<PersonModel> GetAllPersons()
        {
            return GlobalConfig.PeopleFile.FulFilePath().LoadFile().ConvertToPersonModel();
        }

        public List<TeamModel> GetAllTeams()
        {
            return GlobalConfig.TeamFile.FulFilePath().LoadFile().ConvertToTeamModel();
        }

        public List<TournamentModel> GetAllTournaments()
        {
            return GlobalConfig.TournamentFile
                .FulFilePath()
                .LoadFile()
                .ConvertToTournamentModel();
        }

        public void UpdateMatchup(MatchupModel model)
        {
            model.UpdateMatchupToFile();
        }
    }
}
