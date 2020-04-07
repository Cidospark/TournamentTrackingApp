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
        private List<PersonModel> availableTeamMembers = new List<PersonModel>();
        private List<PersonModel> selectedTeamMembers = new List<PersonModel>();
        private ITeamRequester callingForm;

        public CreateTeamForm(ITeamRequester caller)
        {
            InitializeComponent();
            callingForm = caller;

            //CreateSampleData();
            LoadListData();
            WireUpList();
        }

        private void LoadListData()
        {
            try
            {
                if (GlobalConfig.Connections.Count > 1)
                {
                    availableTeamMembers = new SQLConnector().GetAllPersons();
                }
                else
                {
                    if (GlobalConfig.Connections[0].ToString() == "TrackerLibrary.DataAccess.TextConnector")
                        availableTeamMembers = new TextConnector().GetAllPersons();
                    else
                        availableTeamMembers = new SQLConnector().GetAllPersons();
                }
            }
            catch(Exception ex) { MessageBox.Show($"Error loading into listbox, {ex.Message}", "Error Message"); }

        }

        private void CreateSampleData()
        {
            availableTeamMembers.Add(new PersonModel { FirstName = "Tim", LastName = "Corey" });
            selectedTeamMembers.Add(new PersonModel { FirstName = "John", LastName = "Doe" });
        }

        private void WireUpList()
        {
            selectTeamMemberDropDown.DataSource = null;
            selectTeamMemberDropDown.DataSource = availableTeamMembers;
            selectTeamMemberDropDown.DisplayMember = "FullName";

            teamMembersListBox.DataSource = null;
            teamMembersListBox.DataSource = selectedTeamMembers;
            teamMembersListBox.DisplayMember = "FullName";
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

                // added to selected team members on creation
                selectedTeamMembers.Add(p);
                WireUpList();

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
            TeamModel t = new TeamModel();

            t.TeamName = teamNameText.Text;
            t.TeamMembers = selectedTeamMembers;

            try
            {
                if (GlobalConfig.Connections.Count > 1)
                {
                    t = new SQLConnector().CreateTeam(t);
                }
                else
                {
                    if (GlobalConfig.Connections[0].ToString() == "TrackerLibrary.DataAccess.TextConnector")
                        t = new TextConnector().CreateTeam(t);
                    else
                        t = new SQLConnector().CreateTeam(t);
                }

                MessageBox.Show($"Added!");

                callingForm.TeamComplete(t);
                this.Close();

            }
            catch (Exception ex) { MessageBox.Show($"Error loading into listbox, {ex.Message}", "Error Message"); }
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

        private void addMemberButton_Click(object sender, EventArgs e)
        {
            PersonModel p = (PersonModel)selectTeamMemberDropDown.SelectedItem;
            if (p != null)
            {
                availableTeamMembers.Remove(p);
                selectedTeamMembers.Add(p);
                WireUpList();
            }
        }

        private void removeSelectedMemberButton_Click(object sender, EventArgs e)
        {
            PersonModel p = (PersonModel)teamMembersListBox.SelectedItem;
            if(p != null)
            {
                selectedTeamMembers.Remove(p);
                availableTeamMembers.Insert(0, p);
                WireUpList();
            }
        }
    }
}
