using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.DataAccess;

namespace TrackerLibrary
{
    public static class GlobalConfig
    {

        public const string PrizesFile = "PrizeModels.csv";
        public const string PeopleFile = "PersonModels.csv";
        public const string TeamFile = "TeamModels.csv";
        public const string TournamentFile = "TournamentModels.csv";
        public const string MatchupFile = "MatchupModels.csv";
        public const string MatchupEntryFile = "MatchupEntryModels.csv";

        // only methods in this class can change the settings of the connection cause the 'set' parameter is private
        public static List<IDataConnection> Connections { get; private set; } = new List<IDataConnection>();

        public static void InitializeConnections(bool database, bool textFiles)
        {
            if (database)
            {
                // TODO - Create SQL Connection
                SQLConnector sql = new SQLConnector();
                Connections.Add(sql);
            }

            if (textFiles)
            {
                // TODO - Create Text Connection
                TextConnector text = new TextConnector();
                Connections.Add(text);
            }
        }

        public static string ConString(string name)
        {
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }
    }
}

// public static IDataConnection Connection {get; set;}
// add a class file and add enum
/*
 * public enum DatabaseType{Sql, TextFile}
 */
/*
  public static void InitializeConnections(DatabaseType db)
        {
            if (db == DatabaseType.Sql)
            {
                // TODO - Create SQL Connection
                SQLConnector sql = new SQLConnector();
                Connections = sql;
            }

            if (textFiles)
            {
                // TODO - Create Text Connection
                TextConnector text = new TextConnector();
                Connections = text;
            }
        }

*/
