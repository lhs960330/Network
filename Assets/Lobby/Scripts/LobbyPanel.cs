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
        // 나갈 때 dicrionary를 비워주자
        for ( int i = 0; i < roomContent.childCount; i++ )
        {
            Destroy(roomContent.GetChild(i).gameObject);
        }
        roomDictionary.Clear();
    }
    public void LeaveLobby()
    {
        // 로비로 가기(버튼으로)
        PhotonNetwork.LeaveLobby();
    }

    public void UpdateRoomList( List<RoomInfo> roomlist )
    {
        // 모든 방을 확인
        foreach ( RoomInfo roomInfo in roomlist )
        {
            // 1. 방이 사라지는 경우
            if ( roomInfo.RemovedFromList || roomInfo.IsOpen == false || roomInfo.IsVisible == false )        // 1. 방이 사라지는 경우,2. 방이 닫혀있는경우 ,3. 방이 비공개인 경우 
            {
                if ( roomDictionary.ContainsKey(roomInfo.Name) )
                    continue;

                RoomEntry roomEntry = roomDictionary [roomInfo.Name];   // 이름을 통해 찾는다.
                roomDictionary.Remove(roomInfo.Name);                  // 사라진 방을 삭제한다.
                Destroy(roomEntry.gameObject);                          // 방 게임오브젝트 삭제
            }
            // 2. 방이 내용뮬이 바뀌는 경우
            if ( roomDictionary.ContainsKey(roomInfo.Name) )            // 원래 있던 방 
            {
                RoomEntry roomEntry = roomDictionary [roomInfo.Name];
                roomEntry.SetRoomInfo(roomInfo);
            }
            // 3. 방이 생기는 경우
            else                                                        // 없었던 방
            {
                RoomEntry roomEntry = Instantiate(roomEntryPrefab, roomContent);
                roomEntry.SetRoomInfo(roomInfo);
                roomDictionary.Add(roomInfo.Name, roomEntry);
            }
        }
    }
}
