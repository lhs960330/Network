using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerEntry : MonoBehaviour
{
    [SerializeField] TMP_Text playerName;
    [SerializeField] TMP_Text playerReady;
    [SerializeField] Button playerReadyButton;

    private Player player;
    public Player Player { get { return player; } }

    public void SetPlayer( Player player )
    {
        this.player = player;
        playerName.text = player.NickName;
        playerReady.text = player.GetReady() ? "Ready" : "";
        playerReadyButton.gameObject.SetActive(player.IsLocal);
    }

    public void Ready()
    {
        bool ready = player.GetReady();
        player.SetReady(!ready);
    }
    public void ChangeCustomProperty( PhotonHashtable property )
    {
        bool ready = player.GetReady();
        playerReady.text = ready ? "Ready" : "";

    }
}
