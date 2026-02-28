using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif
    public class MainMenuController : MonoBehaviour
    {
        private const string MainGameSceneName = "MainGame";

        public void StartGame()
        {
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
