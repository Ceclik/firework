using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingButtons : MonoBehaviour {
	
	//This script handles repeat and next buttons after completing of the level

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Repeat()
    {
        GameManager.Instance.Repeat();
    }

    public void Next()
    {
        GameManager.Instance.NextScene();
    }
}
