using UnityEngine;
using TMPro;

public class HUDController : MonoBehaviour
{
    [Header("Références UI")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI livesText;

    private void Start()
    {
        GameManager.Instance.OnScoreChanged += AfficherScore;
        GameManager.Instance.OnLivesChanged += AfficherVies;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.OnScoreChanged -= AfficherScore;
        GameManager.Instance.OnLivesChanged -= AfficherVies;
    }

    private void AfficherScore(int score)
    {
        scoreText.text = $"Score : {score}";
    }

    private void AfficherVies(int vies)
    {
        livesText.text = $"Vies : {vies}";
    }
}
