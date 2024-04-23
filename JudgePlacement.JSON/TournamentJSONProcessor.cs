using JudgePlacement.Data;
using JudgePlacement.JSON.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Runtime;

namespace JudgePlacement.JSON
{
    public static class TournamentJSONProcessor
    {
        public static TabroomHTTPClient TabroomHTTPClient { get; set; } = new();

        public static string DownloadTournamentData(string tournId)
        {
            if (!TabroomHTTPClient.HasLoggedIn)
            {
                // handle some exception code. TO-DO later
            }

            return TabroomHTTPClient.TournamentDataToString(tournId);
        }

        public static Tournament CreateNewTournament(string jsonString)
        {
            JSONTournament jsonTourn = JsonSerializer.Deserialize<JSONTournament>(jsonString)!;

            Tournament tournament = new Tournament()
            {
                Name = jsonTourn.name!
            };

            // Creates the "events", "judge categories", and "timeslots".
            CreateBasics(tournament, jsonTourn); 

            // Creates the "schools" and "entries"
            CreateSchools(tournament, jsonTourn);

            // Creates the "judges" and maps their preference ratings to each entry's pref sheet, and the "judge pools".
            CreateJudges(tournament, jsonTourn);

            // Creates the "rounds" that are currently active on Tabroom.com.
            CreateRounds(tournament, jsonTourn);

            return tournament;
        }

        public static void UpdateTournament(Tournament tournament, string jsonTournament)
        {
            // TO-DO Update values from "Create" using new download. It might be easier to just store new tournaments each time, I haven't decided.
        }

        private static void CreateBasics(Tournament tournament, JSONTournament jsonTournament)
        {
            foreach (JSONCategory jsonCategory in jsonTournament.categories)
            {
                // Create the Judge Category
                JudgeCategory judgeCategory = new JudgeCategory() 
                { 
                    Name = jsonCategory.name!,
                    TabroomId = int.Parse(jsonCategory.id!)
                };

                // Create the events
                foreach (JSONEvent jsonEvent in jsonCategory.events)
                {
                    Event @event = new Event()
                    {
                        Name = jsonEvent.name!,
                        Abbreviation = jsonEvent.abbr!,
                        TabroomId = int.Parse(jsonEvent.id!),
                        Category = judgeCategory
                    };

                    tournament.Events.Add(@event);
                    tournament.EventMap.Add(@event.TabroomId, @event);
                }

                tournament.JudgeCategories.Add(judgeCategory);
                tournament.CategoryMap.Add(judgeCategory.TabroomId, judgeCategory);
            }

            foreach (JSONTimeslot jsonTimeslot in jsonTournament.timeslots)
            {
                // Create the timeslot
                Timeslot timeslot = new Timeslot()
                {
                    Name = jsonTimeslot.name!,
                    TabroomId = int.Parse(jsonTimeslot.id!)
                };

                tournament.Timeslots.Add(timeslot);
                tournament.TimeslotMap.Add(timeslot.TabroomId, timeslot);
            }
        }

        private static void CreateSchools(Tournament tournament, JSONTournament jsonTournament)
        {
            foreach (JSONSchool jsonSchool in jsonTournament.schools) 
            {
                // Create School object
                School school = new School()
                {
                    Name = jsonSchool.name!,
                    TabroomId = int.Parse(jsonSchool.id!)
                };

                // Create Entry objects
                foreach (JSONEntry jsonEntry in jsonSchool.entries)
                {
                    if (jsonEntry.active! != 1 || jsonEntry.waitlist! != 0)
                        continue;

                    Entry entry = new Entry()
                    {
                        Code = jsonEntry.code!,
                        TabroomId = int.Parse(jsonEntry.id!),
                        EventId = int.Parse(jsonEntry.@event!),
                        School = school
                    };

                    school.Entries.Add(entry);
                    tournament.EntryMap.Add(entry.TabroomId, entry);

                    if (tournament.EventMap.TryGetValue(entry.EventId, out Event? @event) && @event != null)
                        @event.Entries.Add(entry);

                }

                tournament.Schools.Add(school);
                tournament.SchoolMap.Add(school.TabroomId, school);
            }

            // Creates a school to hold all tournament hired judges.
            School Hires = new School()
            {
                Name = "Tournament Hires",
                TabroomId = 0
            };
            tournament.Schools.Add(Hires);
            tournament.SchoolMap.Add(Hires.TabroomId, Hires);
        }

