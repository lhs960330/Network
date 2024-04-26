using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugGaManager : MonoBehaviourPunCallbacks
{
    [SerializeField] string DebugRoomName;
    private void Start()
    {
        PhotonNetwork.LocalPlayer.NickName = $"TestPlayer {Random.Range(1000, 10000)}";
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 8;
        options.IsVisible = false;
        TypedLobby typedLobby = new TypedLobby("DebugLooby", LobbyType.Default);

        PhotonNetwork.JoinOrCreateRoom(DebugRoomName, options, typedLobby);
    }

    public override void OnJoinedRoom()
    {
        StartCoroutine(GameStartDelay());
    }

    IEnumerator GameStartDelay()
    {
        yield return new WaitForSeconds(1);
        // 여기다가 테스트용 데이터 넣어주면됨 
        GameStart();
    }

    public void GameStart()
    {
        // 포톤 네트워크에 플레이어를 만들어야함
        Vector2 pos = Random.insideUnitCircle * 30;
        PhotonNetwork.Instantiate("Player", new Vector3(pos.x, 0, pos.y), Quaternion.identity);
    }
}