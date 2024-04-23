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

        public bool CurrentlyPlaced { get; set; } = false;

        public List<Event> EventStrikes { get; set; } = new();

        public List<Entry> EntryStrikes { get; set; } = new();

        public List<School> SchoolStrikes { get; set; } = new();

        public School? School { get; set; }
    }
}
