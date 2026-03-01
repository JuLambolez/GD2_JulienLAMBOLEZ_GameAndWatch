using UnityEngine;
using UnityEngine.InputSystem;

public class MinijeuAttrape : MinigameBase
{
    [Header("Références UI")]
    [SerializeField] private RectTransform personnage;
    [SerializeField] private RectTransform objetQuiTombe;
    [SerializeField] private RectTransform zoneDeJeu;

    [Header("Paramètres")]
    [SerializeField] private int nombreEmplacementsH = 5;
    [SerializeField] private int nombreEmplacementsV = 4;
    [SerializeField] private float margeHorizontale = 80f;
    [SerializeField] private float margeVerticale = 100f;
    // Temps en secondes entre chaque déplacement vertical de l'objet
    [SerializeField] private float intervalleChute = 0.8f;

    private float[] _positionsX;
    private float[] _positionsY;
    private int _slotH;
    private int _slotVObjet;
    private int _slotHObjet;
    private float _tempsDernierDeplacement;
    private bool _resolved;

    protected override void Demarrer()
    {
        Canvas.ForceUpdateCanvases();
        _resolved = false;

        float demiLargeur = zoneDeJeu.rect.width * 0.5f;
        float demiHauteur = zoneDeJeu.rect.height * 0.5f;

        // Calcul des emplacements horizontaux
        _positionsX = new float[nombreEmplacementsH];
        float largeurUtile = (demiLargeur - margeHorizontale) * 2f;
        for (int i = 0; i < nombreEmplacementsH; i++)
            _positionsX[i] = -demiLargeur + margeHorizontale + (largeurUtile / (nombreEmplacementsH - 1)) * i;

        // Calcul des emplacements verticaux (haut → bas)
        _positionsY = new float[nombreEmplacementsV];
        float hauteurUtile = (demiHauteur - margeVerticale) * 2f;
        for (int i = 0; i < nombreEmplacementsV; i++)
            _positionsY[i] = demiHauteur - margeVerticale - (hauteurUtile / (nombreEmplacementsV - 1)) * i;

        // Personnage au centre en bas (dernier emplacement vertical)
        _slotH = nombreEmplacementsH / 2;
        personnage.anchoredPosition = new Vector2(_positionsX[_slotH], _positionsY[nombreEmplacementsV - 1]);

        // Objet sur un emplacement horizontal aléatoire en haut
        _slotHObjet = Random.Range(0, nombreEmplacementsH);
        _slotVObjet = 0;
        objetQuiTombe.anchoredPosition = new Vector2(_positionsX[_slotHObjet], _positionsY[_slotVObjet]);

        _tempsDernierDeplacement = Time.time;
    }

    protected override void SurMiseAJourJeu()
    {
        DetecterDeplacement();
        FaireChuter();
    }

    private void DetecterDeplacement()
    {
        int direction = 0;

        if (Touchscreen.current != null)
        {
            foreach (var touch in Touchscreen.current.touches)
            {
                if (!touch.press.wasPressedThisFrame) continue;
                direction = touch.position.ReadValue().x < Screen.width * 0.5f ? -1 : 1;
            }
        }

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

        _slotH = Mathf.Clamp(_slotH + direction, 0, nombreEmplacementsH - 1);
        personnage.anchoredPosition = new Vector2(_positionsX[_slotH], _positionsY[nombreEmplacementsV - 1]);
    }

    private void FaireChuter()
    {
        if (_resolved) return;
        if (Time.time - _tempsDernierDeplacement < intervalleChute) return;

        _tempsDernierDeplacement = Time.time;
        _slotVObjet++;

        // L'objet a atteint le dernier emplacement vertical (niveau du personnage)
        if (_slotVObjet >= nombreEmplacementsV - 1)
        {
            _resolved = true;
            objetQuiTombe.anchoredPosition = new Vector2(_positionsX[_slotHObjet], _positionsY[nombreEmplacementsV - 1]);
            VerifierAttrape();
            return;
        }

        objetQuiTombe.anchoredPosition = new Vector2(_positionsX[_slotHObjet], _positionsY[_slotVObjet]);
    }

    private void VerifierAttrape()
    {
        if (_slotHObjet == _slotH)
            Reussir();
        else
            Echouer();
    }
}
