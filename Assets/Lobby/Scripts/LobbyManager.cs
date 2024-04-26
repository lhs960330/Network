using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    // MonoBehaviourPunCallbacks
    // ���� ��������ִ°�?

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
        Debug.Log(state);  // ������ ���¸� �˱����� ��
    }
    public override void OnConnected()
    {
        SetActivePanel(Panel.Menu); // �����ϸ� �޴���
    }

    public override void OnDisconnected( DisconnectCause cause )
    {
        if ( cause == DisconnectCause.ApplicationQuit )
            return;

        SetActivePanel(Panel.Login); // ����� �α�������
    }
    public override void OnCreateRoomFailed( short returnCode, string message )     // �游��⸦ ����������
    {
        Debug.Log($"Creat room failed with Error : {message}({returnCode})");
    }
    public override void OnCreatedRoom()        // �游��⸦ ����������
    {
        Debug.Log("Creat room success");
    }
    public override void OnJoinedRoom()
    {
        SetActivePanel(Panel.Room); // �濡 ����
    }
    public override void OnJoinRandomFailed( short returnCode, string message ) // ���� �����ϴ°� ����������
    {
        Debug.Log($"Join random failed with error : {message}({returnCode})");
    }
    public override void OnLeftRoom() // �濡�� ��������
    {
        SetActivePanel(Panel.Menu);
    }
    public override void OnJoinedLobby()
    {
        SetActivePanel(Panel.Lobby); // �κ�� ����
    }
    public override void OnLeftLobby()
    {
        SetActivePanel(Panel.Menu);     //�κ񿡼� ������
    }
    public override void OnRoomListUpdate( List<RoomInfo> roomList )        // �濡 �ִ� ����Ʈ�� ����Ǹ� ȣ�� ��
    {
        lobbyPanel.UpdateRoomList(roomList);
    }
    public override void OnPlayerEnteredRoom( Player newPlayer )           // �÷��̾ ���ö�
    {
        roomPanel.PlayerEnterRoom(newPlayer);
    }
    public override void OnPlayerLeftRoom( Player otherPlayer )             // �÷��̾ ������
    {
        roomPanel.PlayerLeftRoom(otherPlayer);
    }
    public override void OnMasterClientSwitched( Player newMasterClient )    // �����Ͱ� �ٲ��� �� 
    {
        roomPanel.MasterClientSwitched(newMasterClient);
    }
    public override void OnPlayerPropertiesUpdate( Player targetPlayer, Hashtable changedProps ) // �÷��̾��� ������Ƽ�� �ٲ����� (�� ����)
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
