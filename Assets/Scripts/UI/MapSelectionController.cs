using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapSelectionController : MonoBehaviour
{
    [Header("Panneau indisponible")]
    [SerializeField] private GameObject panneauIndisponible;
    [SerializeField] private float dureePanneau = 2f;

    private const string NomSceneMainGame = "MainGame";
    private bool _affichageEnCours;

    /// Appelé par le bouton du style Barbaric.
    public void OnStyleBarbareClique()
    {
        SceneManager.LoadScene(NomSceneMainGame);
    }

    /// Appelé par le bouton du style Pirate (indisponible).
    public void OnStylePirateClique()
    {
        if (_affichageEnCours) return;
        StartCoroutine(AfficherPanneauIndisponible());
    }

    /// Affiche le message "Disponible ultérieurement" pendant quelques secondes.
    private IEnumerator AfficherPanneauIndisponible()
    {
        _affichageEnCours = true;
        panneauIndisponible.SetActive(true);
        yield return new WaitForSeconds(dureePanneau);
        panneauIndisponible.SetActive(false);
        _affichageEnCours = false;
    }
}
