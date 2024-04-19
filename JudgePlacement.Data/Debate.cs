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

        public int Bracket { get; set; } = 0;

        public int Number { get; set; } = 0;
    }
}
