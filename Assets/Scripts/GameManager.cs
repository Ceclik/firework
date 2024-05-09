using System.Collections;
using Scenes;
using Settings;
using Tracking;
using UnityEngine;
using UnityEngine.SceneManagement;

//Script in which all main game actions are implemented like changing scenes, finding fire systems in scenes etc. 
//Using game manager is bad practice. All actions should be in separate scripts.

public class GameManager : Singleton<GameManager>
{

    public bool FirstRun = true;
    public bool FireStarted;
    public string ComPort;
    private GameSettings _settings;

    private FireSystem _fireSystem;
    private FireMeter _fireMeter;
    private Stars _stars;
    private Timer _timer;
    private float _sceneDelay = 8f;
    private int _currentLevel;

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

    public void SceneOne()
    {
        Cursor.visible = false;
        SceneManager.LoadScene("SceneOne");
    }

    public void SceneTwo()
    {
        Cursor.visible = false;
        SceneManager.LoadScene("SceneTwo");
    }

    public void SceneCalibrate()
    {
        SceneManager.LoadScene("Calibrate");
    }

    public void SceneSettings()
    {
        Cursor.visible = true;
        MyInput.Instance.gameObject.SetActive(false);
        SceneManager.LoadScene("Settings");
    }
    // Use this for initialization
    void Start()
    {
        MyInput.Instance.Init();
        if (_currentLevel==0)
        {
            Cursor.visible = false;
        }
        ComPort = PlayerPrefs.GetString("COM");
        Debug.Log("Com port settings:" + ComPort);
        if (string.IsNullOrEmpty(ComPort))
        {
            Debug.Log("Cant load comport settings");
            ComPort = "COM0";
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

    // Update is called once per frame
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
        _fireMeter = ((FireMeter)FindObjectOfType(typeof(FireMeter)));
        _stars = ((Stars)FindObjectOfType(typeof(Stars)));
        _timer = ((Timer)FindObjectOfType(typeof(Timer)));
        _stars.gameObject.SetActive(false);
        _timer.gameObject.SetActive(false);
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
        _fireMeter.Show();
        
        foreach (GameObject trigger in GameObject.FindGameObjectsWithTag("Kid"))
        {
            trigger.GetComponent<Animator>().SetTrigger("Fire");
        }

        FireStarted = true;
    }

    private IEnumerator SceneDelay(float delay, string scene)
    {
        float timer = 0;
        while (timer < delay)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        SceneManager.LoadScene(scene);
    }

    public void EndScene()
    {        
            Debug.Log(_stars);
            _stars.gameObject.SetActive(true);
            Debug.Log("timer" + _timer);
            _stars.Show(_timer.Stars());
            _timer.gameObject.SetActive(false);          /*TODO*/
            _fireSystem.gameObject.SetActive(false);

            string nextScene = "MainMenu";
            var m_Scene = SceneManager.GetActiveScene();
            string currentScene = m_Scene.name;

            if (currentScene == "SceneOne") nextScene = "SceneOnePartTwo";
            if (currentScene == "SceneOne" && _timer.Stars() < 1) nextScene = "SceneOne";
            if (currentScene == "SceneTwo") nextScene = "SceneTwoPartTwo";
            if (currentScene == "SceneTwo" && _timer.Stars() < 1) nextScene = "SceneTwo";
        //if (_timer.Stars() > 0)
        //{
        //    StartCoroutine(SceneDelay(_sceneDelay, nextScene));
        //}
    }

    public void NextScene()
    {
        var m_Scene = SceneManager.GetActiveScene();
        string currentScene = m_Scene.name;
        string nextScene="";
        switch (currentScene)
        {
            case "SceneOne": nextScene = "SceneOnePartTwo";
                break;
            case "SceneTwo": nextScene = "SceneTwoPartTwo";
                break;
            default:nextScene = "MainMenu";
                break;
        }

        SceneManager.LoadScene(nextScene);
    }

    public void Repeat()
    {
        var m_Scene = SceneManager.GetActiveScene();
        string currentScene = m_Scene.name;
        SceneManager.LoadScene(currentScene);
    }
}
