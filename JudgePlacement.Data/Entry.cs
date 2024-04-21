using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JudgePlacement.Data
{
    public class Entry
    {
        public string Code { get; set; } = string.Empty;

        public int TabroomId { get; set; } = 0;

        public int EventId { get; set; } = 0;

        public int Wins { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public School? School { get; set; }

        public Dictionary<Judge, float> PreferenceSheet { get; set; } = new();

        public List<Judge> PreviousJudges { get; set; } = new();

        public Guid Guid = Guid.NewGuid();
    }
}
