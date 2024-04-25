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
        int maxPlayer = maxPlayerInputField.text == "" ? 8 : int.Parse(maxPlayerInputField.text);    // ����־����� 8�� ����
        maxPlayer = Mathf.Clamp(maxPlayer, 1, 8);       // �ο��� ����

        RoomOptions options = new RoomOptions();
        options.MaxPlayers = maxPlayer;                 // room�ɼǿ� �ο��� maxPlayer�� ����
        PhotonNetwork.CreateRoom(roomName, options);     // ���� ������
    }

    public void CreateRoomCancel()
    {
        createRoomPanel.SetActive(false);       // ���� ���ٶ� UI����
    }

    public void RandomMatching()
    {
        //PhotonNetwork.JoinRandomRoom();           // ������ ������ �������� �� // ����ִ� �� ã��, ������ ����

        string Name = $"Room {Random.Range(1000, 10000)}";      // �������� �� �̸� ���ϱ�
        RoomOptions options = new RoomOptions() { MaxPlayers = 8 };     // �� �ɼǿ� �ο��� 8������ �����Ѵ�.
        PhotonNetwork.JoinRandomOrCreateRoom(roomName : Name, roomOptions : options);     // ���� ������ ���� �ϳ� ������ش�.
    }

    public void JoinLobby()
    {
        PhotonNetwork.JoinLobby();      // ���� ã�Ƽ� ����
    }

    public void Logout()
    {
        PhotonNetwork.Disconnect();     // ���� ���� 
    }
}
