using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AnalysisLibrary
{
    public enum MatchResult
    { 
        HomeWin = 0,
        Draw = 1,
        AwayWin = 2
    }

    public struct SimpleMatch
    {
        public SimpleTeam HomeTeam { get; set; }
        public SimpleTeam AwayTeam { get; set; }
        public MatchResult Result { get; set; }
    }
}
