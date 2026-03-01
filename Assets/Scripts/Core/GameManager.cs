using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Configuration")]
    [SerializeField] private MinigameSequence minigameSequence;
    [SerializeField] private Transform minigameContainer;
    [SerializeField] private int startingLives = 3;
    [SerializeField] private float delaiEntreMinijeux = 5f;

    [Header("UI")]
    [SerializeField] private GameOverController gameOverController;

    private const int MaxNiveauDifficulte = 3;

    private int _score;
    private int _lives;
    private int _currentIndex;
    private int _niveauDifficulte;
    private MinigameBase _currentMinigame;

    public event Action<int> OnScoreChanged;
    public event Action<int> OnLivesChanged;
    public event Action<int> OnDifficulteChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        _lives = startingLives;
        _score = 0;
        _currentIndex = 0;
        _niveauDifficulte = 0;

        StartCoroutine(RoutineEntreMinijeux());
    }

    private IEnumerator RoutineEntreMinijeux()
    {
        if (_currentMinigame != null)
        {
            Destroy(_currentMinigame.gameObject);
            _currentMinigame = null;
        }

        yield return new WaitForSeconds(delaiEntreMinijeux);

        LoadCurrentMinigame();
    }

    private void LoadCurrentMinigame()
    {
        if (_currentIndex >= minigameSequence.Minigames.Count)
        {
            _currentIndex = 0;

            if (_niveauDifficulte < MaxNiveauDifficulte)
            {
                _niveauDifficulte++;
                OnDifficulteChanged?.Invoke(_niveauDifficulte);
                Debug.Log($"[GameManager] Difficulté augmentée → niveau {_niveauDifficulte}");
            }
        }

        MinigameDefinition definition = minigameSequence.Minigames[_currentIndex];

        GameObject instance = Instantiate(definition.Prefab, minigameContainer);
        _currentMinigame = instance.GetComponent<MinigameBase>();

        if (_currentMinigame == null)
        {
            Debug.LogError($"[GameManager] Le prefab '{definition.Prefab.name}' n'a pas de composant MinigameBase !");
            return;
        }

        _currentMinigame.OnWin += HandleWin;
        _currentMinigame.OnLose += HandleLose;
        _currentMinigame.OnMinigameStart(definition, _niveauDifficulte);
    }

    private void HandleWin()
    {
        _score++;
        OnScoreChanged?.Invoke(_score);
        _currentIndex++;
        StartCoroutine(RoutineEntreMinijeux());
    }

    private void HandleLose()
    {
        _lives--;
        OnLivesChanged?.Invoke(_lives);

        if (_lives <= 0)
        {
            GameOver();
            return;
        }

        _currentIndex++;
        StartCoroutine(RoutineEntreMinijeux());
    }

    private void GameOver()
    {
        // Détruire le mini-jeu en cours
        if (_currentMinigame != null)
        {
            Destroy(_currentMinigame.gameObject);
            _currentMinigame = null;
        }

        // Sauvegarder le score
        ScoreManager.SauvegarderScore(_score);

        // Afficher l'écran de game over
        gameOverController.Afficher(_score);
    }
}
