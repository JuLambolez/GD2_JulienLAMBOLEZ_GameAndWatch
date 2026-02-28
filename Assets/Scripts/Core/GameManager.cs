using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Configuration")]
    [SerializeField] private MinigameSequence minigameSequence;
    [SerializeField] private Transform minigameContainer;
    [SerializeField] private int startingLives = 3;

    private int _score;
    private int _lives;
    private int _currentIndex;
    private MinigameBase _currentMinigame;

    // Événements consommés par le HUD
    public event Action<int> OnScoreChanged;
    public event Action<int> OnLivesChanged;
    public event Action<string> OnInstructionChanged;

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

        LoadCurrentMinigame();
    }

    /// <summary>
    /// Instancie le prefab du mini-jeu courant et le démarre.
    /// </summary>
    private void LoadCurrentMinigame()
    {
        if (_currentMinigame != null)
            Destroy(_currentMinigame.gameObject);

        if (_currentIndex >= minigameSequence.Minigames.Count)
        {
            Victory();
            return;
        }

        MinigameDefinition definition = minigameSequence.Minigames[_currentIndex];

        OnInstructionChanged?.Invoke(definition.Instruction);

        GameObject instance = Instantiate(definition.Prefab, minigameContainer);
        _currentMinigame = instance.GetComponent<MinigameBase>();

        // Vérification que le prefab contient bien un MinigameBase
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
        LoadCurrentMinigame();
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
        LoadCurrentMinigame();
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
