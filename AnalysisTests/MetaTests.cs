using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AnalysisLibrary;

namespace AnalysisTests
{
    [TestFixture]
    public class Tests
    {
        // Liksom eksisterende resultater:
        //
        // SIF 0-2 RBK      RBK x-x SIF
        // RBK 5-0 vif      vif x-x RBK
        // RBK 4-0 lsk      lsk 1-2 RBK
        // RBK 3-0 hbk      hbk x-x RBK
        //
        // SIF x-x vif      vif 1-2 SIF
        // SIF x-x lsk      lsk 2-3 SIF
        // SIF 3-2 hbk      hbk 1-3 SIF
        // 
        // vif 0-1 lsk      lsk 5-1 vif
        // vif 1-2 hbk      hkb 1-0 vif
        // 
        // lsk x-x hbk      hbk 1-0 lsk
        //
        SimpleTeam RBK, SIF, vif, lsk, hbk;
        SimpleMatch vifRBK, RBKSIF, SIFvif, lskhbk, SIFlsk, hbkRBK;
        List<SimpleTeam> teams;
        List<SimpleMatch> matchesLeft;

        #region Setup for analysis

        private void SetupTeamsForAnalysis()
        {
            RBK = new SimpleTeam { TeamName = "Rosenborg", MatchesLeft = 3, PointsBeforeAnalysis = 15, PositionBeforeAnalysis=1 };
            SIF = new SimpleTeam { TeamName = "Strømsgodset", MatchesLeft = 3, PointsBeforeAnalysis = 12, PositionBeforeAnalysis=2 };
            hbk = new SimpleTeam { TeamName = "Hønefoss", MatchesLeft = 2, PointsBeforeAnalysis = 9, PositionBeforeAnalysis=3 };
            lsk = new SimpleTeam { TeamName = "lillestrøm", MatchesLeft = 2, PointsBeforeAnalysis = 6, PositionBeforeAnalysis = 4 };
            vif = new SimpleTeam { TeamName = "vaalerengen", MatchesLeft = 2, PointsBeforeAnalysis = 0, PositionBeforeAnalysis = 5 };

            vifRBK = new SimpleMatch { HomeTeam = vif, AwayTeam = RBK };
            RBKSIF = new SimpleMatch { HomeTeam = RBK, AwayTeam = SIF };
            SIFvif = new SimpleMatch { HomeTeam = SIF, AwayTeam = vif };
            lskhbk = new SimpleMatch { HomeTeam = lsk, AwayTeam = hbk };
            SIFlsk = new SimpleMatch { HomeTeam = SIF, AwayTeam = lsk };
            hbkRBK = new SimpleMatch { HomeTeam = hbk, AwayTeam = RBK };

            teams = new List<SimpleTeam>
            {
                RBK,
                SIF,
                hbk,
                vif,
                lsk
            };

            matchesLeft = new List<SimpleMatch> 
            {
                vifRBK,
                RBKSIF,
                SIFvif,
                lskhbk,
                SIFlsk,
                hbkRBK
            };
        }

        #endregion

        public void SomeEvilTest()
        {
            SetupTeamsForAnalysis();

            var a = new Analyzer(teams, matchesLeft);
            a.ResultsUpdated += delegate(AnalysisBundle bundle)
            {
                Console.WriteLine("New state for team {0}, position {1}: {2}", bundle.Team.TeamName, bundle.Position, bundle.Outcome);
            };

            var res = a.PerformAnalysis();

            foreach (var team in teams)
            {
                var bundles = from b in res
                              where b.Team.TeamName == team.TeamName && (b.Outcome == PositionOutcome.Possible || b.Outcome == PositionOutcome.PossibleByGoalDifference )
                              select b;

                Console.Write("{0}: ", team.TeamName);
                foreach (var bundle in bundles)
                {
                    Console.Write("{1}{0} ", bundle.Position, bundle.Outcome == PositionOutcome.PossibleByGoalDifference ? "-" : "");
                }
                Console.WriteLine("");
            }
        }

    }
}
