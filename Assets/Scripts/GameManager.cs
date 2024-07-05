using System.Collections;
using FireAimScripts;
using FireSeekingScripts;
using Scenes;
using Settings;
using Tracking;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private string comPort;

    private AfterLevelMenuDisplayer _afterLevelMenuDisplayer;

    //private FireSystem _fireSystem;
    private AimFireSystemHandler _aimFireHandler;
    private int _currentLevel;
    private SeekingFireSystemHandler _seekingFireHandler;

    private GameSettings _settings;
    public bool firstRun { get; set; } = true;
    public bool IsFireStarted { get; private set; }
    public string ComPort { get; set; }
    public GameObject PauseButton { get; set; }
    public Timer Timer { get; set; }
    public GameObject Hearts { get; set; }

    public bool FireSeekGameMode { get; set; }
    public bool FireAimGameMode { get; set; }

    private void Start()
    {
        MyInput.Instance.Init();
        if (_currentLevel == 0) Cursor.visible = false;

        ComPort = PlayerPrefs.GetString("COM");
        UnityEngine.Debug.Log("Com port settings:" + ComPort);
        if (string.IsNullOrEmpty(ComPort))
        {
            UnityEngine.Debug.Log("Cant load comport settings");
            ComPort = "COM0";
        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape)) MainMenu();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
        _settings = new GameSettings();
        _settings.Defaults();
        _settings.LoadSettings();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    public void InitSettings()
    {
        _settings.Defaults();
        _settings.LoadSettings();
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


    public void LoadHomeFireAimTVScene()
    {
        Cursor.visible = false;
        SceneManager.LoadScene("HomeFireAimTVScene");
    }

    public void LoadHomeFireAimGasScene()
    {
        Cursor.visible = false;
        SceneManager.LoadScene("HomeFireAimGasScene");
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

    public void MainMenu()
    {
        Cursor.visible = false;
        SceneManager.LoadScene(0);
        Tracker.Instance.TurnOn();
        MyInput.Instance.gameObject.SetActive(true);
        MyInput.Instance.Init();
        Cursor.visible = false;
    }

    private void FindFireSystems()
    {
        if (FireSeekGameMode)
            _seekingFireHandler = GameObject.Find("FireSystem").GetComponent<SeekingFireSystemHandler>();
        if (FireAimGameMode)
            _aimFireHandler = GameObject.Find("FireSystem").GetComponent<AimFireSystemHandler>();

        _afterLevelMenuDisplayer = FindFirstObjectByType<AfterLevelMenuDisplayer>();
        _afterLevelMenuDisplayer.gameObject.SetActive(false);

        Timer.gameObject.SetActive(false);
        if (FireAimGameMode)
            Hearts.SetActive(false);
    }

    private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        UnityEngine.Debug.Log("Level Loaded");
        UnityEngine.Debug.Log(scene.name);
        UnityEngine.Debug.Log(mode);
        if (scene.name == "SceneOne" || scene.name == "SceneTwo")
        {
            Cursor.visible = false;
            FindFireSystems();
        }

        if (scene.name == "MainMenu") Cursor.visible = false;

        switch (scene.name)
        {
            case "SceneOne":
                _currentLevel = 0;
                break;
            case "SceneOnePartTwo":
                _currentLevel = 1;
                break;
            case "SceneTwo":
                _currentLevel = 2;
                break;
            case "SceneTwoPartTwo":
                _currentLevel = 3;
                break;
        }

        MyInput.Instance.Init();
    }

    public IEnumerator StartFireWithDelay(float time)
    {
        yield return new WaitForSeconds(time);
        StartFire();
    }

    public void StartFire()
    {
        if (FireAimGameMode)
        {
            if (_aimFireHandler == null)
                FindFireSystems();

            _aimFireHandler.StartFire();
        }

        if (FireSeekGameMode)
        {
            if (_seekingFireHandler == null) FindFireSystems();

            _seekingFireHandler.StartFire();
        }

        UnityEngine.Debug.Log("Manager starting fire");


        Timer.gameObject.SetActive(true);
        if (FireAimGameMode)
            Hearts.SetActive(true);

        foreach (var trigger in GameObject.FindGameObjectsWithTag("Kid"))
            trigger.GetComponent<Animator>().SetTrigger("Fire");

        IsFireStarted = true;
    }

    public void EndScene(bool isWin)
    {
        IsFireStarted = false;

        _afterLevelMenuDisplayer.gameObject.SetActive(true);

        _afterLevelMenuDisplayer.Show(isWin);

        Timer.gameObject.SetActive(false);

        PauseButton.SetActive(false);
        PauseButton = null;

        if (FireAimGameMode)
        {
            Hearts.SetActive(false);
            _aimFireHandler.gameObject.SetActive(false);
        }

        if (FireSeekGameMode)
            _seekingFireHandler.gameObject.SetActive(false);
    }
}