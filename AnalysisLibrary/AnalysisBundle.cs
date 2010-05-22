using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AnalysisLibrary
{
    /// <summary>
    /// A class that keeps track of whether or not the given team can end up in the given position
    /// </summary>
    public class AnalysisBundle
    {
        public SimpleTeam Team { get; set; }
        public int Position { get; set; }
        public PositionOutcome Outcome { get; set; }
        public PositionStatus Status { get; set; }
    }
}
