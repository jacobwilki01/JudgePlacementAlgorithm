using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JudgePlacement.Data
{
    public class JudgeCategory
    {
        public string Name { get; set; } = string.Empty;

        public int TabroomId { get; set; } = 0;

        public Guid Guid = Guid.NewGuid();

        public List<Judge> Judges { get; set; } = new();

        public List<JudgePool> JudgePools { get; set; } = new();
    }
}
