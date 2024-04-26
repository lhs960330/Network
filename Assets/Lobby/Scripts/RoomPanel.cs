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
        // �濡 ���ö� �������� �濡 �ִ� ����鿡 ���� ��� ����
        foreach ( Player player in PhotonNetwork.PlayerList )
        {
            // ���������� ������ playerEntry����
            PlayerEntry playerEntry = Instantiate(playerEntryPrefab, playerContent);
            playerEntry.SetPlayer(player);
            PlayerList.Add(playerEntry);
        }
        // ��� �÷��̾���� ready�� �� ��Ȳ������ Start�� �ǵ����Ѵ�.
        startButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);  // ������Ŭ���̾�Ʈ�� startButton�� Ȱ��ȭ 
        PhotonNetwork.LocalPlayer.SetReady(false);
        PhotonNetwork.LocalPlayer.SetLoad(false);
        AllPlayerReadyCheck();
        // �����Ͱ� ���� �Ű����� ���� ���󰡰����ش�.
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
        // �����ϸ� �˴ٳ���
        PhotonNetwork.CurrentRoom.IsVisible = false;
        // �������� ���Ӿ����� ������ߵȴ�.
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

    // �÷��̾���� Readyüũ
    public void AllPlayerReadyCheck()
    {
        // �����Ͱ� �ƴϸ� Ȯ�����ʿ䰡����
        if ( PhotonNetwork.IsMasterClient == false )
            return;

        int readyCount = 0;
        // ���� ī��Ʈ ���
        foreach ( Player player in PhotonNetwork.PlayerList )
        {
            if ( player.GetReady() )
            {
                readyCount++;
            }
        }
        // ī��Ʈ�� �÷��̾���� ���� ������ StartButton Ȱ��ȭ
        startButton.interactable = readyCount == PhotonNetwork.PlayerList.Length;
    }
}