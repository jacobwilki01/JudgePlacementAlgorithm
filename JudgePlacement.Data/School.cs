using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JudgePlacement.Data
{
    public class School
    {
        public string Name { get; set; } = string.Empty;

        public int TabroomId { get; set; } = 0;

        public Guid Id = Guid.NewGuid();

        public List<Entry> Entries { get; set; } = new();
    }
}
