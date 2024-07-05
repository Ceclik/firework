using System.Collections;
using UnityEngine;

public class AnimatedCookieTexture : MonoBehaviour
{
    public enum AnimMode
    {
        Forwards,
        Backwards,
        Random
    }

    public Texture2D[] textures;
    public float fps = 15;

    public AnimMode animMode = AnimMode.Forwards;
    private Light _cLight;

    private int _frameNr;

    private void Start()
    {
        _cLight = GetComponent(typeof(Light)) as Light;
        if (_cLight == null)
        {
            UnityEngine.Debug.LogWarning("AnimateCookieTexture: No light found on this gameObject", this);
            enabled = false;
        }

        StartCoroutine("switchCookie");
    }


    private IEnumerator switchCookie()
    {
        while (true)
        {
            _cLight.cookie = textures[_frameNr];

            yield return new WaitForSeconds(1.0f / fps);
            switch (animMode)
            {
                case AnimMode.Forwards:
                    _frameNr++;
                    if (_frameNr >= textures.Length) _frameNr = 0;
                    break;
                case AnimMode.Backwards:
                    _frameNr--;
                    if (_frameNr < 0) _frameNr = textures.Length - 1;
                    break;
                case AnimMode.Random:
                    _frameNr = Random.Range(0, textures.Length);
                    break;
            }
        }
    }
}