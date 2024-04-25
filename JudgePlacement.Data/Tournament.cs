using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JudgePlacement.Data
{
    public class Tournament
    {
        public string Name { get; set; } = string.Empty;

        public string Year { get; set; } = string.Empty;

        public List<School> Schools { get; set; } = new();

        public List<JudgeCategory> JudgeCategories { get; set; } = new();

        public List<Event> Events { get; set; } = new();

        public List<Timeslot> Timeslots { get; set; } = new();

        public dynamic? RawJsonTournament { get; set; }

        #region Maps for Tabroom IDs to C# Objects

        public Dictionary<int, JudgeCategory> CategoryMap { get; set; } = new();

        public Dictionary<int, Entry> EntryMap { get; set; } = new();

        public Dictionary<int, Event> EventMap { get; set; } = new();

        public Dictionary<long, Judge> JudgeMap { get; set; } = new();

        public Dictionary<int, JudgePool> PoolMap { get; set; } = new();

        public Dictionary<int, School> SchoolMap { get; set; } = new();

        public Dictionary<int, Timeslot> TimeslotMap { get; set; } = new();

        #endregion Maps for Tabroom IDs to C# Objects

        public Tournament() 
        {
            
        }
    }
}
