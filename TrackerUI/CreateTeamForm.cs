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
    public partial class CreateTeamForm : Form
    {
        public CreateTeamForm()
        {
            InitializeComponent();
        }

        private void createMemberButton_Click(object sender, EventArgs e)
        {
            // validate the form
            if (validateForm() == "")
            {
                PersonModel p = new PersonModel();
                p.FirstName = firstNameText.Text;
                p.LastName = LastNameTextBox.Text;
                p.EmailAddress = emailTextBox.Text;
                p.CellPhoneNumber = cellphoneTextBox.Text;

                foreach (IDataConnection db in GlobalConfig.Connections)
                {
                    db.CreatePerson(p);
                }

                firstNameText.Text = "";
                LastNameTextBox.Text = "";
                emailTextBox.Text = "";
                cellphoneTextBox.Text = "";
            }
            else
            {
                MessageBox.Show($"The following boxes are empty:\n\n{validateForm()}", "Validation Box");
            }
        }

        private void createTeamButton_Click(object sender, EventArgs e)
        {
            
        }

        private string validateForm()
        {
            string output = "";
            if(firstNameText.Text.Length == 0)
            {
                output += "\nFirst name is empty.\n";
            }

            if(LastNameTextBox.Text.Length == 0)
            {
                output += "\nLast name is empty.\n";
            }

            if(emailTextBox.Text.Length == 0)
            {
                output += "\nEmail is empty.\n";
            }

            if(cellphoneTextBox.Text.Length == 0)
            {
                output += "\nCell phone is empty\n";
            }

            return output;
        }

       
    }
}
