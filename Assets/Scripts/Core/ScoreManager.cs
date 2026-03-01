using System.Collections.Generic;
using UnityEngine;

public static class ScoreManager
{
    private const string CleScores = "MeilleursScores";
    private const int MaxScores = 5;

    [System.Serializable]
    private class ListeScores
    {
        public List<int> scores = new List<int>();
    }

    /// Retourne la liste des meilleurs scores triés par ordre décroissant.
    public static List<int> ChargerScores()
    {
        string json = PlayerPrefs.GetString(CleScores, "");

        if (string.IsNullOrEmpty(json))
            return new List<int>();

        return JsonUtility.FromJson<ListeScores>(json).scores;
    }

    /// Ajoute un score à la liste, conserve uniquement le top 5 et sauvegarde.
    public static void SauvegarderScore(int nouveauScore)
    {
        List<int> scores = ChargerScores();
        scores.Add(nouveauScore);

        // Tri décroissant
        scores.Sort((a, b) => b.CompareTo(a));

        if (scores.Count > MaxScores)
            scores.RemoveRange(MaxScores, scores.Count - MaxScores);

        ListeScores liste = new ListeScores { scores = scores };
        PlayerPrefs.SetString(CleScores, JsonUtility.ToJson(liste));
        PlayerPrefs.Save();
    }
}
