using JudgePlacement.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JudgePlacement.Placement
{
    public static class JudgePlacement
    {
        #region Preference Weights and Maximums
        public static int BalancePreviousWeight { get; set; } = 0;

        public static int PreferenceWeight { get; set; } = 0;

        public static int MutualityWeight { get; set; } = 0;

        public static int PanelWeight { get; set; } = 0;

        public static int TotalWeight { get; set; } = 0;

        public static int MaxMutualityDifference { get; set; } = 0;

        public static int MaxPreference { get; set; } = 0;

        public static int MaxPanelDifference { get; set; } = 0;
        #endregion Preference Weights and Maximums

        public static int BreakingPoint { get; set; } = 0;

        public static void PlaceJudges(Round round, JudgePool judgePool)
        {
            CalculatePrefValues(round, judgePool);
            
            List<List<Debate>> brackets = CreateBrackets(round);

            foreach (List<Debate> bracket in brackets)
            {
                int bracketSize = 0;
            }
        }

        /// <summary>
        /// Processes through each round and judge combination, figures out the rating. 
        /// </summary>
        /// <param name="round"></param>
        /// <param name="judgePool"></param>
        private static void CalculatePrefValues(Round round, JudgePool judgePool)
        {
            foreach (Debate debate in round.Debates)
            {
                if (debate.Affirmative == null || debate.Negative == null)
                {
                    debate.IsBye = true;
                    continue;
                }

                if (round.PanelSize == 1)
                {
                    foreach (Judge judge in judgePool.Judges)
                        debate.JudgeMutualities.Add(CalculateJudgePrefValue(debate.Affirmative, debate.Negative, judge), judge);
                }
                else
                {
                    // TO-DO for loop of all judge combinations
                }
            }
        }

        /// <summary>
        /// Calculates the preference value of a judge for a single-judge debate.
        /// </summary>
        /// <param name="Aff">The affirmative team</param>
        /// <param name="Neg">The negative team</param>
        /// <param name="judge">The judge at hand</param>
        /// <returns></returns>
        private static float CalculateJudgePrefValue(Entry Aff, Entry Neg, Judge judge)
        {
            // Load judge preference value. If missing, or a conflict, set value to 100.
            float affPref = Aff.PreferenceSheet.TryGetValue(judge, out float affValue) && CheckJudgeConflict(judge, Aff) ? affValue : 100f;
            float negPref = Neg.PreferenceSheet.TryGetValue(judge, out float negValue) && CheckJudgeConflict(judge, Neg) ? negValue : 100f;

            // Calculate previous average, if relevant.
            float affPrevSum = 0;
            float negPrevSum = 0;

            foreach (Judge previousJudge in Aff.PreviousJudges)
                affPrevSum = affPrevSum + Aff.PreferenceSheet[previousJudge];
            foreach (Judge previousJudge in Neg.PreviousJudges)
                negPrevSum = negPrevSum + Neg.PreferenceSheet[previousJudge];

            affPrevSum /= Aff.PreviousJudges.Count;
            negPrevSum /= Neg.PreviousJudges.Count;

            float prevAvg = Math.Max(affPrevSum, negPrevSum) * BalancePreviousWeight;

            // Calculate Mutuality.
            float mutuality = Math.Abs(affPref - negPref);

            // Define the mutual pref value.
            float judgePrefValue = 0f;

            // If either pref is below the maximum, add 100 trillion to it.
            if (Math.Max(affPref, negPref) > MaxPreference)
                judgePrefValue = judgePrefValue + 100_000_000_000_000f;

            // If mutuality difference is too large, add another 100 trillion to it.
            if (mutuality > MaxMutualityDifference)
                judgePrefValue = judgePrefValue + 100_000_000_000_000f;

            judgePrefValue = 
                judgePrefValue 
                + (affPref * PreferenceWeight) 
                + (negPref * PreferenceWeight) 
                + (mutuality * MutualityWeight) 
                + affPref 
                + negPref 
                - prevAvg;

            return judgePrefValue;
        }

        /// <summary>
        /// Calculates the preference value of a given judge panel for a multi-judge debate.
        /// </summary>
        /// <param name="Aff">The affirmative team</param>
        /// <param name="Neg">The negative team</param>
        /// <param name="panel">The panel of judges</param>
        /// <returns></returns>
        private static float CalculatePanelPrefValue(Entry Aff, Entry Neg, List<Judge> panel)
        {
            //TO-DO

            return 100_000_000_000_000f;
        }

        /// <summary>
        /// Checks if a judge conflict exists for a given entry.
        /// Intended to be expanded if any more features need to be added.
        /// </summary>
        /// <param name="judge">The judge at hand.</param>
        /// <param name="entry">The entry at had.</param>
        /// <returns></returns>
        private static bool CheckJudgeConflict(Judge judge, Entry entry)
        {
            return entry.School!.Equals(judge.School) || entry.PreviousJudges.Contains(judge);
        }

        /// <summary>
        /// Breaks down a given deabte round's debates into their respective brackets.
        /// Organizes the brackets for "importance", which means break rounds are first, then one-down, etc.
        /// </summary>
        /// <param name="round">The given debate round</param>
        /// <returns>A list of brackets, organized by bracket in order of importance.</returns>
        private static List<List<Debate>> CreateBrackets(Round round)
        {
            List<List<Debate>> brackets = new();

            // If HighLow or HighHigh, organize automatically
            if (round.Type != RoundTypeEnum.Preset || round.Type != RoundTypeEnum.Elim)
            {
                for (int i = 0; i < round.RoundNum; i++)
                    brackets.Add(new List<Debate>());

                int breakRoundBracket = round.RoundNum - 1 - BreakingPoint;

                // Organize debates into brackets
                foreach (Debate debate in round.Debates)
                {
                    /* 
                    The following code "organizes" the brackets of a given debate.
                    It does this by calculating the "distance from the breaking point" and inserting it at the relevant list.
                    The distance from the breaking point is the debate's bracket minus the maximum number of losses a team can have and still clear.
                    In 6 and 7 round tournaments, its usually 2 (or 3, depending on number of elims). In 8 round tournaments, its usually 3 (or 4, depending on number of elims)

                     */
                    int distanceFromBreak = debate.Bracket - breakRoundBracket;

                    if (distanceFromBreak > -1)
                        brackets[distanceFromBreak].Add(debate);
                    else
                        brackets[BreakingPoint + Math.Abs(distanceFromBreak)].Add(debate);
                }
            }

            // If Preset or Elim, all debates are treated the same, so just create a single "bracket".
            else
            {
                brackets.Add(new List<Debate>(round.Debates));
            }

            return brackets;
        }
    }
}
