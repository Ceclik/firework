using UnityEngine;

public class MenuButtonsHandler : MonoBehaviour
{

    [SerializeField] private GameObject pickGameMode;
    [SerializeField] private GameObject pickLocation;

    private GameManager _gameManager;

    private void Start()
    {
        _gameManager = FindFirstObjectByType<GameManager>();
    }

    public void OnExitButtonClick()
    {
        Application.Quit();
    }

    public void OnSchoolLocationButtonClick()
    {
        FindFirstObjectByType<GameManager>().LoadSchoolFireSeekScene();
    }

    public void OnHomeTVSceneButtonClick()
    {
        FindFirstObjectByType<GameManager>().LoadHomeFireAimTVScene();
    }

    public void OnHomeGasSceneButtonClick()
    {
        FindFirstObjectByType<GameManager>().LoadHomeFireAimGasScene();
    }

    public void OnSettingsSceneButtonClick()
    {
        FindFirstObjectByType<GameManager>().SceneSettings();
    }

    public void OnSeekFireModeButtonClick()
    {
        FindFirstObjectByType<GameManager>().FireSeekGameMode = true;
        FindFirstObjectByType<GameManager>().FireAimGameMode = false;
        pickGameMode.SetActive(false);
        pickLocation.SetActive(true);
    }

    public void OnFireAimModeButtonClick()
    {
        FindFirstObjectByType<GameManager>().FireSeekGameMode = false;
        FindFirstObjectByType<GameManager>().FireAimGameMode = true;
        pickGameMode.SetActive(false);
        pickLocation.SetActive(true);
    }
}
