using System;
using System.Collections;
using UnityEngine;

public abstract class MinigameBase : MonoBehaviour
{
    // Événements écoutés par le GameManager
    public event Action OnWin;
    public event Action OnLose;

    [Header("Instruction")]
    [SerializeField] private GameObject panneauInstruction;
    [SerializeField] private float dureeAffichageInstruction = 1.5f;

    private float _dureeTotale;
    private float _tempsRestant;
    private bool _enCours;

    /// Appelé par le GameManager pour démarrer le mini-jeu.
    public void OnMinigameStart(MinigameDefinition definition)
    {
        _dureeTotale = definition.TimerDuration;
        _tempsRestant = _dureeTotale;
        _enCours = false;

        StartCoroutine(RoutineIntro());
    }

    private IEnumerator RoutineIntro()
    {
        // Afficher le panneau d'instruction si présent
        if (panneauInstruction != null)
            panneauInstruction.SetActive(true);

        yield return new WaitForSeconds(dureeAffichageInstruction);

        // Cacher l'instruction et démarrer le jeu
        if (panneauInstruction != null)
            panneauInstruction.SetActive(false);

        _enCours = true;
        Demarrer();
    }

    private void Update()
    {
        if (!_enCours) return;

        _tempsRestant -= Time.deltaTime;

        SurMiseAJourTimer(_tempsRestant, _dureeTotale);

        if (_tempsRestant <= 0f)
        {
            Echouer();
            return;
        }

        SurMiseAJourJeu();
    }

    /// Logique d'initialisation propre à chaque mini-jeu.
    /// Appelé après la disparition du panneau d'instruction.
    protected abstract void Demarrer();

    /// Appelé à chaque frame tant que le mini-jeu est en cours.
    /// À surcharger dans les classes enfants à la place de Update().
    protected virtual void SurMiseAJourJeu() { }

    /// Appelé à chaque frame avec le temps restant et la durée totale.
    /// Optionnel : permet d'afficher une barre de progression.
    protected virtual void SurMiseAJourTimer(float tempsRestant, float dureeTotale) { }

    /// À appeler depuis le mini-jeu enfant quand le joueur réussit.
    protected void Reussir()
    {
        if (!_enCours) return;
        _enCours = false;
        OnWin?.Invoke();
    }

    /// À appeler depuis le mini-jeu enfant quand le joueur échoue,
    /// ou déclenché automatiquement à l'expiration du timer.
    protected void Echouer()
    {
        if (!_enCours) return;
        _enCours = false;
        OnLose?.Invoke();
    }
}
