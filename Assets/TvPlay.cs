using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class TvPlay : MonoBehaviour
{
    public AudioSource tvsound;
    public Material newMat;
    private readonly List<VideoPlayer> players = new();

    private Renderer render;

    // Use this for initialization
    private void Start()
    {
        //GetComponent<VideoPlayer>().Play();
        render = GetComponent<Renderer>();
        foreach (var player in GetComponents<VideoPlayer>()) players.Add(player);
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void ChangeMaterial()
    {
        render.material = newMat;
        foreach (var player in players) player.Stop();
        StartCoroutine(AudioFade.FadeOut(tvsound, 0.1f));
    }
}