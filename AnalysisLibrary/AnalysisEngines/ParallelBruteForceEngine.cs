using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalysisLibrary.AnalysisEngines
{
    /// <summary>
    /// Goal: A class that divides the brute force work into .Net 4.0 parallel tasks
    /// I am aware that this will not really help reduce the combinations in any
    /// meaningful way. However, it is an interesting technique and could be useful
    /// in combination with other methods.
    /// </summary>
    public class ParallelBruteForceEngine : BruteForceEngine
    {
        private List<SimpleMatch[]> _cache;

        public override string Name
        {
            get
            {
                return "ParallelBruteForceEngine";
            }
        }

        public ParallelBruteForceEngine(IEnumerable<SimpleMatch> matches)
            : base(matches)
        {

        }

        /// <summary>
        /// Probably a horribly expensive way to get a subset of all combinations.
        /// There should be caching.
        /// Also, I am unsure if the LINQ Skip() and Take() extension methods are good enough, given that they take
        /// int arguments. We'll see if that becomes a limitation.
        /// </summary>
        /// <param name="rangeStart">Where in the 0-based list of combinations to start fetching</param>
        /// <param name="rangeSize">The size of the range</param>
        /// <returns>A stream of combination arrays</returns>
        public IEnumerable<SimpleMatch[]> GetCombinationRange(long rangeStart, long rangeSize)
        {
            if (_cache == null)
                _cache = new List<SimpleMatch[]>(GetAllCombinations());

            return _cache.Skip((int)rangeStart).Take((int)rangeSize);
        }

        public override void AnalyzeOutcomes(IEnumerable<AnalysisBundle> bundles)
        {
            // 1. Should probably be a parameter somehow
            // 2. Should be a few orders of magnitude larger
            long chunkSize = 100;
            long offset;
            List<Task> tasks = new List<Task>();

            for (offset = 0; offset < PossibleCombinations; offset += chunkSize)
            {
                var chunk = GetCombinationRange(offset, chunkSize);

                var newTask = Task.Factory.StartNew(delegate
                {
                    foreach (var outcomeArray in chunk)
                    {
                        var orderedList = new List<ResultTeam>(CalculateOutcome(outcomeArray, (from b in bundles
                                                                                               select b.Team).Distinct()));

                        for (int i = 1; i <= orderedList.Count; i++)
                        {
                            // TODO: Care about equal points

                            var team = orderedList[i - 1];
                            var bundle = bundles.Where(t => t.Team.TeamName == team.TeamName).Where(t => t.Position == i).Single();
                            if (bundle.Outcome != PositionOutcome.Possible)
                            {
                                Console.WriteLine("Position #{0} is now possible for {1}!", i, team.TeamName);
                                bundle.Outcome = PositionOutcome.Possible;
                            }
                        }
                    }

                });
                tasks.Add(newTask);
            }
            foreach (var task in tasks)
            {
                task.Wait();
            }
        }
    }
}
