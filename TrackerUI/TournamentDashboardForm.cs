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
    public partial class TournamentDashboardForm : Form
    {
        List<TournamentModel> tournaments = new List<TournamentModel>();
        public TournamentDashboardForm()
        {
            InitializeComponent();
            LoadListData();
            WireUpLists();
        }

        private void LoadListData()
        {
            //try
            //{
                if (GlobalConfig.Connections.Count > 1)
                {
                    tournaments = new SQLConnector().GetAllTournaments();
                }
                else
                {
                    if (GlobalConfig.Connections[0].ToString() == "TrackerLibrary.DataAccess.TextConnector")
                        tournaments = new TextConnector().GetAllTournaments();
                    else
                        tournaments = new SQLConnector().GetAllTournaments();
                }

            //}
            //catch (Exception ex) { MessageBox.Show($"Error loading into listbox, {ex.Message}", "Error Message"); }

        }

        private void WireUpLists()
        {
            loadExistingTournamentDropDown.DataSource = tournaments;
            loadExistingTournamentDropDown.DisplayMember = "TournamentName";
        }

        private void createTournamentButton_Click(object sender, EventArgs e)
        {
            CreateTournamentForm frm = new CreateTournamentForm();
            frm.Show();
        }

        private void loadTournamentButton_Click(object sender, EventArgs e)
        {
            TournamentModel tm = (TournamentModel)loadExistingTournamentDropDown.SelectedItem;
            TournamentViewerForm frm = new TournamentViewerForm(tm);
            frm.Show();
        }
    }
}