        private static void CreateJudges(Tournament tournament, JSONTournament jsonTournament)
        {
            foreach (JSONCategory jsonCategory in jsonTournament.categories)
            {
                if (tournament.CategoryMap.TryGetValue(int.Parse(jsonCategory.id!), out JudgeCategory? category))
                {
                    foreach (JSONJudge jsonJudge in jsonCategory.judges)
                    {
                        Judge judge = new Judge()
                        {
                            Name = jsonJudge.first! + " " + jsonJudge.last!,
                            TabroomId = long.Parse(jsonJudge.id!),
                            Obligation = jsonJudge.obligation ?? 0,
                            School = tournament.SchoolMap[int.Parse(jsonJudge.school ?? "0")]
                        };

                        judge.SchoolStrikes.Add(judge.School);

                        foreach (JSONRating jsonRating in jsonJudge.ratings)
                        {
                            int entryId = int.Parse(jsonRating.entry!);

                            if (tournament.EntryMap.TryGetValue(entryId, out Entry? entry) && entry != null)
                                entry.PreferenceSheet.Add(judge, float.Parse(jsonRating.percentile!));
                        }

                        foreach (JSONStrike jsonStrike in jsonJudge.strikes)
                        {
                            switch (jsonStrike.tag)
                            {
                                case "event":
                                    Event @event = tournament.EventMap[(int)jsonStrike.@event!];
                                    judge.EventStrikes.Add(@event);
                                    break;
                                case "entry":
                                    Entry entry = tournament.EntryMap[(int)jsonStrike.entry!];
                                    judge.EntryStrikes.Add(entry);
                                    break;
                                case "school":
                                    School school = tournament.SchoolMap[(int)jsonStrike.school!];
                                    judge.SchoolStrikes.Add(school);
                                    break;
                                default:
                                    break;
                            }
                        }

                        category.Judges.Add(judge);
                        tournament.JudgeMap.Add(judge.TabroomId, judge);
                    }

                    foreach (JSONJudgePool jsonJudgePool in jsonCategory.judge_pools)
                    {
                        JudgePool judgePool = new JudgePool()
                        {
                            Name = jsonJudgePool.name!,
                            TabroomId = int.Parse(jsonJudgePool.id!)
                        };

                        foreach (int judgeId in jsonJudgePool.judges)
                        {
                            if (tournament.JudgeMap.TryGetValue(judgeId, out Judge? judge) && judge != null)
                                judgePool.Judges.Add(judge);
                        }

                        category.JudgePools.Add(judgePool);
                        tournament.PoolMap.Add(judgePool.TabroomId, judgePool);
                    }
                }
            }
        }

