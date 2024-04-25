using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomPanel : MonoBehaviour
{
    [SerializeField] RectTransform playerContent;
    [SerializeField] PlayerEntry playerEntryPrefab;
    [SerializeField] Button startButton;

    private List<PlayerEntry> PlayerList;

    private void Awake()
    {
        PlayerList = new List<PlayerEntry>();
    }
    private void OnEnable()
    {
        foreach ( Player player in PhotonNetwork.PlayerList )
        {
            PlayerEntry playerEntry = Instantiate(playerEntryPrefab, playerContent);
            playerEntry.SetPlayer(player);
            PlayerList.Add(playerEntry);
        }
        startButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);

    }
    private void OnDisable()
    {
        for ( int i = 0; i < playerContent.childCount; i++ )
        {
            Destroy(playerContent.GetChild(i).gameObject);
        }
        PlayerList.Clear();
    }
    public void StartGame()
    {
    }

    public void PlayerEnterRoom( Player newPlayer )
    {
        PlayerEntry playerEntry = Instantiate(playerEntryPrefab, playerContent);
        playerEntry.SetPlayer(newPlayer);
        PlayerList.Add(playerEntry);
    }

    public void PlayerLeftRoom( Player otherPlayer )
    {
        PlayerEntry playerEntry = null;
        foreach ( PlayerEntry entry in PlayerList )
        {
            if ( entry.Player.ActorNumber == otherPlayer.ActorNumber )
            {
                playerEntry = entry;
            }
        }
        PlayerList.Remove(playerEntry);
        Destroy(playerEntry.gameObject);
    }
    public void MasterClientSwitched( Player newMasterClient )
    {
        startButton.gameObject.SetActive(newMasterClient.IsLocal);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
}
