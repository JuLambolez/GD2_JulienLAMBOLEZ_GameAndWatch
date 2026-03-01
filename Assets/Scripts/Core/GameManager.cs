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
    // Durée du temps de battement avant le premier mini-jeu et entre chaque mini-jeu
    [SerializeField] private float delaiEntreMinijeux = 5f;

    private int _score;
    private int _lives;
    private int _currentIndex;
    private MinigameBase _currentMinigame;

    public event Action<int> OnScoreChanged;
    public event Action<int> OnLivesChanged;

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

        // Temps de battement initial avant le premier mini-jeu
        StartCoroutine(RoutineEntreMinijeux());
    }

    /// Détruit le mini-jeu actif, attend le délai, puis charge le suivant.
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
            Victory();
            return;
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
        _currentMinigame.OnMinigameStart(definition);
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

    private void Victory()
    {
        Debug.Log("[GameManager] Victoire !");
        // TODO : charger l'écran de victoire
    }

    private void GameOver()
    {
        Debug.Log("[GameManager] Game Over !");
        // TODO : charger l'écran de game over
    }
}
