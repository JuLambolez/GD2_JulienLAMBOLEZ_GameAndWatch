using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreboardController : MonoBehaviour
{
    [SerializeField] private GameObject panneauScoreboard;
    // Assigne 5 TextMeshProUGUI dans l'Inspector, un par ligne
    [SerializeField] private TextMeshProUGUI[] lignesScore;

    /// Affiche le panneau avec les meilleurs scores chargés.
    public void AfficherScoreboard()
    {
        panneauScoreboard.SetActive(true);

        List<int> scores = ScoreManager.ChargerScores();

        for (int i = 0; i < lignesScore.Length; i++)
        {
            lignesScore[i].text = i < scores.Count
                ? $"{i + 1}.   {scores[i]}"
                : $"{i + 1}.   ---";
        }
    }

    /// Ferme le panneau du scoreboard.
    public void FermerScoreboard()
    {
        panneauScoreboard.SetActive(false);
    }
}
