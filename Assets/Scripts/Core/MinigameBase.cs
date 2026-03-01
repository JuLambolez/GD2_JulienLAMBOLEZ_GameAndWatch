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

    private float _dureeTotale;
    private float _tempsRestant;
    private bool _enCours;

    // Accessible par les classes enfants pour adapter la difficulté
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

    protected void Reussir()
    {
        if (!_enCours) return;
        _enCours = false;
        OnWin?.Invoke();
    }

    protected void Echouer()
    {
        if (!_enCours) return;
        _enCours = false;
        OnLose?.Invoke();
    }
}
