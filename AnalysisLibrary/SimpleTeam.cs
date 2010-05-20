using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AnalysisLibrary
{
    public struct SimpleTeam
    {
        public string TeamName { get; set; }
        public int PointsBeforeAnalysis { get; set; }
        public int PositionBeforeAnalysis { get; set; }
        public int MatchesLeft { get; set; }
    }
}
