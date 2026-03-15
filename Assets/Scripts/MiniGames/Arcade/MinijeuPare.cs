using UnityEngine;
using UnityEngine.InputSystem;

public class MinijeuPare : MinigameBase
{
    [Header("Références UI")]
    [SerializeField] private RectTransform fleche;
    [SerializeField] private GameObject bouclierGauche;
    [SerializeField] private GameObject bouclierDroit;

    [Header("Paramètres")]
    [SerializeField] private float vitesseBase = 500f;
    [SerializeField] private float bonusVitesseParNiveau = 200f;
    [SerializeField] private float distanceDepart = 700f;
    // Distance du bouclier par rapport au centre (doit correspondre à la position visuelle dans le prefab)
    [SerializeField] private float positionBouclier = 150f;

    // -1 = vient de la gauche (se déplace vers la droite), 1 = vient de la droite
    private int _directionFleche;
    private float _vitesseActuelle;
    private bool _resolved;
    // 0 = aucun bouclier actif, -1 = gauche, 1 = droite
    private int _bouclierActif;

    protected override void Demarrer()
    {
        _resolved = false;
        _bouclierActif = 0;
        bouclierGauche.SetActive(false);
        bouclierDroit.SetActive(false);

        _vitesseActuelle = vitesseBase + NiveauDifficulte * bonusVitesseParNiveau;
        _directionFleche = Random.value > 0.5f ? -1 : 1;

        float startX = _directionFleche == -1 ? -distanceDepart : distanceDepart;
        fleche.anchoredPosition = new Vector2(startX, fleche.anchoredPosition.y);

        // Flèche vient de la gauche → pointe à droite (0°)
        // Flèche vient de la droite → pointe à gauche (180°)
        fleche.localEulerAngles = _directionFleche == -1
            ? Vector3.zero
            : new Vector3(0f, 0f, 180f);
    }

    protected override void SurMiseAJourJeu()
    {
        if (_resolved) return;

        DetecterInput();
        DeplacerFleche();
        VerifierCollisions();
    }

    private void DetecterInput()
    {
        int directionInput = 0;

        if (Touchscreen.current != null)
        {
            foreach (var touch in Touchscreen.current.touches)
            {
                if (!touch.press.wasPressedThisFrame) continue;
                directionInput = touch.position.ReadValue().x < Screen.width * 0.5f ? -1 : 1;
            }
        }

#if UNITY_EDITOR
        if (Keyboard.current != null)
        {
            if (Keyboard.current.leftArrowKey.wasPressedThisFrame || Keyboard.current.qKey.wasPressedThisFrame)
                directionInput = -1;
            if (Keyboard.current.rightArrowKey.wasPressedThisFrame || Keyboard.current.dKey.wasPressedThisFrame)
                directionInput = 1;
        }
#endif

        if (directionInput == 0) return;

        // Un seul bouclier actif à la fois — switcher efface le précédent
        _bouclierActif = directionInput;
        bouclierGauche.SetActive(directionInput == -1);
        bouclierDroit.SetActive(directionInput == 1);
    }

    private void DeplacerFleche()
    {
        float deplacement = _vitesseActuelle * Time.deltaTime * -_directionFleche;
        fleche.anchoredPosition += new Vector2(deplacement, 0f);
    }

    private void VerifierCollisions()
    {
        float x = fleche.anchoredPosition.x;

        if (_directionFleche == -1) // flèche se déplace vers la droite
        {
            // Bouclier gauche placé entre la flèche et le personnage
            if (_bouclierActif == -1 && x >= -positionBouclier)
            {
                _resolved = true;
                Reussir();
                return;
            }

            // Flèche atteint le personnage (centre)
            if (x >= 0f)
            {
                _resolved = true;
                Echouer();
            }
        }
        else // flèche se déplace vers la gauche
        {
            // Bouclier droit placé entre la flèche et le personnage
            if (_bouclierActif == 1 && x <= positionBouclier)
            {
                _resolved = true;
                Reussir();
                return;
            }

            // Flèche atteint le personnage (centre)
            if (x <= 0f)
            {
                _resolved = true;
                Echouer();
            }
        }
    }
}
