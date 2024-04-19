using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JudgePlacement.Data
{
    public class Timeslot
    {
        public string Name { get; set; } = string.Empty;

        public int TabroomId { get; set; } = 0;

        public Guid Guid = Guid.NewGuid();
    }
}
