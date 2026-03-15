using UnityEngine;
using UnityEngine.InputSystem;

public class MinijeuEvite : MinigameBase
{
    [Header("Références UI")]
    [SerializeField] private RectTransform personnage;
    [SerializeField] private RectTransform zoneDeJeu;
    [SerializeField] private RectTransform[] haches;

    [Header("Paramètres")]
    [SerializeField] private int nombreEmplacements = 5;
    [SerializeField] private float margeHorizontale = 80f;
    [SerializeField] private float vitesseChuteBase = 400f;
    [SerializeField] private float bonusVitesseParNiveau = 120f;
    [SerializeField] private float intervalleSpawnBase = 1f;
    [SerializeField] private float reductionIntervalleParNiveau = 0.2f;

    private float[] _positionsX;
    private int _slotActuel;
    private float _demiLargeur;
    private float _posYPersonnage;
    private float _posYHaut;
    private bool[] _hachesActives;
    private float _vitesseChute;
    private float _intervalleSpawn;
    private float _tempsDernierSpawn;
    private int _prochainIndex;

    protected override void Demarrer()
    {
        Canvas.ForceUpdateCanvases();

        _demiLargeur = zoneDeJeu.rect.width * 0.5f;
        float demiHauteur = zoneDeJeu.rect.height * 0.5f;
        _posYPersonnage = -demiHauteur + 100f;
        _posYHaut = demiHauteur + 80f;

        // Calcul des 5 emplacements horizontaux fixes
        _positionsX = new float[nombreEmplacements];
        float largeurUtile = (_demiLargeur - margeHorizontale) * 2f;
        for (int i = 0; i < nombreEmplacements; i++)
            _positionsX[i] = -_demiLargeur + margeHorizontale + (largeurUtile / (nombreEmplacements - 1)) * i;

        // Personnage au centre
        _slotActuel = nombreEmplacements / 2;
        personnage.anchoredPosition = new Vector2(_positionsX[_slotActuel], _posYPersonnage);

        // Difficulté
        _vitesseChute = vitesseChuteBase + NiveauDifficulte * bonusVitesseParNiveau;
        _intervalleSpawn = Mathf.Max(0.3f, intervalleSpawnBase - NiveauDifficulte * reductionIntervalleParNiveau);

        _hachesActives = new bool[haches.Length];
        foreach (var hache in haches)
            hache.gameObject.SetActive(false);

        _tempsDernierSpawn = Time.time;
        _prochainIndex = 0;
    }

    protected override void SurMiseAJourJeu()
    {
        DetecterDeplacement();
        GererHaches();
    }

    private void DetecterDeplacement()
    {
        int direction = 0;

        // Un tap = un emplacement (wasPressedThisFrame)
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

        _slotActuel = Mathf.Clamp(_slotActuel + direction, 0, nombreEmplacements - 1);
        personnage.anchoredPosition = new Vector2(_positionsX[_slotActuel], _posYPersonnage);
    }

    private void GererHaches()
    {
        if (Time.time - _tempsDernierSpawn >= _intervalleSpawn)
        {
            SpawnerHache();
            _tempsDernierSpawn = Time.time;
        }

        for (int i = 0; i < haches.Length; i++)
        {
            if (!_hachesActives[i]) continue;

            float newY = haches[i].anchoredPosition.y - _vitesseChute * Time.deltaTime;
            haches[i].anchoredPosition = new Vector2(haches[i].anchoredPosition.x, newY);

            // Collision : même X (même slot) et même niveau Y que le personnage
            if (newY <= _posYPersonnage + 50f)
            {
                float distX = Mathf.Abs(haches[i].anchoredPosition.x - personnage.anchoredPosition.x);
                if (distX < margeHorizontale * 0.5f)
                {
                    Echouer();
                    return;
                }
            }

            if (newY < _posYPersonnage - 120f)
            {
                haches[i].gameObject.SetActive(false);
                _hachesActives[i] = false;
            }
        }
    }

    private void SpawnerHache()
    {
        for (int i = 0; i < haches.Length; i++)
        {
            int index = (_prochainIndex + i) % haches.Length;
            if (_hachesActives[index]) continue;

            // Spawn sur un emplacement fixe aléatoire
            int slotX = Random.Range(0, nombreEmplacements);
            haches[index].anchoredPosition = new Vector2(_positionsX[slotX], _posYHaut);
            haches[index].gameObject.SetActive(true);
            _hachesActives[index] = true;
            _prochainIndex = (index + 1) % haches.Length;
            return;
        }
    }
    protected override void SurExpiration()
    {
        Reussir();
    }
}
