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
        protected int PointsForWin = 3;

        #endregion

        #region IAnalysisEngine Members

        public abstract string Name { get; }

        public event Action<AnalysisBundle> ResultsUpdated;

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

        #region Protected methods

        /// <summary>
        /// Updates the bundles so it accurately reflects which teams can end up in which positions
        /// </summary>
        /// <param name="orderedTeams">The ordered list of team results</param>
        /// <param name="bundles">The complete list of interesting bundles</param>
        protected void UpdateBundles(IEnumerable<ResultTeam> orderedTeams, IEnumerable<AnalysisBundle> bundles)
        {
            var orderedList = new List<ResultTeam>(orderedTeams);

            for (int i = 1; i <= orderedList.Count; i++)
            {
                var team = orderedList[i - 1];

                // Check if anyone else has the same amount of points
                var samePointsBundles = new List<AnalysisBundle>();

                for (int delta = 1; delta <= orderedList.Count; delta++)
                {
                    if (i == delta)
                        continue;

                    if (orderedList[i-1].Points == orderedList[delta-1].Points)
                    {
                        // TODO: Hvis man har samme poengsum som noen andre, oppdater alle de posisjonene og sett GoalDifference hvis det ikke allerede er noe høyere
                        var bundle = bundles.Where(t => t.Team.TeamName == team.TeamName).Where(t => t.Position == delta).Single();
                        samePointsBundles.Add(bundle);
                    }
                }
                if (samePointsBundles.Count > 0)
                {
                    // We're in goal difference territory
                    samePointsBundles.Add(bundles.Where(t => t.Team.TeamName == team.TeamName).Where(t => t.Position == i).Single());
                    foreach (var bundle in samePointsBundles)
                    {
                        if (bundle.Outcome == PositionOutcome.Unknown)
                        {
                            bundle.Outcome = PositionOutcome.PossibleByGoalDifference;
                            if (ResultsUpdated != null)
                                ResultsUpdated(bundle);
                        }
                    }
                }
                else
                {
                    var bundle = bundles.Where(t => t.Team.TeamName == team.TeamName).Where(t => t.Position == i).Single();
                    if (bundle.Outcome != PositionOutcome.Possible)
                    {
                        bundle.Outcome = PositionOutcome.Possible;
                        bundle.Status = PositionStatus.Finished;

                        if (ResultsUpdated != null)
                            ResultsUpdated(bundle);
                    }
                }
            }
        }

        /// <summary>
        /// Calculates the team points and returns an ordered list for position analysis
        /// </summary>
        /// <param name="outcome">The array of matches</param>
        /// <param name="allTeams">List of teams</param>
        /// <returns>A ranked list of teams</returns>
        protected IEnumerable<ResultTeam> CalculateOutcome(SimpleMatch[] outcome, IEnumerable<SimpleTeam> allTeams)
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

        protected IEnumerable<AnalysisBundle> FilterImpossibleBundles(IEnumerable<AnalysisBundle> bundles, IEnumerable<SimpleMatch> matchesLeft)
        {
            var teamsAndMatchesLeft = new Dictionary<SimpleTeam, int>();

            // Enumerate teams
            foreach (var match in matchesLeft)
            {
                if (teamsAndMatchesLeft.Keys.Contains(match.HomeTeam))
                    teamsAndMatchesLeft[match.HomeTeam]++;
                else
                    teamsAndMatchesLeft.Add(match.HomeTeam, 1);

                if (teamsAndMatchesLeft.Keys.Contains(match.AwayTeam))
                    teamsAndMatchesLeft[match.AwayTeam]++;
                else
                    teamsAndMatchesLeft.Add(match.AwayTeam, 1);
            }

            foreach (var bundle in bundles)
            {
                var us = bundle.Team;
                var them = (from t in teamsAndMatchesLeft.Keys
                            where t.PositionBeforeAnalysis == bundle.Position
                            select t).Single();
                if (object.ReferenceEquals(us, them))
                    continue;

                var ourPotentialPoints = (teamsAndMatchesLeft[us] * PointsForWin) + us.PointsBeforeAnalysis;
                if (ourPotentialPoints < them.PointsBeforeAnalysis)
                {
                    Console.WriteLine("{0} (max {1} pts) can not pass {2} (min {3}pts) in {4} matches", us.TeamName, ourPotentialPoints, them.TeamName, them.PointsBeforeAnalysis, teamsAndMatchesLeft[us]);
                    continue;
                }
                yield return bundle;
            }
        }

        #endregion
    }
}
