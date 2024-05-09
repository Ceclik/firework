using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//button handler methods
public class MenuActions : MonoBehaviour {

	public void MenuExit()
    {
        Application.Quit();
    }

    public void SceneOne()
    {
        Debug.Log("starting scene one");
        FindObjectOfType<GameManager>().SceneOne(); //its better to use FindFirstObjectOfType<>();
    }

    public void SceneTwo()
    {
        FindObjectOfType<GameManager>().SceneTwo();
    }

    public void Calibrate()
    {
        FindObjectOfType<GameManager>().SceneSettings();
    }
}
