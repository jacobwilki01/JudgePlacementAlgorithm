using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JudgePlacement.Data
{
    public class Debate
    {
        public Entry? Affirmative { get; set; }

        public Entry? Negative { get; set; }

        public List<Judge> Judges { get; set; } = new();

        public SortedDictionary<float, Judge> JudgeMutualities { get; set; } = new();

        public SortedDictionary<float, List<Judge>> PanelMutualities { get; set; } = new();

        public bool IsBye { get; set; } = false;

        public int Bracket { get; set; } = 0;

        public int Number { get; set; } = 0;

        public float CurrentMutualPref { get; set; } = 0f;

        //temp TO-DO remove
        public Judge? Previous { get; set; } = null;

        public float GetVariance()
        {
            if (IsBye)
                return 0f;

            if (Judges.Count == 1)
            {
                float affPref = 50f;
                float negPref = 50f;

                if (Affirmative!.PreferenceSheet.TryGetValue(Judges[0], out float affValue))
                    affPref = affValue;
                if (Negative!.PreferenceSheet.TryGetValue(Judges[0], out float negValue))
                    negPref = negValue;

                return Math.Abs(affPref - negPref);
            }
            else
            {
                float AffPref = 0f;
                float NegPref = 0f;

                foreach (Judge judge in Judges)
                {
                    AffPref = AffPref + Affirmative!.PreferenceSheet[judge];
                    NegPref = NegPref + Negative!.PreferenceSheet[judge];
                }

                return Math.Abs(AffPref - NegPref);
            }
        }

        public float GetPrefMaximum()
        {
            if (IsBye) 
                return 0f;

            if (Judges.Count == 1)
            {
                if (Affirmative!.PreferenceSheet.ContainsKey(Judges[0]))
                {
                    if (Negative!.PreferenceSheet.ContainsKey(Judges[0]))
                        return Math.Max(Affirmative!.PreferenceSheet[Judges[0]], Negative!.PreferenceSheet[Judges[0]]);

                    return Affirmative!.PreferenceSheet[Judges[0]];
                }
                else if (Negative!.PreferenceSheet.ContainsKey(Judges[0]))
                {
                    if (Affirmative!.PreferenceSheet.ContainsKey(Judges[0]))
                        return Math.Max(Affirmative!.PreferenceSheet[Judges[0]], Negative!.PreferenceSheet[Judges[0]]);

                    return Negative!.PreferenceSheet[Judges[0]];
                }
                else
                { 
                    return 0f; 
                }
            }
            else
            {
                float AffPref = 0f;
                float NegPref = 0f;

                foreach (Judge judge in Judges)
                {
                    AffPref = AffPref + Affirmative!.PreferenceSheet[judge];
                    NegPref = NegPref + Negative!.PreferenceSheet[judge];
                }

                return Math.Max(AffPref, NegPref);
            }
        }

        /// <summary>
        /// Returns the potential impact of a judge on a given debate.
        /// </summary>
        /// <param name="judge">The potential judge.</param>
        /// <returns>A float representing how it would impact the debate.</returns>
        public float GetPotentialImpact(Judge judge)
        {
            if (IsBye)
                return 0f;

            if (Affirmative!.PreferenceSheet.TryGetValue(judge, out float affPref) && Negative!.PreferenceSheet.TryGetValue(judge, out float negPref))
                return Math.Abs(affPref - negPref);

            return 1000f;
        }

        /// <summary>
        /// Returns the potential max pref value of a judge on a given debate.
        /// </summary>
        /// <param name="judge">The potential judge.</param>
        /// <returns>A float representing the max preference value.</returns>
        public float GetPotentialPrefMaximum(Judge judge)
        {
            if (IsBye)
                return 0f;

            if (Affirmative!.PreferenceSheet.TryGetValue(judge, out float affPref) && Negative!.PreferenceSheet.TryGetValue(judge, out float negPref))
                return Math.Max(affPref, negPref);

            return 1000f;
        }

        public string GetConsoleLine()
        {
            string judgeNames = string.Empty;
            foreach (Judge judge in Judges)
                judgeNames = judgeNames + judge.Name;

            float affCur = 0f;
            float negCur = 0f;
            float affPrev = 0f;
            float negPrev = 0f;

            if (Affirmative!.PreferenceSheet.TryGetValue(Judges[0], out _))
                affCur = Affirmative!.PreferenceSheet[Judges[0]];
            if (Negative!.PreferenceSheet.TryGetValue(Judges[0], out _))
                negCur = Negative!.PreferenceSheet[Judges[0]];
            if (Affirmative!.PreferenceSheet.TryGetValue(Previous!, out _))
                affPrev = Affirmative!.PreferenceSheet[Previous!];
            if (Negative!.PreferenceSheet.TryGetValue(Previous!, out _))
                negPrev = Negative!.PreferenceSheet[Previous!];

            string curPref = "(" + Math.Round(affCur, 2).ToString() + "-" + Math.Round(negCur, 2).ToString() + ")";
            string prevPref = "(" + Math.Round(affPrev, 2).ToString() + "-" + Math.Round(negPrev, 2).ToString() + ")";

            // Affirmative!.Code + "\tvs. " + Negative!.Code + "\t| " + 
            return Bracket.ToString() + " |" + judgeNames + curPref + " | " + Previous!.Name + prevPref;
        }
    }
}
