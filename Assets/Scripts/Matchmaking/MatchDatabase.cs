using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using MongoDB.Driver;
using MongoDB.Bson;

public class MatchDatabase : MonoBehaviour
{
    MongoClient client = new MongoClient("mongodb+srv://orpheus_roleDB:uJPKdHo5Kn0S8fds@rps-online.eckyjkp.mongodb.net/?retryWrites=true&w=majority");
    IMongoDatabase database;
    IMongoCollection<BsonDocument> collection;

    // Start is called before the first frame update
    void Start()
    {
        database = client.GetDatabase("RpsMatch");
        collection = database.GetCollection<BsonDocument>("MatchResult");
    }

    public async void WriteScoreToDatabase(string Player1Name, string Player2Name, float Player1Score, float Player2Score, List<RoundHistory> RoundHistories)
    {
        var document = new BsonDocument { 
            { "matchID", Guid.NewGuid().ToString() } ,
            { "name1", Player1Name },
            { "name2", Player2Name },
            { "score1", Player1Score },
            { "score2", Player2Score },
            { "histories",  SetUpHistories(RoundHistories) }
        };

        Debug.Log(document);

        await collection.InsertOneAsync(document);
    }

    public async Task<List<MatchData>> GetDataFromDatabase()
    {
        var allMatchTask = collection.FindAsync(new BsonDocument());
        var matchAwaited = await allMatchTask;

        List<MatchData> matches = new List<MatchData>();
        foreach(var match in matchAwaited.ToList())
        {
            matches.Add(Deserialize(match));
        }

        return matches;
    }

    private MatchData Deserialize(BsonDocument match)
    {
        var matchData = new MatchData();

        matchData.MatchID = match["matchID"].AsString;
        matchData.Player1Name = match["name1"].AsString;
        matchData.Player2Name = match["name2"].AsString;
        matchData.Player1Score = (float)match["score1"].AsDouble;
        matchData.Player2Score = (float)match["score2"].AsDouble;
        matchData.RoundHistories = BreakDownHistories(match["histories"].AsBsonArray);

        return matchData;
    }

    private List<RoundHistory> BreakDownHistories(BsonArray histories)
    {
        List<RoundHistory> ret = new List<RoundHistory>();

        foreach(var history in histories)
        {
            var roundHistory = new RoundHistory();
            roundHistory.Round = history["round"].AsInt32;
            roundHistory.Choice1 = history["choice1"].AsString;
            roundHistory.Choice2 = history["choice2"].AsString;

            ret.Add(roundHistory);
        }

        return ret;
    }

    private BsonArray SetUpHistories(List<RoundHistory> histories)
    {
        BsonArray ret = new BsonArray();
        histories.ForEach((history) =>
        {
            var newHistory = new BsonDocument
            {
                { "round", history.Round },
                { "choice1", history.Choice1 },
                { "choice2", history.Choice2 }
            };

            ret.Add(newHistory);
        });

        return ret;
    }
}

public class MatchData
{
    public string MatchID { get; set; }
    public string Player1Name { get; set; }
    public string Player2Name { get; set; }
    public float Player1Score { get; set; }
    public float Player2Score { get; set; }
    public List<RoundHistory> RoundHistories { get; set; }
}

public class RoundHistory
{
    public int Round { get; set; }
    public string Choice1 { get; set; }
    public string Choice2 { get; set; }

    public RoundHistory()
    {
        this.Round = 0;
        this.Choice1 = "Choosing";
        this.Choice2 = "Choosing";
    }

    public RoundHistory(int round, string choice1, string choice2)
    {
        this.Round = round;
        this.Choice1 = choice1;
        this.Choice2 = choice2;
    }
}