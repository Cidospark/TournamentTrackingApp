﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary.Models
{
    public class TournamentModel
    {
        public event EventHandler<DateTime> OnTournamentComplete;
        public int Id { get; set; }
        public string TournamentName { get; set; }
        public decimal EntryFee { get; set; }
        public List<TeamModel> EnteredTeams { get; set; } = new List<TeamModel>();
        public List<PrizeModel> Prizes { get; set; } = new List<PrizeModel>();
        public List<List<MatchupModel>> Round { get; set; } = new List<List<MatchupModel>>();

        internal int CheckCurrentRound()
        {
            throw new NotImplementedException();
        }

        internal void AlertUsersToNewRound()
        {
            throw new NotImplementedException();
        }

        public void CompletTournament()
        {
            OnTournamentComplete?.Invoke(this, DateTime.Now);
        }

    }
}
