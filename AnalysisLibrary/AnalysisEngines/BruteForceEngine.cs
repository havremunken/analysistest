using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AnalysisLibrary.AnalysisEngines
{
    public class BruteForceEngine : BaseAnalysisEngine
    {
        public override string Name
        {
            get { return "BruteForceEngine"; }
        }

        public override void AnalyzeOutcomes(IEnumerable<AnalysisBundle> bundles)
        {
            int pos;

            foreach (var outcomeArray in GetAllCombinations())
            {
                var orderedList = new List<ResultTeam>(CalculateOutcome(outcomeArray, (from b in bundles
                                                                                       select b.Team).Distinct()));

                for (int i = 1; i <= orderedList.Count; i++)
                {
                    // TODO: Care about equal points

                    var team = orderedList[i-1];
                    var bundle = bundles.Where(t => t.Team.TeamName == team.TeamName).Where(t => t.Position == i).Single();
                    if (bundle.Outcome != PositionOutcome.Possible)
                    {
                        //Console.WriteLine("Position #{0} is now possible for {1}!", i, team.TeamName);
                        bundle.Outcome = PositionOutcome.Possible;
                    }
                }
            }
        }

        public BruteForceEngine(IEnumerable<SimpleMatch> matches)
            : base(matches)
        {

        }

        public IEnumerable<SimpleMatch[]> GetAllCombinations()
        {
            yield return _matches;

            for (long combo = 1; combo < PossibleCombinations; combo++)
            {
                RaisePosition(0);

                var copy = new SimpleMatch[_matches.Length];
                _matches.CopyTo(copy, 0);

                yield return copy;
            }
        }

        private void RaisePosition(long position)
        {
            switch (_matches[position].Result)
            {
                case MatchResult.HomeWin:
                    _matches[position].Result = MatchResult.Draw;
                    break;
                case MatchResult.Draw:
                    _matches[position].Result = MatchResult.AwayWin;
                    break;
                case MatchResult.AwayWin:
                    _matches[position].Result = MatchResult.HomeWin;
                    RaisePosition(position + 1);
                    break;
            }
        }


    }
}
