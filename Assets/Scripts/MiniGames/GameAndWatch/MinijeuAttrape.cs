using UnityEngine;
using UnityEngine.InputSystem;

public class MinijeuAttrape : MinigameBase
{
    [Header("Références UI")]
    [SerializeField] private RectTransform personnage;
    [SerializeField] private RectTransform objetQuiTombe;
    [SerializeField] private RectTransform zoneDeJeu;

    [Header("Paramètres")]
    [SerializeField] private int nombreEmplacements = 5;
    [SerializeField] private float vitesseChute = 250f;
    // Marge depuis les bords gauche et droit de la zone de jeu
    [SerializeField] private float margeHorizontale = 80f;
    // Hauteur du personnage depuis le bas de la zone de jeu
    [SerializeField] private float hauteurPersonnage = 100f;

    private float[] _positionsX;
    private int _slotActuel;
    private float _posYPersonnage;
    private bool _resolved;

    protected override void Demarrer()
    {
        Canvas.ForceUpdateCanvases();
        _resolved = false;

        float demiLargeur = zoneDeJeu.rect.width * 0.5f;
        float demiHauteur = zoneDeJeu.rect.height * 0.5f;

        // Calcul des positions X de chaque emplacement
        _positionsX = new float[nombreEmplacements];
        float largeurUtile = (demiLargeur - margeHorizontale) * 2f;

        for (int i = 0; i < nombreEmplacements; i++)
        {
            _positionsX[i] = -demiLargeur + margeHorizontale + (largeurUtile / (nombreEmplacements - 1)) * i;
        }

        // Personnage au centre au départ
        _slotActuel = nombreEmplacements / 2;
        _posYPersonnage = -demiHauteur + hauteurPersonnage;
        personnage.anchoredPosition = new Vector2(_positionsX[_slotActuel], _posYPersonnage);

        // Objet sur un emplacement aléatoire en haut
        int slotDepart = Random.Range(0, nombreEmplacements);
        objetQuiTombe.anchoredPosition = new Vector2(_positionsX[slotDepart], demiHauteur - 50f);
    }

    protected override void SurMiseAJourJeu()
    {
        DetecterDeplacement();
        FaireChuter();
    }

    private void DetecterDeplacement()
    {
        int direction = 0;

        // Tap gauche ou droite sur mobile (wasPressedThisFrame = une seule détection par tap)
        if (Touchscreen.current != null)
        {
            foreach (var touch in Touchscreen.current.touches)
            {
                if (!touch.press.wasPressedThisFrame) continue;

                direction = touch.position.ReadValue().x < Screen.width * 0.5f ? -1 : 1;
            }
        }

        // Flèches ou QD dans l'éditeur
#if UNITY_EDITOR
        if (Keyboard.current != null)
        {
            if (Keyboard.current.leftArrowKey.wasPressedThisFrame || Keyboard.current.qKey.wasPressedThisFrame)
                direction = -1;
            if (Keyboard.current.rightArrowKey.wasPressedThisFrame || Keyboard.current.dKey.wasPressedThisFrame)
                direction = 1;
        }
#endif

        if (direction == 0) return;

        // Déplacement d'un emplacement, bloqué aux bords
        _slotActuel = Mathf.Clamp(_slotActuel + direction, 0, nombreEmplacements - 1);
        personnage.anchoredPosition = new Vector2(_positionsX[_slotActuel], _posYPersonnage);
    }

    private void FaireChuter()
    {
        if (_resolved) return;

        float nouveauY = objetQuiTombe.anchoredPosition.y - vitesseChute * Time.deltaTime;
        objetQuiTombe.anchoredPosition = new Vector2(objetQuiTombe.anchoredPosition.x, nouveauY);

        // L'objet a atteint le niveau du personnage
        if (nouveauY <= _posYPersonnage)
        {
            _resolved = true;
            VerifierAttrape();
        }
    }

    private void VerifierAttrape()
    {
        // Comme l'objet est forcément sur un emplacement, la distance peut être comparée directement aux slots
        float distanceX = Mathf.Abs(objetQuiTombe.anchoredPosition.x - personnage.anchoredPosition.x);

        if (distanceX <= margeHorizontale)
            Reussir();
        else
            Echouer();
    }
}
