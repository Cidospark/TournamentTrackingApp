using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary.Models
{
    public class MatchupEntryModel
    {
        // Represemts one team in the matchup
        public TeamModel TeamCompeting { get; set; }

        // The score for this particular team
        public double Score { get; set; }

        // The matchup that this team came from as the winner
        public MatchupModel ParentMatchup { get; set; }
    }
}
