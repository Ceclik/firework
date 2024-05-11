using System;
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
    
    public void OnSchoolSceneButtonClick()
    {
        if (_gameManager.FireSeekGameMode)
            _gameManager.LoadSchoolFireSeekScene();
        else
            _gameManager.LoadSchoolFireAimScene(); 
    }

    public void OnHomeSceneButtonClick()
    {
        if(_gameManager.FireSeekGameMode)
            _gameManager.LoadHomeFireSeekScene();
        else
            _gameManager.LoadHomeFireAimScene();
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
