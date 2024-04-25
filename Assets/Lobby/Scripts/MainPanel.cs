using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class MainPanel : MonoBehaviour
{
    [SerializeField] GameObject menuPanel;
    [SerializeField] GameObject createRoomPanel;
    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] TMP_InputField maxPlayerInputField;

    private void OnEnable()
    {
        createRoomPanel.SetActive(false);
    }
    public void CreateRoomMenu()
    {
        createRoomPanel.SetActive(true);
    }

    public void CreateRoomConfirm()
    {
        string roomName = roomNameInputField.text;
        if ( roomName == "" )
        {
            roomName = $"Room {Random.Range(1000, 100000)}";
        }
        int maxPlayer = maxPlayerInputField.text == "" ? 8 : int.Parse(maxPlayerInputField.text);    // 비워있었을때 8로 설정
        maxPlayer = Mathf.Clamp(maxPlayer, 1, 8);       // 인원수 제한

        RoomOptions options = new RoomOptions();
        options.MaxPlayers = maxPlayer;                 // room옵션에 인원을 maxPlayer로 설정
        PhotonNetwork.CreateRoom(roomName, options);     // 방을 생성함
    }

    public void CreateRoomCancel()
    {
        createRoomPanel.SetActive(false);       // 방을 없앨때 UI꺼줌
    }

    public void RandomMatching()
    {
        //PhotonNetwork.JoinRandomRoom();           // 생성된 방으로 랜덤으로 들어감 // 비어있는 방 찾기, 없으면 못들어감

        string Name = $"Room {Random.Range(1000, 10000)}";      // 랜덤으로 방 이름 정하기
        RoomOptions options = new RoomOptions() { MaxPlayers = 8 };     // 방 옵션에 인원을 8명으로 제한한다.
        PhotonNetwork.JoinRandomOrCreateRoom(roomName : Name, roomOptions : options);     // 방이 없으면 방을 하나 만들어준다.
    }

    public void JoinLobby()
    {
        PhotonNetwork.JoinLobby();      // 방을 찾아서 들어갈때
    }

    public void Logout()
    {
        PhotonNetwork.Disconnect();     // 접속 종료 
    }
}
