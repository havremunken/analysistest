using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AnalysisLibrary
{
    public enum PositionStatus
    {
        NotStarted,
        Started,
        Finished
    }

    public enum PositionOutcome
    {
        Unknown,
        PossibleByGoalDifference,
        Possible,
        Impossible
    }
}
