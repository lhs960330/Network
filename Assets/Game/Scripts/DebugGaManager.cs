using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;

public class DebugGaManager : MonoBehaviourPunCallbacks
{
    [SerializeField] string DebugRoomName;
    [SerializeField] float spawnStoneTime = 3f;

    private void Start()
    {
        PhotonNetwork.LocalPlayer.NickName = $"TestPlayer {Random.Range(1000, 10000)}";
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnMasterClientSwitched( Player newMasterClient )
    {
        if ( newMasterClient.IsLocal )
        {
            StartCoroutine(SpawnStoneRoutine());
        }
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
        // 포톤 네트워크에 플레이어를 만들어야함(포톤는 Resources를 사용하여 가져와야됨, 포톤 뷰또한 가지고 있어야됨) = 그럼 몬스터들도 포톤 뷰를 들고 있어야되나?
        Vector2 pos = Random.insideUnitCircle * 30;
        PhotonNetwork.Instantiate("Player", new Vector3(pos.x, 0, pos.y), Quaternion.identity);
        if ( PhotonNetwork.IsMasterClient )
            spawnStoneRoutine = StartCoroutine(SpawnStoneRoutine());

    }

    Coroutine spawnStoneRoutine;

    IEnumerator SpawnStoneRoutine()
    {
        while ( true )
        {
            yield return new WaitForSeconds(spawnStoneTime);

            Vector2 direction = Random.insideUnitCircle.normalized;
            Vector3 pos = new Vector3(direction.x, 0, direction.y) * 200f;

            Vector3 force = -pos.normalized * 30f + new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f));
            Vector3 torque = Random.insideUnitSphere * Random.Range(1f, 3f);
            object [] instantiateDate = { force, torque };

            if ( Random.Range(0, 2) < 1 )
            {
                // InstantiateRoomObject는 방 전용 오브젝트이다.(소유권을 가진 마스터가 나가고 다음 방장에게 소유권이 이전되도 오브젝트가 사라지지 않는다.)
                // Instantiate는 방장이 소유권을 들고있어 다음 방장에게 이전되면 오브젝트가 사라지게 된다.
                // 공통적으로 사용할 필요가 있는 오브젝트들에 활용함
                PhotonNetwork.InstantiateRoomObject("LargeStone", pos, Random.rotation, 0, instantiateDate);
            }
            else
            {
                PhotonNetwork.InstantiateRoomObject("SmallStone", pos, Random.rotation, 0, instantiateDate);
            }
        }
    }
}