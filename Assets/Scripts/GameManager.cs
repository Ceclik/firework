using System.Collections;
using Scenes;
using Settings;
using Tracking;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

//Script in which all main game actions are implemented like changing scenes, finding fire systems in scenes etc. 
//Using game manager is bad practice. All actions should be in separate scripts.

public class GameManager : Singleton<GameManager>
{

    public bool firstRun = true;
    public bool fireStarted;
    public string comPort;
    private GameSettings _settings;

    private FireSystem _fireSystem;
    private AfterLevelMenuDisplayer _afterLevelMenuDisplayer;
    public Timer Timer { get; set; }
    private int _currentLevel;

    public bool FireSeekGameMode { get; set; }
    public bool FireAimGameMode { get; set; }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
        _settings = new GameSettings();
        _settings.Defaults();
        _settings.LoadSettings();
    }

    public void InitSettings()
    {
        _settings.Defaults();
        _settings.LoadSettings();
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    public void SaveSettings()
    {
        _settings.SaveSettings();
    }    

    public void UpdateLevelSettings(int levelIndex, LevelSettings settings)
    {
        _settings.levelSettings[levelIndex] = settings;
    }

    public LevelSettings GetLevelSettings(int levelIndex)
    {
        return _settings.GetSettings(levelIndex);
    }

    public LevelSettings GetLevelSettings()
    {
        return _settings.GetSettings(_currentLevel);
    }

    public void LoadSchoolFireAimScene()
    {
        Cursor.visible = false;
        SceneManager.LoadScene("SchoolFireAim");
    }

    public void LoadSchoolFireSeekScene()
    {
        Cursor.visible = false;
        SceneManager.LoadScene("SchoolFireSeekScene");
    }

    public void LoadHomeFireSeekScene()
    {
        Cursor.visible = false;
        SceneManager.LoadScene("HomeFireSeekScene");
    }

    public void LoadHomeFireAimScene()
    {
        Cursor.visible = false;
        SceneManager.LoadScene("HomeFireAimScene");
    }

    public void SceneCalibrate()
    {
        SceneManager.LoadScene("Calibrate");
    }

    public void SceneMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void SceneSettings()
    {
        Cursor.visible = true;
        MyInput.Instance.gameObject.SetActive(false);
        SceneManager.LoadScene("Settings");
    }
    
    void Start()
    {
        MyInput.Instance.Init();
        if (_currentLevel==0)
        {
            Cursor.visible = false;
        }
        comPort = PlayerPrefs.GetString("COM");
        Debug.Log("Com port settings:" + comPort);
        if (string.IsNullOrEmpty(comPort))
        {
            Debug.Log("Cant load comport settings");
            comPort = "COM0";
        }
    }

    public void MainMenu()
    {

        Cursor.visible = false;
        SceneManager.LoadScene(0);
        Tracker.Instance.TurnOn();
        MyInput.Instance.gameObject.SetActive(true);
        MyInput.Instance.Init();
        Cursor.visible = false;
    }
    
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            MainMenu();
        }
    }

    private void FindFireSystems()
    {
        _fireSystem = GameObject.Find("FireSystem").GetComponent<FireSystem>();
        _afterLevelMenuDisplayer = FindFirstObjectByType<AfterLevelMenuDisplayer>();
        //Timer = FindFirstObjectByType<Timer>();
        _afterLevelMenuDisplayer.gameObject.SetActive(false);
        
        Timer.gameObject.SetActive(false);
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Level Loaded");
        Debug.Log(scene.name);
        Debug.Log(mode);
        if (scene.name == "SceneOne" || scene.name == "SceneTwo")
        {
            Cursor.visible = false;
            FindFireSystems();
        }

        if (scene.name == "MainMenu")
        {
            Cursor.visible = false;
        }
        switch (scene.name)
        {
            case "SceneOne":_currentLevel = 0;
                break;
            case "SceneOnePartTwo":_currentLevel = 1;
                break;
            case "SceneTwo":
                _currentLevel = 2;
                break;
            case "SceneTwoPartTwo":
                _currentLevel = 3;
                break;
            default:
                break;
        }
        MyInput.Instance.Init();
    }

    public IEnumerator FireStartDelayed(float time)
    {
        yield return new WaitForSeconds(time);
        FireStart();
    }

    public void FireStart()
    {
        if (_fireSystem == null)
        {
            FindFireSystems();
        }
        Debug.Log("Manager starting fire");
        _fireSystem.StartFire();
        Timer.gameObject.SetActive(true);
        
        foreach (GameObject trigger in GameObject.FindGameObjectsWithTag("Kid"))
        {
            trigger.GetComponent<Animator>().SetTrigger("Fire");
        }

        fireStarted = true;
    }

    public void EndScene()
    {        
            Debug.Log(_afterLevelMenuDisplayer);
            _afterLevelMenuDisplayer.gameObject.SetActive(true);
            Debug.Log("timer" + Timer);
            _afterLevelMenuDisplayer.Show(Timer.Stars());
            Timer.gameObject.SetActive(false);          
            _fireSystem.gameObject.SetActive(false);
    }
}
