using UnityEngine;
using UnityEngine.InputSystem;

public class MinijeuTap : MinigameBase
{
    [Header("Références UI")]
    [SerializeField] private RectTransform curseur;
    [SerializeField] private RectTransform piste;

    [Header("Paramètres")]
    [SerializeField] private float vitesse = 1.2f;
    // Demi-largeur de la zone verte en valeur normalisée (0 à 0.5)
    [SerializeField] private float tailleZoneVerte = 0.12f;

    // Position normalisée du curseur entre 0 et 1
    private float _positionNormalisee;
    private float _tempsDepart;

    protected override void Demarrer()
    {
        _tempsDepart = Time.time;
        _positionNormalisee = 0f;

        // Chaque niveau ajoute 0.4 à la vitesse de base
        vitesse += NiveauDifficulte * 0.4f;
    }


    protected override void SurMiseAJourJeu()
    {
        DeplacerCurseur();
        DetecterInput();
    }

    private void DeplacerCurseur()
    {
        // Ping-pong entre 0 et 1
        _positionNormalisee = Mathf.PingPong((Time.time - _tempsDepart) * vitesse, 1f);

        // Conversion en position locale sur la piste
        float demiLargeur = piste.rect.width * 0.5f;
        float positionX = Mathf.Lerp(-demiLargeur, demiLargeur, _positionNormalisee);

        curseur.anchoredPosition = new Vector2(positionX, curseur.anchoredPosition.y);
    }

    private void DetecterInput()
    {
        bool aAppuye = false;

        // Détection du tap sur mobile
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            aAppuye = true;

        // Détection du clic gauche dans l'éditeur
#if UNITY_EDITOR
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            aAppuye = true;
#endif

        if (!aAppuye) return;

        // Le centre de la zone verte est à 0.5 en normalisé
        bool dansLaZone = Mathf.Abs(_positionNormalisee - 0.5f) <= tailleZoneVerte;

        if (dansLaZone)
            Reussir();
        else
            Echouer();
    }
}
