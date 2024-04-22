using JudgePlacement.Data;
using JudgePlacement.JSON;
using JudgePlacement.JSON.Data;
using JudgePlacement.Placement;
using System.Text.Json;

string filepath = "C:\\Users\\jwilk\\Downloads\\Complete-KCKCC Policy Debate TOC Qualifier-2024-04-19-at-01-36-06.json";
string jsonString = File.ReadAllText(filepath);

Tournament tournament = TournamentJSONProcessor.CreateNewTournament(jsonString);

Round TOCRound6 = tournament.Events[1].Rounds[5];
Round NovRound6 = tournament.Events[0].Rounds[5];
JudgePool JudgePoolRound6 = tournament.JudgeCategories[0].JudgePools[1];

for (int i = 5; i < 11; i++)
{
    tournament.Events[1].Rounds[i].EraseJudges();
}
for (int i = 5; i < 10; i++)
{
    tournament.Events[0].Rounds[i].EraseJudges();
}

JudgePlacement.Placement.JudgePlacement.PlaceJudges(TOCRound6, JudgePoolRound6);

TOCRound6.Debates = TOCRound6.Debates.OrderBy(debate => debate.Bracket).Reverse().ToList();

foreach (Debate debate in TOCRound6.Debates)
{
    Console.WriteLine(debate.GetConsoleLine());
}

while (true) { }