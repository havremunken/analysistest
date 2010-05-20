using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AnalysisLibrary
{
    public class AnalysisBundle
    {
        public SimpleTeam Team { get; set; }
        public int Position { get; set; }
        public PositionOutcome Outcome { get; set; }
        public PositionStatus Status { get; set; }
    }
}
