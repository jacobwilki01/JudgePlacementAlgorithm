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
        public static int BalancePreviousWeight { get; set; } = 5;

        public static int PreferenceWeight { get; set; } = 20;

        public static int MutualityWeight { get; set; } = 40;

        public static int PanelWeight { get; set; } = 20;

        public static int TotalWeight { get; set; } = 0;

        public static int MaxMutualityDifferenceIn { get; set; } = 20;

        public static int MaxMutualityDifferenceOut { get; set; } = 35;

        public static int MaxPreferenceIn { get; set; } = 50;

        public static int MaxPreferenceOut { get; set; } = 80;

        public static int MaxPanelDifference { get; set; } = 0;
        #endregion Preference Weights and Maximums

        public static int BreakingPoint { get; set; } = 2;

        public static bool Backtrack { get; set; } = true;

        public static void PlaceJudges(Round round, JudgePool judgePool)
        {
            // Reset availability for each judge.
            foreach (Judge judge in judgePool.Judges)
                judge.CurrentlyPlaced = false;

            // Calculate the pref value for each judge/panel and round combination.
            CalculatePrefValues(round, judgePool);
            
            // Create the brackets.
            List<List<Debate>> brackets = CreateBrackets(round);

            // First pass at judge assignment (NO backtracking)
            FirstPassAssignment(brackets, round.Event!, round.PanelSize);

            // Backtracking
            if (Backtrack)
            {
                for (int i = 0; i < 1_000_000; i++)
                {
                    if (i % 1000 == 0)
                        Console.WriteLine(i.ToString());

                    if (BacktrackingSingleJudge(round))
                        break;
                }
            }

            // Re-Integrate Excluded Judges
            foreach (Judge judge in judgePool.Judges)
            {
                if (judge.CurrentlyPlaced && !IsJudgeJudging(round, judge))
                    judge.CurrentlyPlaced = false;
            }

            for (int i = 0; i < 1000; i++)
                ReIncludeJudges(round, judgePool);

            foreach (Judge judge in judgePool.Judges)
            {
                if (judge.CurrentlyPlaced)
                    judge.RoundsJudged++;
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
                    {
                        float prefValue = CalculateJudgePrefValue(round, debate, judge);

                        while (debate.JudgeMutualities.TryGetValue(prefValue, out _))
                            prefValue = prefValue + 0.01f;

                        debate.JudgeMutualities.Add(prefValue, judge);
                    }
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
        private static float CalculateJudgePrefValue(Round round, Debate debate, Judge judge)
        {
            Entry Aff = debate.Affirmative!;
            Entry Neg = debate.Negative!;

            // Load judge preference value. If missing, or a conflict, set value to 100.
            float affPref = Aff.PreferenceSheet.TryGetValue(judge, out float affValue) && CheckJudgeConflict(judge, Aff) ? affValue : 100f;
            float negPref = Neg.PreferenceSheet.TryGetValue(judge, out float negValue) && CheckJudgeConflict(judge, Neg) ? negValue : 100f;

            // Calculate previous average, if relevant.
            float affPrevSum = 0f;
            float negPrevSum = 0f;

            foreach (Judge previousJudge in Aff.PreviousJudges)
            {
                if (Aff.PreferenceSheet.ContainsKey(previousJudge))
                    affPrevSum = affPrevSum + Aff.PreferenceSheet[previousJudge];
            }
            foreach (Judge previousJudge in Neg.PreviousJudges)
            {
                if (Neg.PreferenceSheet.ContainsKey(previousJudge))
                    negPrevSum = negPrevSum + Neg.PreferenceSheet[previousJudge];
            }

            affPrevSum /= Aff.PreviousJudges.Count;
            negPrevSum /= Neg.PreviousJudges.Count;

            float prevAvg = Math.Max(affPrevSum, negPrevSum) * BalancePreviousWeight;

            // Calculate Mutuality.
            float mutuality = Math.Abs(affPref - negPref);

            // Define the mutual pref value.
            float judgePrefValue = 0f;

            // Get Max Preference and Max Mutuality
            int maxPref = (debate.Bracket >= round.RoundNum - BreakingPoint - 1) ? MaxPreferenceIn : MaxPreferenceOut;
            int maxMutuality = (debate.Bracket >= round.RoundNum - BreakingPoint - 1) ? MaxMutualityDifferenceIn : MaxMutualityDifferenceOut;

            // Prevents draw of unwanted judge unless in an extreme circumstance.
            if (Math.Max(affPref, negPref) > maxPref || mutuality > maxMutuality)
                judgePrefValue = judgePrefValue + 100_000f;

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
        /// Checks if the judge is "struck" from seeing a debate using Tabroom's "strike" system.
        /// </summary>
        /// <param name="event">The event at hand.</param>
        /// <param name="debate">The debate at hand.</param>
        /// <param name="judge">The judge at hand.</param>
        /// <returns>True if struck, false otherwise.</returns>
        private static bool IsJudgeStruck(Event @event, Debate debate, Judge judge)
        {
            return judge.EventStrikes.Contains(@event)
                || judge.EntryStrikes.Contains(debate.Affirmative!)
                || judge.EntryStrikes.Contains(debate.Negative!)
                || judge.SchoolStrikes.Contains(debate.Affirmative!.School!)
                || judge.SchoolStrikes.Contains(debate.Negative!.School!);
        }

        /// <summary>
        /// Checks if a judge is obligated for a given debate.
        /// </summary>
        /// <param name="judge">The judge at hand.</param>
        /// <returns>True if obligated, false if not.</returns>
        private static bool IsJudgeObligated(Judge judge)
        {
            return judge.Obligation - judge.RoundsJudged > 0;
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

                foreach (Debate debate in round.Debates)
                {
                    /* 
                    The following code "organizes" the brackets of a given debate.
                    It does this by calculating the "distance from the breaking point" and inserting it at the relevant list.
                    The distance from the breaking point is the debate's bracket minus the maximum number of losses a team can have and still clear.
                    This distance is positive if the team is still eligible to break, and negative if not.

                    EXAMPLE:
                    If it is round 8 of an 8 round tournament, you have the following:
                    | Bracket | Distance from Break Point |
                        7-0                 3
                        6-1                 2
                        5-2                 1
                        4-3                 0
                        3-4                 -1
                        2-5                 -2
                        1-6                 -3
                        0-7                 -4

                    The brackets list is organized in the following order: 4-3, 5-2, 6-1, 7-0, 3-4, 2-5, 1-6, 0-7.

                    This organization comes from using the distance in a certain way to append to a certain list.
                        If the value is positive, it can be directly used and insert the debate into the n-th list. 
                        If it is negative, we need to insert the debate into the (Breaking Point - |Distance|)-th list.
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

        /// <summary>
        /// Does first pass judge assignment, without backtracking or any guarantee of "optimal" or "correct" placement.
        /// </summary>=
        /// <param name="brackets">The brackets to process over</param>
        /// <param name="panelSize">The size of the judge panel</param>
        private static void FirstPassAssignment(List<List<Debate>> brackets, Event @event, int panelSize)
        {
            foreach (List<Debate> bracket in brackets)
            {
                foreach (Debate debate in bracket)
                {
                    // TO-DO Implement updating evaluation value for extra Gary things (lines 630-670 in his file).

                    if (panelSize == 1)
                    {
                        Judge? judge = debate.JudgeMutualities.First().Value;
                        float currentMutualPref = debate.JudgeMutualities.First().Key;


                        foreach (KeyValuePair<float, Judge> pair in debate.JudgeMutualities)
                        {
                            // Skip the first, so that the "pair" is always one ahead of "judge"
                            if (pair.Equals(debate.JudgeMutualities.First()))
                                continue;

                            // if judge is used, assign next. Else, break from loop.
                            if (judge.CurrentlyPlaced || !judge.IsActive || IsJudgeStruck(@event, debate, judge) || !IsJudgeObligated(judge))
                            {
                                judge = pair.Value;
                                currentMutualPref = pair.Key;
                            }
                            else
                                break;
                        }

                        // If the final judge attempt is in use, then we have ran out of eligible judges.
                        if (judge.CurrentlyPlaced || !judge.IsActive)
                        {
                            judge = null;
                            continue; // TO-DO, handle lack of judge placement!
                        }

                        judge.CurrentlyPlaced = true;
                        debate.Judges.Add(judge);
                        debate.CurrentMutualPref = currentMutualPref;
                    }
                    else
                    {
                        List<Judge>? panel = debate.PanelMutualities.First().Value;
                        float currentMutualPref = debate.PanelMutualities.First().Key;

                        foreach (KeyValuePair<float, List<Judge>> pair in debate.PanelMutualities)
                        {
                            // Skip the first, so that the "pair" is always one ahead of "panel".
                            if (pair.Equals(debate.PanelMutualities.First()))
                                continue;

                            // If any judge is in use, assign next. Else, break from loop.
                            if (!CheckPanelAvailability(panel))
                            {
                                panel = pair.Value;
                                currentMutualPref = pair.Key;
                            }
                            else
                                break;
                        }

                        if (!CheckPanelAvailability(panel))
                        {
                            panel = null;
                            continue; // TO-DO, handle lack of judge placement.
                        }

                        foreach (Judge judge in panel)
                            debate.Judges.Add(judge);
                        foreach (Judge judge in panel)
                            judge.CurrentlyPlaced = true;
                        debate.CurrentMutualPref = currentMutualPref;
                    }
                }
            }
        }

        /// <summary>
        /// Checks if a panel of judges are available or not.
        /// </summary>
        /// <param name="panel">The panel being checked.</param>
        /// <returns>True if all judges are available. False if any judge is not.</returns>
        private static bool CheckPanelAvailability(List<Judge> panel)
        {
            foreach (Judge judge in panel)
            {
                if (judge.CurrentlyPlaced || !judge.IsActive)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Method for backtrack replacements of judges in a given single-judge pairing.
        /// </summary>
        /// <param name="round">The round</param>
        /// <returns>True if no changes were made, False otherwise.</returns>
        private static bool BacktrackingSingleJudge(Round round)
        {
            int numChanges = 0;
            
            foreach (Debate debate1 in round.Debates)
            {
                Judge judge1 = debate1.Judges[0];

                foreach (Debate debate2 in round.Debates)
                {
                    if (debate2.Equals(debate1))
                        continue;

                    Judge judge2 = debate2.Judges[0];

                    int maxPreference1 = (debate1.Bracket >= round.RoundNum - BreakingPoint - 1) ? MaxPreferenceIn : MaxPreferenceOut;
                    int maxPreference2 = (debate2.Bracket >= round.RoundNum - BreakingPoint - 1) ? MaxPreferenceIn : MaxPreferenceOut;

                    if (IsJudgeStruck(round.Event!, debate1, judge2) && IsJudgeStruck(round.Event!, debate2, judge1))
                        continue;

                    // Evaluate cross-bracket permutations
                    if (debate1.Bracket == debate2.Bracket || (debate1.GetPotentialPrefMaximum(judge2) <= maxPreference1 && debate2.GetPotentialPrefMaximum(judge1) <= maxPreference2) )
                    {
                        // Gets the impact of a judge on a debate
                        float impact1On1 = debate1.GetPotentialImpact(judge1);
                        float impact1On2 = debate2.GetPotentialImpact(judge1);
                        float impact2On1 = debate1.GetPotentialImpact(judge2);
                        float impact2On2 = debate2.GetPotentialImpact(judge2);

                        float tempVariance1 = Math.Abs(debate1.GetVariance() - impact1On1 + impact2On1);
                        float tempVariance2 = Math.Abs(debate2.GetVariance() - impact2On2 + impact1On2);

                        int maxMutuality1 = (debate1.Bracket >= round.RoundNum - BreakingPoint - 1) ? MaxMutualityDifferenceIn : MaxMutualityDifferenceOut;
                        int maxMutuality2 = (debate2.Bracket >= round.RoundNum - BreakingPoint - 1) ? MaxMutualityDifferenceIn : MaxMutualityDifferenceOut;

                        if (tempVariance1 <= maxMutuality1 && tempVariance2 <= maxMutuality2 && impact2On1 <= maxMutuality1 && impact1On2 <= maxMutuality2)
                        {
                            float currentEval = debate1.CurrentMutualPref + debate2.CurrentMutualPref + (debate1.GetVariance() * PanelWeight) + (debate2.GetVariance() * PanelWeight);
                            float flipEval = CalculateJudgePrefValue(round, debate1, judge2) + CalculateJudgePrefValue(round, debate2, judge1) + (debate1.GetVariance() * PanelWeight) + (debate2.GetVariance() * PanelWeight);

                            if (flipEval < currentEval)
                            {
                                // Swap assignments
                                debate1.Judges.Add(judge2);
                                debate2.Judges.Add(judge1);
                                debate1.Judges.Remove(judge1);
                                debate2.Judges.Remove(judge2);

                                // Swap internal reference
                                judge1 = judge2;

                                numChanges++;
                            }
                        }
                    }
                }
            }

            return numChanges == 0;
        }


        private static bool ReIncludeJudges(Round round, JudgePool judgePool)
        {
            int numChanges = 0;

            foreach (Judge newJudge in judgePool.Judges) 
            { 
                Debate? bestDebate = null;

                if (IsJudgeObligated(newJudge) && !newJudge.CurrentlyPlaced)
                {
                    float flipEval = float.MaxValue;

                    foreach (Debate debate in round.Debates)
                    {
                        Judge curJudge = debate.Judges[0]; // TO-DO modify for panels

                        int maxPreference = (debate.Bracket >= round.RoundNum - BreakingPoint - 1) ? MaxPreferenceIn : MaxPreferenceOut;
                        int maxMutuality = (debate.Bracket >= round.RoundNum - BreakingPoint - 1) ? MaxMutualityDifferenceIn : MaxMutualityDifferenceOut;
                        float variance = Math.Abs(debate.GetVariance() - debate.GetPotentialImpact(curJudge) + debate.GetPotentialImpact(newJudge));

                        if (debate.GetPotentialPrefMaximum(newJudge) <= maxPreference && debate.GetPotentialImpact(newJudge) <= maxMutuality && variance <= maxMutuality)
                        {
                            float curEval = debate.CurrentMutualPref + (variance * PanelWeight);
                            float newEval = CalculateJudgePrefValue(round, debate, newJudge) + (variance * PanelWeight);

                            if (newEval < flipEval && newEval < curEval)
                            {
                                flipEval = newEval;
                                bestDebate = debate;
                            }
                        }
                    }

                    if (flipEval < float.MaxValue)
                    {
                        Judge curJudge = bestDebate!.Judges[0]; // TO-DO modify for panels
                        bestDebate!.Judges.Add(newJudge);
                        bestDebate!.Judges.Remove(curJudge);

                        curJudge.CurrentlyPlaced = false;
                        newJudge.CurrentlyPlaced = true;

                        numChanges++;
                    }
                }
            }
            return numChanges == 0;
        }


        private static bool IsJudgeJudging(Round round, Judge judge)
        {
            foreach (Debate debate in round.Debates)
            {
                if (debate.Judges.Contains(judge))
                    return true;
            }

            return false;
        }
    }
}
