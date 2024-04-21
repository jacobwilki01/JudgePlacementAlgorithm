using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JudgePlacement.Data
{
    public class Judge
    {
        public string Name { get; set; } = string.Empty;

        public long TabroomId { get; set; } = 0;

        public int Obligation { get; set; } = 0;

        public int RoundsJudged { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public School? School { get; set; }
    }
}
