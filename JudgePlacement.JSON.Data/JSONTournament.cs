using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
 All of the classes in this file make up shell classes used for JSON data scrapping.
 None of the classes in this file are used for actual data storage. They are converted upon download into usable classes.
 All of the classes in this file reflect a truncated version of Tabroom.com's JSON output for Complete Tournament Data.
 */
namespace JudgePlacement.JSON.Data
{
    /// <summary>
    /// Class for a tournament from JSON data.
    /// </summary>
    public class JSONTournament
    {
        public string? name { get; set; }

        public List<JSONSchool> schools { get; set; } = new();

        public List<JSONCategory> categories { get; set; } = new();

        public List<JSONTimeslot> timeslots { get; set; } = new();
    }

    /// <summary>
    /// Class for a school from JSON data.
    /// </summary>
    public class JSONSchool
    {
        public string? name { get; set; }

        public string? id { get; set; }

        public List<JSONEntry> entries { get; set; } = new();
    }

    /// <summary>
    /// Class for a entry from JSON data.
    /// </summary>
    public class JSONEntry
    {
        public string? code { get; set; }

        public string? id { get; set; }

        public string? @event { get; set; }

        public int? active { get; set; }

        public int? waitlist { get; set; }
    }

    /// <summary>
    /// Class for a category from JSON data.
    /// </summary>
    public class JSONCategory
    {
        public string? name { get; set; }

        public string? id { get; set; }

        public List<JSONEvent> events { get; set; } = new();

        public List<JSONJudgePool> judge_pools { get; set; } = new();
        
        public List<JSONJudge> judges { get; set; } = new();
    }

    /// <summary>
    /// Class for a event from JSON data.
    /// </summary>
    public class JSONEvent
    {
        public string? id { get; set; }

        public string? name { get; set; }

        public string? abbr { get; set; }

        public List<JSONRound> rounds { get; set; } = new();
    }

    /// <summary>
    /// Class for a round from JSON data.
    /// </summary>
    public class JSONRound
    {
        public int? name { get; set; }

        public int? timeslot { get; set; }

        public string? type { get; set; }

        public List<JSONSection> sections { get; set; } = new();
    }

    /// <summary>
    /// Class for a section from JSON data.
    /// </summary>
    public class JSONSection
    {
        public string? letter {  get; set; }

        public List<JSONBallot> ballots { get; set; } = new();
    }

    /// <summary>
    /// Class for a ballot from JSON data.
    /// </summary>
    public class JSONBallot
    {
        public int? entry { get; set; }

        public int? side { get; set; }

        public long? judge { get; set; }

        public List<JSONScore> scores {  get; set; } = new();
    }

    /// <summary>
    /// Class for a score from JSON data.
    /// </summary>
    public class JSONScore
    {
        public string? tag { get; set; }

        public float? value { get; set; }
    }

    /// <summary>
    /// Class for a judge pool from JSON data.
    /// </summary>
    public class JSONJudgePool
    {
        public string? id { get; set; }

        public string? name { get; set; }

        public List<int> judges { get; set; } = new();
    }

    /// <summary>
    /// Class for a judge from JSON data.
    /// </summary>
    public class JSONJudge
    {
        public string? first { get; set; }

        public string? last { get; set; }

        public string? id { get; set; }

        public string? school { get; set; }

        public int? obligation { get; set; }

        public List<JSONRating> ratings { get; set; } = new();
    }

    /// <summary>
    /// Class for a judge rating from JSON data.
    /// </summary>
    public class JSONRating
    {
        public long? id { get; set; }

        public int? ordinal { get; set; }

        public string? percentile {  get; set; }

        public string? entry { get; set; }
    }

    /// <summary>
    /// Class for a timeslot from JSON data.
    /// </summary>
    public class JSONTimeslot
    {
        public string? id { get; set; }

        public string? name { get; set; }
    }
}
