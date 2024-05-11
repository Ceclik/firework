using UnityEngine;

public class MenuActions : MonoBehaviour
{

    [SerializeField] private GameObject pickGameMode;
    [SerializeField] private GameObject pickLocation;
    
	public void OnExitButtonClick()
    {
        Application.Quit();
    }
    
    public void OnSchoolSceneButtonClick()
    {
        FindFirstObjectByType<GameManager>().SceneOne(); 
    }

    public void OnHomeSceneButtonClick()
    {
        FindFirstObjectByType<GameManager>().SceneTwo();
    }

    public void OnSettingsSceneButtonClick()
    {
        FindFirstObjectByType<GameManager>().SceneSettings();
    }

    public void OnSeekFireModeButtonClick()
    {
        FindFirstObjectByType<GameManager>().FireSeekGameMode = true;
        pickGameMode.SetActive(false);
        pickLocation.SetActive(true);
    }

    public void OnFireAimModeButtonClick()
    {
        FindFirstObjectByType<GameManager>().FireAimGameMode = true;
        pickGameMode.SetActive(false);
        pickLocation.SetActive(true);
    }
}
