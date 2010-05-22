using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AnalysisLibrary.AnalysisEngines;

namespace AnalysisLibrary
{
    public class Analyzer
    {
        private List<SimpleTeam> _teams;
        private List<SimpleMatch> _matches;
        private List<AnalysisBundle> _bundles;

        public event Action<AnalysisBundle> ResultsUpdated;

        public Analyzer(IEnumerable<SimpleTeam> teams, IEnumerable<SimpleMatch> remainingMatches)
        {
            _teams = new List<SimpleTeam>(teams);
            _matches = new List<SimpleMatch>(remainingMatches);

            CreateBundles();
        }

        public IEnumerable<AnalysisBundle> PerformAnalysis()
        {
            var analyzer = new ParallelBruteForceEngine(_matches);

            analyzer.ResultsUpdated += new Action<AnalysisBundle>(analyzer_ResultsUpdated);

            analyzer.AnalyzeOutcomes(_bundles);

            return _bundles;
        }

        void analyzer_ResultsUpdated(AnalysisBundle bundle)
        {
            if (ResultsUpdated != null)
                ResultsUpdated(bundle);
        }

        private void CreateBundles()
        {
            _bundles = new List<AnalysisBundle>();

            foreach (var team in _teams)
            {
                for (int i = 1; i <= _teams.Count; i++)
                {
                    _bundles.Add(new AnalysisBundle { Team = team, Position = i, Status = PositionStatus.NotStarted, Outcome = PositionOutcome.Unknown });
                }
            }
        }
    }
}
