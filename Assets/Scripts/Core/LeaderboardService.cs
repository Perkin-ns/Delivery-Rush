using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LeaderboardEntry
{
    public string name;
    public int score;
}

[Serializable]
public class LeaderboardSaveData
{
    public List<LeaderboardEntry> entries = new();
}

public class LeaderboardService : ILeaderboardService
{
    private const string SaveKey = "Leaderboard";
    private const int MaxEntries = 10;

    private readonly IPersistenceService save;
    private readonly List<LeaderboardEntry> entries;

    public LeaderboardService(IPersistenceService save)
    {
        this.save = save;
        entries = Load();
    }

    public void Add(string name, int score)
    {
        entries.Add(new LeaderboardEntry { name = name, score = score });
        entries.Sort((a, b) => b.score.CompareTo(a.score));
        if (entries.Count > MaxEntries)
            entries.RemoveRange(MaxEntries, entries.Count - MaxEntries);
        Persist();
    }

    public IReadOnlyList<LeaderboardEntry> GetTop(int count)
    {
        int n = Mathf.Min(count, entries.Count);
        return entries.GetRange(0, n);
    }

    private List<LeaderboardEntry> Load()
    {
        string json = save.GetString(SaveKey, "");
        if (string.IsNullOrEmpty(json))
            return new List<LeaderboardEntry>();

        LeaderboardSaveData data = JsonUtility.FromJson<LeaderboardSaveData>(json);
        return data != null && data.entries != null ? data.entries : new List<LeaderboardEntry>();
    }

    private void Persist()
    {
        var data = new LeaderboardSaveData { entries = entries };
        save.SetString(SaveKey, JsonUtility.ToJson(data));
        save.Save();
    }
}
