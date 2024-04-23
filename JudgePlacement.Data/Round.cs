using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JudgePlacement.Data
{
    public class Round
    {
        public string Name { get; set; } = string.Empty;

        public RoundTypeEnum Type { get; set; } = RoundTypeEnum.None;

        public int PanelSize { get; set; } = 1;

        public int RoundNum { get; set; } = 0;

        public Guid Guid = Guid.NewGuid();

        public Timeslot? Timeslot { get; set; }

        public List<Debate> Debates { get; set; } = new();

        public Event? Event { get; set; }

        public void EraseJudges()
        {
            foreach (Debate debate in Debates)
            {
                if (debate.IsBye)
                    continue;

                foreach (Judge judge in debate.Judges)
                {
                    judge.CurrentlyPlaced = false;
                    judge.RoundsJudged--;
                    debate.Previous = debate.Judges[0]; // TO-DO remove
                }

                if (debate.Affirmative != null && debate.Affirmative.WinLossMap[RoundNum])
                    debate.Affirmative.Wins--;
                else if (debate.Negative != null && debate.Negative.WinLossMap[RoundNum])
                    debate.Negative.Wins--;

                debate.CurrentMutualPref = 0f;
                debate.Judges.Clear();
            }
        }
    }
}