        private static void CreateRounds(Tournament tournament, JSONTournament jsonTournament)
        {
            List<Tuple<Event, JSONRound>> roundsToProcess = GetJSONRounds(tournament, jsonTournament);

            foreach (Tuple<Event, JSONRound> round in roundsToProcess)
            {
                Event @event = round.Item1;
                JSONRound jsonRound = round.Item2;

                Round debateRound = new Round()
                {
                    Name = "Round " + jsonRound.name!.ToString(),
                    RoundNum = (int)jsonRound.name
                };

                switch(jsonRound.type!)
                {
                    case "prelim":
                        debateRound.Type = RoundTypeEnum.Preset;
                        break;
                    case "highlow":
                        debateRound.Type = RoundTypeEnum.HighLow;
                        break;
                    case "highhigh":
                        debateRound.Type = RoundTypeEnum.HighHigh;
                        break;
                    case "elim":
                        debateRound.Type = RoundTypeEnum.Elim;
                        break;
                    default:
                        break;
                }

                if (tournament.TimeslotMap.TryGetValue(jsonRound.timeslot ?? 0, out Timeslot? timeslot) && timeslot != null)
                    debateRound.Timeslot = timeslot;

                foreach (JSONRoundSetting jsonRoundSetting in jsonRound.settings)
                {
                    if (jsonRoundSetting.tag!.Equals("num_judges"))
                        debateRound.PanelSize = int.Parse(jsonRoundSetting.value!);
                }

                foreach (JSONSection jsonSection in jsonRound.sections)
                {
                    Debate debate = new Debate()
                    {
                        Number = int.Parse(jsonSection.letter!)
                    };

                    int numBallots = 0;

                    foreach (JSONBallot jsonBallot in jsonSection.ballots)
                    {
                        bool isAff = jsonBallot.side! == 1;

                        if (tournament.EntryMap.TryGetValue(jsonBallot.entry ?? 0, out Entry? entry) && entry != null)
                        {
                            if (isAff)
                                debate.Affirmative = entry;
                            else
                                debate.Negative = entry;

                            if (!debate.EntryBallotMap.ContainsKey(entry))
                                debate.EntryBallotMap.Add(entry, 0);

                            foreach (JSONScore jsonScore in jsonBallot.scores)
                            {
                                if (!jsonScore.tag!.Equals("winloss"))
                                    continue;

                                numBallots++;

                                if (jsonScore.value! == 1)
                                    debate.EntryBallotMap[entry] = 1;
                            }

                            // Means a bye/forfeit combination occured.
                            if (jsonBallot.scores.Count == 0)
                            {
                                if (jsonBallot.bye != null)
                                {
                                    entry.Wins++;
                                    entry.WinLossMap.Add(debateRound.RoundNum, true);
                                }
                                else
                                {
                                    entry.WinLossMap.Add(debateRound.RoundNum, false);
                                }
                            }
                        }

                        if (tournament.JudgeMap.TryGetValue(jsonBallot.judge ?? 0, out Judge? judge) && judge != null && !debate.Judges.Contains(judge))
                        {
                            debate.Judges.Add(judge);

                            if (debate.Affirmative != null)
                                debate.Affirmative!.PreviousJudges.Add(judge);
                            if (debate.Negative != null)
                                debate.Negative!.PreviousJudges.Add(judge);

                            judge.RoundsJudged++;
                        }
                    }

                    // De-duplicates the number of ballots.
                    numBallots /= 2;

                    if (debate.Affirmative != null && debate.Negative != null)
                    {
                        debate.Bracket = Math.Max(debate.Affirmative.Wins, debate.Negative.Wins);

                        if (numBallots > 0 && debate.EntryBallotMap[debate.Affirmative] / numBallots > 0.5)
                        {
                            debate.Affirmative.Wins++;
                            debate.Affirmative.WinLossMap.Add(debateRound.RoundNum, true);
                            debate.Negative.WinLossMap.Add(debateRound.RoundNum, false);
                        }
                        else if (numBallots > 0)
                        {
                            debate.Negative.Wins++;
                            debate.Negative.WinLossMap.Add(debateRound.RoundNum, true);
                            debate.Affirmative.WinLossMap.Add(debateRound.RoundNum, false);
                        }
                    }

                    debateRound.Debates.Add(debate);
                    debateRound.Event = @event;
                }

                @event.Rounds.Add(debateRound);
                @event.Rounds = @event.Rounds.OrderBy(rd => rd.RoundNum).ToList();
            }
        }

        private static List<Tuple<Event, JSONRound>> GetJSONRounds(Tournament tournament, JSONTournament jsonTournament)
        {
            List<Tuple<Event, JSONRound>> roundsToProcess = new();

            foreach (JSONCategory jsonCategory in jsonTournament.categories)
            {
                foreach (JSONEvent jsonEvent in jsonCategory.events)
                {
                    if (tournament.EventMap.TryGetValue(int.Parse(jsonEvent.id!), out Event? @event) && @event != null)
                    {
                        foreach (JSONRound jsonRound in jsonEvent.rounds)
                            roundsToProcess.Add(new Tuple<Event, JSONRound>(@event, jsonRound));
                    }
                }
            }

            return roundsToProcess;
        }
    }
}
