using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using TMPro;
using UnityEngine;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text infoText;
    [SerializeField] float countDownTime;

    private void Start()
    {
        PhotonNetwork.LocalPlayer.SetLoad(true);

    }

    public override void OnPlayerPropertiesUpdate( Player targetPlayer, PhotonHashtable changedProps )
    {
        if ( changedProps.ContainsKey(CustomProperty.LOAD) )
        {

            if ( PlayerLoadCount() == PhotonNetwork.PlayerList.Length )
            {
                // ���� ����
                // �������϶� ���� ������ ������
                if ( PhotonNetwork.IsMasterClient )
                {
                    PhotonNetwork.CurrentRoom.SetGameStart(true);

                    PhotonNetwork.CurrentRoom.SetGameStartTime(PhotonNetwork.Time);
                }
            }
            else
            {
                // �ٸ� �÷��̾� �ε� ��ٸ���
                infoText.text = $"{PlayerLoadCount()}/{PhotonNetwork.PlayerList.Length}";
            }
        }
    }

    public override void OnRoomPropertiesUpdate( PhotonHashtable propertiesThatChanged )
    {
        if ( propertiesThatChanged.ContainsKey(CustomProperty.GAMESTART) )
        {
            StartCoroutine(StartTiem());
        }
    }

    IEnumerator StartTiem()
    {
        double LoadTime = PhotonNetwork.CurrentRoom.GetGameStartTimne();
        while ( PhotonNetwork.Time - LoadTime < countDownTime )
        {
            int remainTime = ( int )( countDownTime - ( PhotonNetwork.Time - LoadTime ) );
            infoText.text = ( remainTime + 1 ).ToString();
            yield return null;
        }

        infoText.text = "Game Start!";
        StartCoroutine(GameStartDelay());
        yield return new WaitForSeconds(3);

        infoText.text = "";

    }

    private int PlayerLoadCount()
    {
        int loadCount = 0;
        foreach ( Player player in PhotonNetwork.PlayerList )
        {
            if ( player.GetLoad() )
            {
                loadCount++;
            }
        }
        return loadCount;
    }

    IEnumerator GameStartDelay()
    {
        yield return new WaitForSeconds(1);
        // ����ٰ� �׽�Ʈ�� ������ �־��ָ�� 
        GameStart();
    }

    public void GameStart()
    {
        // ���� ��Ʈ��ũ�� �÷��̾ ��������
        Vector2 pos = Random.insideUnitCircle * 30;
        PhotonNetwork.Instantiate("Player", new Vector3(pos.x, 0, pos.y), Quaternion.identity);
    }
}
