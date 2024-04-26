using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class LobbyPanel : MonoBehaviour
{
    [SerializeField] RectTransform roomContent;
    [SerializeField] RoomEntry roomEntryPrefab;

    private Dictionary<string, RoomEntry> roomDictionary;

    private void Awake()
    {
        roomDictionary = new Dictionary<string, RoomEntry>();
    }

    private void OnDisable()
    {
        // ���� �� dicrionary�� �������
        for ( int i = 0; i < roomContent.childCount; i++ )
        {
            Destroy(roomContent.GetChild(i).gameObject);
        }
        roomDictionary.Clear();
    }
    public void LeaveLobby()
    {
        // �κ�� ����(��ư����)
        PhotonNetwork.LeaveLobby();
    }

    public void UpdateRoomList( List<RoomInfo> roomlist )
    {
        // ��� ���� Ȯ��
        foreach ( RoomInfo roomInfo in roomlist )
        {
            // 1. ���� ������� ���
            if ( roomInfo.RemovedFromList || roomInfo.IsOpen == false || roomInfo.IsVisible == false )        // 1. ���� ������� ���,2. ���� �����ִ°�� ,3. ���� ������� ��� 
            {
                if ( roomDictionary.ContainsKey(roomInfo.Name) )
                    continue;

                RoomEntry roomEntry = roomDictionary [roomInfo.Name];   // �̸��� ���� ã�´�.
                roomDictionary.Remove(roomInfo.Name);                  // ����� ���� �����Ѵ�.
                Destroy(roomEntry.gameObject);                          // �� ���ӿ�����Ʈ ����
            }
            // 2. ���� ������� �ٲ�� ���
            if ( roomDictionary.ContainsKey(roomInfo.Name) )            // ���� �ִ� �� 
            {
                RoomEntry roomEntry = roomDictionary [roomInfo.Name];
                roomEntry.SetRoomInfo(roomInfo);
            }
            // 3. ���� ����� ���
            else                                                        // ������ ��
            {
                RoomEntry roomEntry = Instantiate(roomEntryPrefab, roomContent);
                roomEntry.SetRoomInfo(roomInfo);
                roomDictionary.Add(roomInfo.Name, roomEntry);
            }
        }
    }
}
