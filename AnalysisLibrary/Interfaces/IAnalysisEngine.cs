using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AnalysisLibrary.Interfaces
{
    public interface IAnalysisEngine
    {
        string Name { get; }

        event Action ResultsUpdated;

        event Action ProcessFinished;

        void AnalyzeOutcomes(IEnumerable<AnalysisBundle> bundles);
    }
}
