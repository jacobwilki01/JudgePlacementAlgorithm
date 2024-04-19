using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JudgePlacement.Data
{
    public class Event
    {
        public string Name { get; set; } = string.Empty;

        public string Abbreviation { get; set; } = string.Empty;

        public int TabroomId { get; set; } = 0;

        public Guid Guid = Guid.NewGuid();

        public JudgeCategory? Category { get; set; }

        public List<Entry> Entries { get; set; } = new();

        public List<Round> Rounds {  get; set; } = new();
    }
}
