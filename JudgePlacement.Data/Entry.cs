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

        public Dictionary<Judge, Tuple<int, float>> PreferenceSheet { get; set; } = new();

        public Guid Guid = Guid.NewGuid();
    }
}
