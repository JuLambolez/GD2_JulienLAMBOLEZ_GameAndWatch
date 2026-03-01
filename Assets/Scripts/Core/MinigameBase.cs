using System;
using System.Collections;
using UnityEngine;

public abstract class MinigameBase : MonoBehaviour
{
    public event Action OnWin;
    public event Action OnLose;

    [Header("Instruction")]
    [SerializeField] private GameObject panneauInstruction;
    [SerializeField] private float dureeAffichageInstruction = 1.5f;

    [Header("Audio")]
    [SerializeField] private AudioClip sonVictoire;
    [SerializeField] private AudioClip sonDefaite;

    private float _dureeTotale;
    private float _tempsRestant;
    private bool _enCours;

    protected int NiveauDifficulte { get; private set; }

    /// Appelé par le GameManager pour démarrer le mini-jeu.
    public void OnMinigameStart(MinigameDefinition definition, int niveauDifficulte)
    {
        NiveauDifficulte = niveauDifficulte;
        _dureeTotale = definition.TimerDuration;
        _tempsRestant = _dureeTotale;
        _enCours = false;

        StartCoroutine(RoutineIntro());
    }

    private IEnumerator RoutineIntro()
    {
        if (panneauInstruction != null)
            panneauInstruction.SetActive(true);

        yield return new WaitForSeconds(dureeAffichageInstruction);

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

    protected abstract void Demarrer();
    protected virtual void SurMiseAJourJeu() { }
    protected virtual void SurMiseAJourTimer(float tempsRestant, float dureeTotale) { }

    /// À appeler depuis le mini-jeu enfant quand le joueur réussit.
    protected void Reussir()
    {
        if (!_enCours) return;
        _enCours = false;
        AudioManager.Instance.JouerSFX(sonVictoire);
        OnWin?.Invoke();
    }

    /// À appeler depuis le mini-jeu enfant quand le joueur échoue,
    /// ou déclenché automatiquement à l'expiration du timer.
    protected void Echouer()
    {
        if (!_enCours) return;
        _enCours = false;
        AudioManager.Instance.JouerSFX(sonDefaite);
        OnLose?.Invoke();
    }
}
