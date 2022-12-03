using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GUIControl : MonoBehaviour
{
    public GameObject GUICanvas;
    bool isOpen = false;
    public TextMeshProUGUI TXTWinningPlayerNick;
    public TextMeshProUGUI TXTWinningPlayerScore;
    public AvatarManager WinningPlayerAvatar;
    public TextMeshProUGUI TXTTie;
    public GameObject GUIHelpPopup;

    public void MenuToggle()
    {
        isOpen = !isOpen;
        if(GUICanvas == null) gameObject.SetActive(isOpen);
        else GUICanvas.SetActive(isOpen);
    }

    public void onContinue()
    {
        isOpen = false;
        if (GUICanvas == null) gameObject.SetActive(isOpen);
        else GUICanvas.SetActive(isOpen);
    }

    public void onQuit()
    {
        NetworkClientManager.Singleton?.Client?.Disconnect();
        NetworkServerManager.Singleton?.Server?.Stop();
        PlayerManager.playerList.Clear();
        SceneManager.LoadScene("Assets/Scenes/MainMenu.unity", LoadSceneMode.Single);
    }

    public void onMultiplayerMenu()
    {
        NetworkClientManager.Singleton?.Client?.Disconnect();
        NetworkServerManager.Singleton?.Server?.Stop();
        PlayerManager.playerList.Clear();
        SceneManager.LoadScene("Assets/Scenes/Multiplayer_Select.unity", LoadSceneMode.Single);
    }

    public void showWinningPlayer()
    {
        MenuToggle();
        if (PlayerManager.winningPlayer != PlayerManager.tiedMatch)
        {
            TXTWinningPlayerNick.text = PlayerManager.playerList[PlayerManager.winningPlayer].username;
            TXTWinningPlayerScore.text = PlayerManager.playerList[PlayerManager.winningPlayer].score + " pts";
            WinningPlayerAvatar.SetPlayerData(PlayerManager.playerList[PlayerManager.winningPlayer]);
        }
        else
        {
            TXTTie.gameObject.SetActive(true);
            int maiorPontuacao = PlayerManager.playerList.OrderByDescending(x => x.Value.score).First().Value.score;
            TXTTie.text = $"Empate!\nMaior pontuação: {maiorPontuacao}";
            TXTWinningPlayerNick.gameObject.SetActive(false);
            TXTWinningPlayerScore.gameObject.SetActive(false);
            WinningPlayerAvatar.gameObject.SetActive(false);
        }
    }

    public void onGUIHelpPopupToggle()
    {
        GUIHelpPopup.gameObject.SetActive(!GUIHelpPopup.gameObject.activeSelf);
    }
}
