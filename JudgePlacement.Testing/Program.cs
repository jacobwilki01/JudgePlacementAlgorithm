using JudgePlacement.Data;
using JudgePlacement.JSON;
using JudgePlacement.JSON.Data;
using JudgePlacement.Placement;
using System.Text.Json;

string filepath = "C:\\Users\\jwilk\\Downloads\\Complete-KCKCC Policy Debate TOC Qualifier-2024-04-23-at-17-51-21.json";
string jsonString = File.ReadAllText(filepath);

Tournament tournament = TournamentJSONProcessor.CreateNewTournament(jsonString);

Round TOCRound = tournament.Events[1].Rounds[5];
JudgePool JudgePoolRound6 = tournament.JudgeCategories[0].JudgePools[1];

// temp code
Judge Chief = JudgePoolRound6.Judges.Find(jdg => jdg.Name.Contains("Darren Elliott"))!;
Judge Wilkus = JudgePoolRound6.Judges.Find(jdg => jdg.Name.Contains("Wilkus"))!;
Judge Kaut = JudgePoolRound6.Judges.Find(jdg => jdg.Name.Contains("Kaut"))!;
Judge Swanson = JudgePoolRound6.Judges.Find(jdg => jdg.Name.Contains("Swanson"))!;

JudgePoolRound6.Judges.Remove(Chief);
JudgePoolRound6.Judges.Remove(Wilkus);
JudgePoolRound6.Judges.Remove(Kaut);
JudgePoolRound6.Judges.Remove(Swanson);

for (int i = 5; i < 11; i++)
{
    tournament.Events[1].Rounds[i].EraseJudges();
}
for (int i = 5; i < 10; i++)
{
    tournament.Events[0].Rounds[i].EraseJudges();
}

JudgePlacement.Placement.JudgePlacement.PlaceJudges(TOCRound, JudgePoolRound6);

TOCRound.Debates = TOCRound.Debates.OrderBy(debate => debate.Bracket).Reverse().ToList();

int numImprovedAff = 0;
int numImprovedNeg = 0;
int numImprovedMutuals = 0;
foreach (Debate debate in TOCRound.Debates)
{
    Console.WriteLine(debate.GetConsoleLine());

    Tuple<float, float, float, float> PrefData = debate.GetPrefData();

    float curAff = PrefData.Item1;
    float curNeg = PrefData.Item2;
    float prevAff = PrefData.Item3;
    float prevNeg = PrefData.Item4;

    float curDiff = Math.Abs(curAff - curNeg);
    float prevDiff = Math.Abs(prevAff - prevNeg);

    if (curAff <= prevAff)
        numImprovedAff++;
    if (curNeg <= prevNeg)
        numImprovedNeg++;
    if (curDiff <= prevDiff)
        numImprovedMutuals++;

    Console.WriteLine("In this debate, the affirmative had a higher pref ("
        + (curAff <= prevAff).ToString()
        + "). The negative had a higher pref ("
        + (curNeg <= prevNeg).ToString()
        + "). The difference was lower ("
        + (curDiff <= prevDiff).ToString()
        + ").\n");
}

Console.WriteLine("The number of improved AFF values was: "
    + numImprovedAff.ToString()
    + ".");
Console.WriteLine("The number of improved NEG values was: "
    + numImprovedNeg.ToString()
    + ".");
Console.WriteLine("The number of improved mutuality values was: "
    + numImprovedMutuals.ToString()
    + ".");
Console.WriteLine("The number of debates was: "
    + TOCRound.Debates.Count.ToString()
    + ".");

while (true) { }