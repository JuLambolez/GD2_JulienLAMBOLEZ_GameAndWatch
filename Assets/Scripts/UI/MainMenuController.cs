using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenuController : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioClip musiqueMenu;

    private const string MainGameSceneName = "MainGame";

    private void Start()
    {
        AudioManager.Instance.JouerMusique(musiqueMenu);
    }

    public void StartGame()
    {
        AudioManager.Instance.ArreterMusique();
        SceneManager.LoadScene(MainGameSceneName);
    }

    public void QuiGame()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}
