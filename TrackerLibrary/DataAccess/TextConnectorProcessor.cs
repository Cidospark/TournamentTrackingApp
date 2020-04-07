using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
