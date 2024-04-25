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
        // ���̵� ���� ����ó��
        if(idInputField.text == "" )
        {
            Debug.Log("�̸� ���� ���� �����");
            return;
        }

        PhotonNetwork.LocalPlayer.NickName = idInputField.text;     // ���濡 ������ �� ���̵�
        PhotonNetwork.ConnectUsingSettings();                       // ���� ������ �����ϴ� �Լ�
    }
}