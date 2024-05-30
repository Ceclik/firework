using System;
using UnityEngine;
using UnityEngine.UI;

namespace FireAimScripts
{
    public class HeartsHandler : MonoBehaviour
    {
        [SerializeField] private RectTransform heartsParent;
        [SerializeField] private Sprite emptyHeart;

        [SerializeField] private FireSplitter[] splitterSystems;

        private int _heartIndex = 2;
        private Image[] _hearts;

        private void EmptyHeart()
        {
            if (_heartIndex >= 0)
            {
                _hearts[_heartIndex].sprite = emptyHeart;
                _heartIndex--;
            }
        }

        private void Start()
        {
            _hearts = new Image[heartsParent.childCount];
            for (int i = 0; i < heartsParent.childCount; i++)
                _hearts[i] = heartsParent.GetChild(i).GetComponent<Image>();

            foreach (var splitter in splitterSystems)
                splitter.OnFireSplitted += EmptyHeart;
        }

        private void OnDestroy()
        {
            foreach (var splitter in splitterSystems)
                splitter.OnFireSplitted -= EmptyHeart;
        }
    }
}
