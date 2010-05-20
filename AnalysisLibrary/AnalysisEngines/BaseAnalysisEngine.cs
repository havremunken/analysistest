using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AnalysisLibrary.Interfaces;

namespace AnalysisLibrary.AnalysisEngines
{
    public class ResultTeam
    {
        public string TeamName { get; set; }
        public int Points { get; set; }
    }

    public abstract class BaseAnalysisEngine : IAnalysisEngine
    {
        #region Protected fields

        protected SimpleMatch[] _matches;

        #endregion

        #region IAnalysisEngine Members

        public abstract string Name { get; }

        public event Action ResultsUpdated;

        public event Action ProcessFinished;

        public abstract void AnalyzeOutcomes(IEnumerable<AnalysisBundle> bundles);

        #endregion

        #region Public properties

        public long PossibleCombinations
        {
            get
            {
                if (_matches.Length == 0)
                    throw new InvalidOperationException();

                return (long)Math.Pow(3, _matches.LongLength);
            }
        }

        #endregion

        public BaseAnalysisEngine(IEnumerable<SimpleMatch> matches)
        {
            _matches = new SimpleMatch[matches.Count()];
            int i = 0;

            foreach (var match in matches)
            {
                _matches[i] = match;
                _matches[i++].Result = MatchResult.HomeWin;
            }
        }

        #region Public methods


        public IEnumerable<ResultTeam> CalculateOutcome(SimpleMatch[] outcome, IEnumerable<SimpleTeam> allTeams)
        {
            var teams = new List<ResultTeam>();
            foreach( var team in allTeams)
            {
                teams.Add(new ResultTeam { TeamName = team.TeamName, Points = team.PointsBeforeAnalysis } );
            }

            foreach (var match in outcome)
            {
                switch (match.Result)
                { 
                    case MatchResult.HomeWin:
                        teams.Single(t => t.TeamName == match.HomeTeam.TeamName).Points += 3;
                        break;
                    case MatchResult.Draw:
                        teams.Single(t => t.TeamName == match.HomeTeam.TeamName).Points++;
                        teams.Single(t => t.TeamName == match.AwayTeam.TeamName).Points++;
                        break;
                    case MatchResult.AwayWin:
                        teams.Single(t => t.TeamName == match.AwayTeam.TeamName).Points += 3;
                        break;
                }
            }

            return teams.OrderByDescending(t => t.Points);
        }

        #endregion
    }
}
