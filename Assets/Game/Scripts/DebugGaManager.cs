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
        // ����ٰ� �׽�Ʈ�� ������ �־��ָ�� 
        GameStart();
    }

    public void GameStart()
    {
        // ���� ��Ʈ��ũ�� �÷��̾ ��������(����� Resources�� ����Ͽ� �����;ߵ�, ���� ����� ������ �־�ߵ�) = �׷� ���͵鵵 ���� �並 ��� �־�ߵǳ�?
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
                // InstantiateRoomObject�� �� ���� ������Ʈ�̴�.(�������� ���� �����Ͱ� ������ ���� ���忡�� �������� �����ǵ� ������Ʈ�� ������� �ʴ´�.)
                // Instantiate�� ������ �������� ����־� ���� ���忡�� �����Ǹ� ������Ʈ�� ������� �ȴ�.
                // ���������� ����� �ʿ䰡 �ִ� ������Ʈ�鿡 Ȱ����
                PhotonNetwork.InstantiateRoomObject("LargeStone", pos, Random.rotation, 0, instantiateDate);
            }
            else
            {
                PhotonNetwork.InstantiateRoomObject("SmallStone", pos, Random.rotation, 0, instantiateDate);
            }
        }
    }
}