using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary.Models
{
    public class PrizeModel
    {
        public int PlaceNumber { get; set; }
        public string PlaceName { get; set; }
        public decimal PlaceAmount { get; set; }
        public double PlacePercentage { get; set; }

        public PrizeModel()
        {
        }

        public PrizeModel(string placeName, string placeNumber, string placeAmount, string prizePercentage)
        {
            // default = <value>;
            // datatype.TryParse(<Value from textbox>, out <Returned textbox value if valid or default assigned value if invalid >)

            PlaceName = placeName;

            int placeNumberValue = 0;
            int.TryParse(placeNumber, out placeNumberValue);
            PlaceNumber = placeNumberValue;

            decimal placeAmountValue = 0;
            decimal.TryParse(placeAmount, out placeAmountValue);
            PlaceAmount = placeAmountValue;

            double prizePercentageValue = 0;
            double.TryParse(prizePercentage, out prizePercentageValue);
            PlacePercentage = prizePercentageValue;

        }
    }
}
