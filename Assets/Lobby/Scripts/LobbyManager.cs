using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    // MonoBehaviourPunCallbacks
    // 포톤 연결시켜주는거?

    public enum Panel { Login, Menu, Lobby, Room }

    [SerializeField] LoginPanel loginPanel;
    [SerializeField] MainPanel menuPanel;
    [SerializeField] RoomPanel roomPanel;
    [SerializeField] LobbyPanel lobbyPanel;

    private ClientState state;

    private void Update()
    {
        ClientState curState = PhotonNetwork.NetworkClientState;
        if ( state == curState )
            return;

        state = curState;
        Debug.Log(state);  // 포톤의 상태를 알기위해 씀
    }
    public override void OnConnected()
    {
        SetActivePanel(Panel.Menu); // 접속하면 메뉴로
    }

    public override void OnDisconnected( DisconnectCause cause )
    {
        if ( cause == DisconnectCause.ApplicationQuit )
            return;

        SetActivePanel(Panel.Login); // 끊기면 로그인으로
    }
    public override void OnCreateRoomFailed( short returnCode, string message )     // 방만들기를 실패했을떄
    {
        Debug.Log($"Creat room failed with Error : {message}({returnCode})");
    }
    public override void OnCreatedRoom()        // 방만들기를 성공했을때
    {
        Debug.Log("Creat room success");
    }
    public override void OnJoinedRoom()
    {
        SetActivePanel(Panel.Room); // 방에 들어갈때
    }
    public override void OnJoinRandomFailed( short returnCode, string message ) // 방을 접속하는게 실패했을때
    {
        Debug.Log($"Join random failed with error : {message}({returnCode})");
    }
    public override void OnLeftRoom() // 방에서 나갔을때
    {
        SetActivePanel(Panel.Menu);
    }
    public override void OnJoinedLobby()
    {
        SetActivePanel(Panel.Lobby); // 로비로 갈때
    }
    public override void OnLeftLobby()
    {
        SetActivePanel(Panel.Menu);     //로비에서 나갈때
    }
    public override void OnRoomListUpdate( List<RoomInfo> roomList )        // 방에 있는 리스트가 변경되면 호출 됨
    {
        lobbyPanel.UpdateRoomList(roomList);
    }
    public override void OnPlayerEnteredRoom( Player newPlayer )           // 플레이어가 들어올때
    {
        roomPanel.PlayerEnterRoom(newPlayer);
    }
    public override void OnPlayerLeftRoom( Player otherPlayer )             // 플레이어가 나갈때
    {
        roomPanel.PlayerLeftRoom(otherPlayer);
    }
    public override void OnMasterClientSwitched( Player newMasterClient )    // 마스터가 바꼈을 때 
    {
        roomPanel.MasterClientSwitched(newMasterClient);
    }
    public override void OnPlayerPropertiesUpdate( Player targetPlayer, Hashtable changedProps ) // 플레이어의 프로퍼티가 바꼈을때 (좀 느림)
    {
        roomPanel.PlayerPropertyiesUpdate(targetPlayer, changedProps);
    }
    private void Start()
    {
        SetActivePanel(Panel.Login);
    }

    private void SetActivePanel( Panel panel )
    {
        loginPanel.gameObject.SetActive(panel == Panel.Login);
        menuPanel.gameObject.SetActive(panel == Panel.Menu);
        roomPanel.gameObject.SetActive(panel == Panel.Room);
        lobbyPanel.gameObject.SetActive(panel == Panel.Lobby);
    }
}
