using UnityEngine;
using UnityEngine.SceneManagement;

public class GUIControl : MonoBehaviour
{
    bool isOpen = false;

    public void MenuToggle()
    {
        isOpen = !isOpen;
        gameObject.SetActive(isOpen);
    }

    public void onContinue()
    {
        isOpen = false;
        gameObject.SetActive(false);
    }

    public void onQuit()
    {
        NetworkClientManager.Singleton?.Client.Disconnect();
        SceneManager.LoadScene("Assets/Scenes/MainMenu.unity", LoadSceneMode.Single);
    }
}
