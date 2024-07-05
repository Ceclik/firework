using System.Collections;
using UnityEngine;

public class AnimatedCookieTexture : MonoBehaviour
{
    public enum AnimMode
    {
        forwards,
        backwards,
        random
    }

    public Texture2D[] textures;
    public float fps = 15;

    public AnimMode animMode = AnimMode.forwards;
    private Light cLight;

    private int frameNr;

    private void Start()
    {
        cLight = GetComponent(typeof(Light)) as Light;
        if (cLight == null)
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
            cLight.cookie = textures[frameNr];

            yield return new WaitForSeconds(1.0f / fps);
            switch (animMode)
            {
                case AnimMode.forwards:
                    frameNr++;
                    if (frameNr >= textures.Length) frameNr = 0;
                    break;
                case AnimMode.backwards:
                    frameNr--;
                    if (frameNr < 0) frameNr = textures.Length - 1;
                    break;
                case AnimMode.random:
                    frameNr = Random.Range(0, textures.Length);
                    break;
            }
        }
    }
}