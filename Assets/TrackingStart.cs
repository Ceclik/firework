using Tracking;
using UnityEngine;

public class TrackingStart : MonoBehaviour {
    private void OnEnable()
    {
        Tracker.Instance.TurnOn();
    }
}
