using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlick : MonoBehaviour {

    public float flickSpeed = 0.1f;
    public float maxIntens = 2.0f;
    public float minIntens = 0.2f;
    public int maxRnd = 2;

    private Light _light;
    private bool _napr;
    
	// Use this for initialization
	void Start () {
        _light = GetComponent<Light>();
	}
	
	// Update is called once per frame
	void Update () {
        if (_napr) _light.intensity += flickSpeed;
        else _light.intensity -= flickSpeed;

        _light.intensity = Mathf.Clamp(_light.intensity, minIntens, maxIntens);

        var rnd = Random.Range(0, maxRnd);
        _napr = rnd == 0;

        if (_light.intensity == minIntens) _napr = true;
        if (_light.intensity == maxIntens) _napr = false;
	}
}
