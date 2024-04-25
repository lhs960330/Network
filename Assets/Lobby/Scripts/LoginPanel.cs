using Photon.Pun;
using TMPro;
using UnityEngine;

public class LoginPanel : MonoBehaviour
{
    [SerializeField] TMP_InputField idInputField;

    private void Start()
    {
        idInputField.text = $"Player {Random.Range(1000, 10000)}";
    }

    public void Login()
    {
        // 아이디가 없을 예외처리
        if(idInputField.text == "" )
        {
            Debug.Log("이름 없어 당장 만드셈");
            return;
        }

        PhotonNetwork.LocalPlayer.NickName = idInputField.text;     // 포톤에 접속할 내 아이디
        PhotonNetwork.ConnectUsingSettings();                       // 포톤 서버에 접속하는 함수
    }
}