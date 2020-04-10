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
    public partial class CreateTournamentForm : Form, IPrizeRequester, ITeamRequester
    {
        List<TeamModel> availableTeams = new List<TeamModel>();
        List<TeamModel> selectedTeams = new List<TeamModel>();
        List<PrizeModel> selectedPrizes = new List<PrizeModel>();
        public CreateTournamentForm()
        {
            InitializeComponent();
            LoadListData();
            WireUpLists();
        }

        private void LoadListData()
        {
            try
            {
                if (GlobalConfig.Connections.Count > 1)
                {
                    availableTeams = new SQLConnector().GetAllTeams();
                }
                else
                {
                    if (GlobalConfig.Connections[0].ToString() == "TrackerLibrary.DataAccess.TextConnector")
                        availableTeams = new TextConnector().GetAllTeams();
                    else
                        availableTeams = new SQLConnector().GetAllTeams();
                }

            }
            catch (Exception ex) { MessageBox.Show($"Error loading into listbox, {ex.Message}", "Error Message"); }

        }

        private void WireUpLists()
        {

            selectTeamDropDown.DataSource = null;
            selectTeamDropDown.DataSource = availableTeams;
            selectTeamDropDown.DisplayMember = "TeamName";

            tournamentTeamListBox.DataSource = null;
            tournamentTeamListBox.DataSource = selectedTeams;
            tournamentTeamListBox.DisplayMember = "TeamName";

            prizesListBox.DataSource = null;
            prizesListBox.DataSource = selectedPrizes;
            prizesListBox.DisplayMember = "PlaceName";

        }

        private void addTeamButton_Click(object sender, EventArgs e)
        {
            TeamModel t = (TeamModel)selectTeamDropDown.SelectedItem;
            if(t != null)
            {
                availableTeams.Remove(t);
                selectedTeams.Add(t);
                WireUpLists();
            }
        }

        private void createPriceButton_Click(object sender, EventArgs e)
        {
            // call the create prize for 
            CreatePrizeForm frm = new CreatePrizeForm(this);
            frm.Show();
        }

        public void PrizeComplete(PrizeModel model)
        {
            // get back from the form a prize model
            // take the prize model and put it into our list of the selected prize
            selectedPrizes.Add(model);
            WireUpLists();
        }

        public void TeamComplete(TeamModel model)
        {
            selectedTeams.Add(model);
            WireUpLists();
        }

        private void createNewTeamLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CreateTeamForm frm = new CreateTeamForm(this);
            frm.Show();
        }

        private void deleteSelectedPlayerButton_Click(object sender, EventArgs e)
        {
            // get the selected item from the listbox
            TeamModel t = (TeamModel)tournamentTeamListBox.SelectedItem;

            // if item is not null, remove item from list and add to dropdown
            if(t != null)
            {
                selectedTeams.Remove(t);
                availableTeams.Add(t);
                WireUpLists();
            }

        }

        private void deleteSelectedPrizeButton_Click(object sender, EventArgs e)
        {
            PrizeModel p = (PrizeModel)prizesListBox.SelectedItem;
            if(p != null)
            {
                selectedPrizes.Remove(p);

                WireUpLists();
            }
        }

        private void createTournamentButton_Click(object sender, EventArgs e)
        {
            decimal fee = 0;
            bool feeAcceptable = decimal.TryParse(entryFeeText.Text, out fee);

            if (!feeAcceptable)
            {
                MessageBox.Show("Entry fee is needed!","Invalid Entry", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // create tournament entry
            TournamentModel tm = new TournamentModel();

            tm.TournamentName = tournamentNameText.Text;
            tm.EntryFee = fee;

            tm.Prizes = selectedPrizes;
            tm.EnteredTeams = selectedTeams;

            // TODO - create our matchups
            TournamentLoginc.CreateRounds(tm);


            // create all of the prizes entries
            // create all of the team entries
            //try
            //{
                if (GlobalConfig.Connections.Count > 1)
                {
                    new SQLConnector().CreateTournament(tm);
                }
                else
                {
                    if (GlobalConfig.Connections[0].ToString() == "TrackerLibrary.DataAccess.TextConnector")
                        new TextConnector().CreateTournament(tm);
                    else
                        new SQLConnector().CreateTournament(tm);
                }

            //}
            //catch (Exception ex) { MessageBox.Show($"Error saving, {ex.Message}", "Error Message"); }
            
            TournamentViewerForm frm = new TournamentViewerForm(tm);
            frm.Show();
            this.Close();
        }
    }
}
