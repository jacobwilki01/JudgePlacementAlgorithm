﻿using System;
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
    }
}
