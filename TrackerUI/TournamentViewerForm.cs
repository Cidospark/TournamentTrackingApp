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
    public partial class TournamentViewerForm : Form
    {
        private TournamentModel tournament;
        BindingList<int> rounds = new BindingList<int>();
        BindingList<MatchupModel> selectedMatchups = new BindingList<MatchupModel>();

        public TournamentViewerForm(TournamentModel tournamentModel)
        {
            InitializeComponent();
            tournament = tournamentModel;
            WireUpLists();
            LoadFormData();
            loadRounds();
        }        

        private void LoadFormData()
        {
            tournamentName.Text = tournament.TournamentName;
        }

        private void WireUpLists()
        {
            roundDropDown.DataSource = rounds;
            matchupListBox.DataSource = selectedMatchups;
            matchupListBox.DisplayMember = "DisplayName";
        }
        
        private void loadRounds()
        {
            rounds.Clear();
            rounds.Add(1);
            int currRound = 1;

            foreach(List<MatchupModel> matchups in tournament.Round)
            {
                if (matchups.First().MatchopRound > currRound)
                {
                    currRound = matchups.First().MatchopRound;
                    rounds.Add(currRound);
                }
            }
            LoadMatchups(1);
        }

        private void roundDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadMatchups((int)roundDropDown.SelectedItem);
        }

        private void LoadMatchups(int round)
        {
            foreach(List<MatchupModel> matchups in tournament.Round)
            {
                if(matchups.First().MatchopRound == round)
                {
                    selectedMatchups.Clear();
                    foreach(MatchupModel m in matchups)
                    {
                        if (m.Winner != null || !unplayedOnlyCheckbox.Checked) 
                        {
                            selectedMatchups.Add(m);
                        }
                    }
                }
            }

            if (selectedMatchups.Count > 0)
            {
                LoadMatchup(selectedMatchups.First());
            }

            DisplayMatcupInfo();

        }

        private void DisplayMatcupInfo()
        {
            bool isVisible = (selectedMatchups.Count > 0);

            teamOneNameLabel.Visible = isVisible;
            teamOneScoreLabel.Visible = isVisible;
            teamOneScoreText.Visible = isVisible;
            teamTwoNameLabel.Visible = isVisible;
            teamTwoScoreLabel.Visible = isVisible;
            teamTwoScoreText.Visible = isVisible;
            versusLabel.Visible = isVisible;
            scoreButton.Visible = isVisible;

        }

        private void LoadMatchup(MatchupModel m)
        {
            for(int i = 0; i < m.Entries.Count; i++)
            {
                if(i == 0)
                {
                    if(m.Entries[0].TeamCompeting != null)
                    {
                        teamOneNameLabel.Text = m.Entries[0].TeamCompeting.TeamName;
                        teamOneScoreText.Text = m.Entries[0].Score.ToString();
                        
                        teamTwoNameLabel.Text = "<Bye>";
                        teamTwoScoreText.Text = "0";

                    }
                    else
                    {
                        teamOneNameLabel.Text = "Not yet set";
                        teamOneScoreText.Text = "";
                    }
                }

                if (i == 1)
                {
                    if (m.Entries[0].TeamCompeting != null)
                    {
                        teamTwoNameLabel.Text = m.Entries[1].TeamCompeting.TeamName;
                        teamTwoScoreText.Text = m.Entries[1].Score.ToString();
                    }
                    else
                    {
                        teamTwoNameLabel.Text = "Not yet set";
                        teamTwoScoreText.Text = "";
                    }
                }
            }
        }

        private void matchupListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadMatchup((MatchupModel)matchupListBox.SelectedItem);
        }

        private void unplayedOnlyCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            LoadMatchups((int)roundDropDown.SelectedItem);
        }

        private string validateData()
        {
            string output = "";
            double teamOneScore = 0;
            double teamTwoScore = 0;

            bool scoreOneValid = double.TryParse(teamOneScoreText.Text, out teamOneScore);
            bool scoreTwoValid = double.TryParse(teamTwoScoreText.Text, out teamTwoScore);
            if (scoreOneValid || !scoreTwoValid)
            {
                output = "Score one value is invalid!";
            }else if (!scoreTwoValid)
            {
                output = "Score two value is invalid!";
            }else if (teamOneScore == 0 && teamTwoScore == 0)
            {
                output = "You did not enter score for both teams.";
            }else if(teamOneScore == teamTwoScore)
            {
                output = "Application does not support ties.";
            }

            return output;
        }

        private void scoreButton_Click(object sender, EventArgs e)
        {
            string errorMessage = validateData();
            if (errorMessage.Length > 0)
            {
                MessageBox.Show("Invalid Data!");
                return;
            }

            MatchupModel m = (MatchupModel)matchupListBox.SelectedItem;
            double teamOneScore = 0;
            double teamTwoScore = 0;

            for (int i = 0; i < m.Entries.Count; i++)
            {
                if (i == 0)
                {
                    if (m.Entries[0].TeamCompeting != null)
                    {                        
                        bool scoreValid = double.TryParse(teamOneScoreText.Text, out teamOneScore);
                        if (scoreValid)
                        {
                            m.Entries[0].Score = teamOneScore;
                        }
                        else
                        {
                            MessageBox.Show("Please enter a valid score for team 1.");
                            return;
                        }
                    }
                }

                if (i == 1)
                {
                    if (m.Entries[0].TeamCompeting != null)
                    { 
                        bool scoreValid = double.TryParse(teamTwoScoreText.Text, out teamTwoScore);
                        if (scoreValid)
                        {
                            m.Entries[1].Score = teamTwoScore;
                        }
                        else
                        {
                            MessageBox.Show("Please enter a valid score for team 2.");
                            return;
                        }
                    }
                }
            }

            try
            {
                TournamentLoginc.UpdateTournamentResults(tournament);
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            LoadMatchups((int)roundDropDown.SelectedItem);
        }
    }
}
