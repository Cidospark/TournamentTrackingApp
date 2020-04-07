using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackerLibrary;
using TrackerLibrary.DataAccess;
using TrackerLibrary.Models;

namespace TrackerUI
{
    public partial class CreatePrizeForm : Form
    {
        IPrizeRequester callingForm;

        public CreatePrizeForm(IPrizeRequester caller)
        {
            InitializeComponent();
            callingForm = caller;
        }

        private void createPrizeButton_Click(object sender, EventArgs e)
        {
            if (validateForm() == "")
            {
                PrizeModel model = new PrizeModel(
                    placeNumberTextBox.Text, 
                    placeNameTextBox.Text,
                    placeAmountTextBox.Text,
                    prizePercentageTextBox.Text
                    );

                // save your model in all the data sources
                foreach(IDataConnection db in GlobalConfig.Connections)
                {
                    db.CreatePrize(model);
                }
                //GlobalConfig.Connections.CreatePrize(model)

                MessageBox.Show("Price Added!", "Success Message");

                callingForm.PrizeComplete(model);
                this.Close();
                //placeNumberTextBox.Text = "";
                //placeNameTextBox.Text = "";
                //placeAmountTextBox.Text = "0";
                //prizePercentageTextBox.Text = "0";

            }
            else
            {
                MessageBox.Show("Fillin the following fields:\n\n" + validateForm());
            }
        }

        private string validateForm()
        {
            string output = "";

            int placeNumber = 0;
            // check if the value entered into the placeNumberTextbox is an integer
            bool placeNumberValidNumber = int.TryParse(placeNumberTextBox.Text, out placeNumber);
            if (!placeNumberValidNumber)
            {
                output += "\nPlace number must be an integer value";
            }

            // check if the value entered is less than 0
            if(placeNumber < 1)
            {
                output += "\nPlace number must not be less than 1"; 
            }

            // check if the price name is not empty
            if (placeNameTextBox.Text.Length == 0)
            {
                output += "\nPlace name must not be empty";
            }


            decimal placeAmount = 0;
            double prizePercentage = 0;

            bool placeAmountValid = decimal.TryParse(placeAmountTextBox.Text, out placeAmount);
            bool prizePercentageValid = double.TryParse(prizePercentageTextBox.Text, out prizePercentage);
            if(!placeAmountValid && !prizePercentageValid)
            {
                output += "\nPlace amount or prize percentage must not be empty or less than 0";
            }

            if(prizePercentage < 0 || prizePercentage > 100)
            {
                output += "\nPlace amount or prize percentage must between 0 and 100";
            }
            
            return output;
        }
    }
}
