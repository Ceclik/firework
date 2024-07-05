using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class TvPlay : MonoBehaviour
{
    [SerializeField] private AudioSource tvSound;
    [SerializeField] private Material newMat;
    private readonly List<VideoPlayer> _players = new();

    private Renderer _render;

    // Use this for initialization
    private void Start()
    {
        //GetComponent<VideoPlayer>().Play();
        _render = GetComponent<Renderer>();
        foreach (var player in GetComponents<VideoPlayer>()) _players.Add(player);
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void ChangeMaterial()
    {
        _render.material = newMat;
        foreach (var player in _players) player.Stop();
        StartCoroutine(AudioFade.FadeOut(tvSound, 0.1f));
    }
}