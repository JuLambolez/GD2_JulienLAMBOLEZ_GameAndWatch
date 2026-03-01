using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI texteScore;

    [Header("Audio")]
    [SerializeField] private AudioClip sonGameOver;

    private const string NomSceneMenu = "MainMenu";

    /// Initialise et affiche le panneau de game over avec le score final.
    public void Afficher(int score)
    {
        gameObject.SetActive(true);
        texteScore.text = $"Score : {score}";
        AudioManager.Instance.JouerSFX(sonGameOver);
    }

    /// Retourne au menu principal et détruit le GameManager persistant.
    public void ReturnToMainMenu()
    {
        Destroy(GameManager.Instance.gameObject);
        SceneManager.LoadScene(NomSceneMenu);
    }
}
