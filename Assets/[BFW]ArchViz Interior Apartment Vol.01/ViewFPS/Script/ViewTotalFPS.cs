using UnityEngine;
using UnityEngine.UI;

public class ViewTotalFPS : MonoBehaviour
{
    public Text TheLable;
    private float LastTime;

    private int TheFPS;

    // Use this for initialization
    private void Start()
    {
        TheLable.text = "";
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        TheFPS++;
        if (Time.time > LastTime + 1)
        {
            LastTime = Time.time;
            TheLable.text = "Total Frame:" + TheFPS;
            TheFPS = 0;
        }
    }
}