using UnityEngine;

public class EndingButtons : MonoBehaviour
{
    public void OnBackToMenuButtonClick()
    {
        GameManager.Instance.SceneMainMenu();
    }
}