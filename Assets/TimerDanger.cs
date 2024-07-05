using UnityEngine;

[RequireComponent(typeof(Timer))]
public class TimerDanger : MonoBehaviour
{
    public GameObject danger;

    private Timer _timer;

    // Use this for initialization
    private void Start()
    {
        _timer = GetComponent<Timer>();
        danger.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        if (_timer.value < 15) danger.SetActive(true);
        if (_timer.value <= 0 || !_timer.gameObject.activeSelf) danger.SetActive(false);
    }

    private void OnDisable()
    {
        danger.SetActive(false);
    }
}