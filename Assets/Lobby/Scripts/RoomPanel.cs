using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

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
        // 방에 들어올때 기준으로 방에 있던 사람들에 대한 목록 갱신
        foreach ( Player player in PhotonNetwork.PlayerList )
        {
            // 프리팹으로 만들어둔 playerEntry생성
            PlayerEntry playerEntry = Instantiate(playerEntryPrefab, playerContent);
            playerEntry.SetPlayer(player);
            PlayerList.Add(playerEntry);
        }
        // 모든 플레이어들이 ready가 된 상황에서만 Start가 되도록한다.
        startButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);  // 마스터클라이언트면 startButton을 활성화 
        PhotonNetwork.LocalPlayer.SetReady(false);
        PhotonNetwork.LocalPlayer.SetLoad(false);
        AllPlayerReadyCheck();
        // 마스터가 씬이 옮겨지면 같이 따라가게해준다.
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    private void OnDisable()
    {
        for ( int i = 0; i < playerContent.childCount; i++ )
        {
            Destroy(playerContent.GetChild(i).gameObject);
        }
        PlayerList.Clear();
        PhotonNetwork.AutomaticallySyncScene = false;
    }
    public void StartGame()
    {
        // 시작하면 셧다내려
        PhotonNetwork.CurrentRoom.IsVisible = false;
        // 서버에서 게임씬으로 보내줘야된다.
        PhotonNetwork.LoadLevel("GameScene");
    }

    public void PlayerEnterRoom( Player newPlayer )
    {
        PlayerEntry playerEntry = Instantiate(playerEntryPrefab, playerContent);
        playerEntry.SetPlayer(newPlayer);
        PlayerList.Add(playerEntry);
        AllPlayerReadyCheck();
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
        AllPlayerReadyCheck();
    }
    public void MasterClientSwitched( Player newMasterClient )
    {
        startButton.gameObject.SetActive(newMasterClient.IsLocal);
        AllPlayerReadyCheck();
    }

    public void PlayerPropertyiesUpdate( Player targetPlayer, PhotonHashtable changedProps )
    {
        PlayerEntry playerEntry = null;
        foreach ( PlayerEntry entry in PlayerList )
        {
            if ( entry.Player.ActorNumber == targetPlayer.ActorNumber )
            {
                playerEntry = entry;
            }
        }
        playerEntry.ChangeCustomProperty(changedProps);

        AllPlayerReadyCheck();
    }
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    // 플레이어들의 Ready체크
    public void AllPlayerReadyCheck()
    {
        // 마스터가 아니면 확인할필요가없다
        if ( PhotonNetwork.IsMasterClient == false )
            return;

        int readyCount = 0;
        // 레디 카운트 재기
        foreach ( Player player in PhotonNetwork.PlayerList )
        {
            if ( player.GetReady() )
            {
                readyCount++;
            }
        }
        // 카운트와 플레이어들의 수가 같은때 StartButton 활성화
        startButton.interactable = readyCount == PhotonNetwork.PlayerList.Length;
    }
}