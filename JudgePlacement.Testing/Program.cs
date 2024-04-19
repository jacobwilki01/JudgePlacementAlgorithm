using JudgePlacement.JSON;
using JudgePlacement.JSON.Data;
using System.Text.Json;

string filepath = "C:\\Users\\jwilk\\Downloads\\Complete-KCKCC Policy Debate TOC Qualifier-2024-04-19-at-01-36-06.json";
string jsonString = File.ReadAllText(filepath);

TournamentJSONProcessor.CreateNewTournament(jsonString);

while (true) { }