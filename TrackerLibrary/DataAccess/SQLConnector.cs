using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;
using System.Data;
using System.Data.SqlClient;

namespace TrackerLibrary.DataAccess
{
    public class SQLConnector : IDataConnection
    {
        private const string db = "Tournaments";
        public PersonModel CreatePerson(PersonModel model)
        {
            using (IDbConnection connection = new SqlConnection(GlobalConfig.ConString(db)))
            {
                var p = new DynamicParameters();
                p.Add("@FirstName", model.FirstName);
                p.Add("@LastName", model.LastName);
                p.Add("@EmailAddress", model.EmailAddress);
                p.Add("@CellphoneNumber", model.CellPhoneNumber);
                p.Add("@Id", 0, dbType: DbType.Int32, ParameterDirection.Output);

                connection.Execute("dbo.spPeople_Insert", p, commandType: CommandType.StoredProcedure);

                // The Id of the entry generated at the database and returned
                model.Id = p.Get<int>("@Id");

                return model;
            }
        }

        public PrizeModel CreatePrize(PrizeModel model)
        {
            // create connection object, create dynamic parameters object from the dapper class
            // add fields to it, use the connection object to access the execute command
            //
            using(IDbConnection connection = new SqlConnection(GlobalConfig.ConString(db)))
            {
                var p = new DynamicParameters();
                p.Add("@PlaceNumber", model.PlaceNumber);
                p.Add("@PlaceName", model.PlaceName);
                p.Add("@PrizeAmount", model.PlaceAmount);
                p.Add("@PrizePercentage", model.PlacePercentage);
                p.Add("@Id", 0, dbType: DbType.Int32, ParameterDirection.Output);

                connection.Execute("dbo.spPrizes_Insert", p, commandType: CommandType.StoredProcedure);

                // The Id of the entry generated at the database and returned
                model.Id = p.Get<int>("@Id");

                return model;
            }
        }

        public List<PersonModel> GetAllPersons()
        {
            List<PersonModel> output;
            using (IDbConnection connection = new SqlConnection(GlobalConfig.ConString(db))) {
                output = connection.Query<PersonModel>("dbo.spPeople_GetAll").ToList();
            }

            return output;
        }
    }
}
