using System.IO;
using System.Net.Sockets;
using TMPro;
using UnityEngine;

public class Client : MonoBehaviour
{
    [SerializeField] Chat chat;

    [SerializeField] TMP_InputField nameField;
    [SerializeField] TMP_InputField ipField;
    [SerializeField] TMP_InputField portField;

    private string clientName;   // � �̸����� �����ϰ� �ִ���
    private string ip;           // � Ip����
    private int port;            // � ��Ʈ����

    public bool IsConnect { get; private set; } = false;

    private TcpClient client;       // ��������� �����ϱ����� Ŭ����
    private NetworkStream stream;   // ������ �ްų� ������ ���� ���۸� �غ��ϴµ� �ش� ���۸� ���·� �� �аų�, ������ ���� Ŭ����
    private StreamWriter writer;    // ���� ���·� ��Ƶд�? (������)
    private StreamReader reader;    // �ѹ��� �б����� Ŭ���� (�б�)
    
    /* ����
     * �����͸� �Ѱ����� �Ͻ������� �����ϴ� �޸��� ����
     * 
     * ��뿹��
     * A�� �����͸� 1�ʴ� 3�� ������ �ְ� B�� 1�ʴ� 100���� �������ִµ�
     * A���� B�� �ٷ� ������ B�� ȿ�������� ���� ���۸� ����Ͽ�? A�� �����͵��� ��Ƽ� �ѹ��� B���� �����µ� ���ȴ�?
     */
    private void Update()
    {
        if ( IsConnect && stream.DataAvailable )
        {
            string chat = reader.ReadLine();
            if ( chat != null )
                ReceiveChat(chat);
        }
    }

    private void OnDisable()
    {
        //�������� ����
        if ( IsConnect )
            DisConnect();
    }

    public void Connect()
    {
        if ( IsConnect )
            return;

        AddMessage("Connect try");

        if ( nameField.text == "" )
        {
            clientName = $"Client {Random.Range(1000, 10000)}";
            nameField.text = clientName;
        }
        else
        {
            clientName = nameField.text;
        }
        ip = ipField.text == "" ? "127.0.0.1" : ipField.text; // 127.0.0.1 ���� ��ǻ���� ip
        port = portField.text == "" ? 5555 : int.Parse(portField.text);

        try
        {
            client = new TcpClient(ip, port);
            stream = client.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);

            Debug.Log("Connect success");
            IsConnect = true;
            nameField.interactable = false;
            ipField.interactable = false;
            portField.interactable = false;
        }
        catch ( System.Exception e )
        {
            AddMessage($"Connect fail : {e.Message}");
            DisConnect();
        }
    }

    public void DisConnect()
    {
        writer?.Close();
        writer = null;
        reader?.Close();
        reader = null;
        stream?.Close();
        stream = null;
        client?.Close();
        client = null;
        IsConnect = false;
        nameField.interactable = true;
        ipField.interactable = true;
        portField.interactable = true;
        AddMessage("DisConnected");
    }

    public void SendChat( string chatText )
    {
        if ( !IsConnect )
        {
            AddMessage("Client is not connected");
            return;
        }

        try
        {
            writer.WriteLine($"{clientName} : {chatText}");
            writer.Flush(); // ��������ֱ�
        }
        catch ( System.Exception e )
        {
            AddMessage($"Send chat Fail {e.Message}");
        }

    }

    public void ReceiveChat( string chatText )
    {
        Debug.Log($"[ReceiveChat] {chatText}");
        chat.AddMessage(chatText);
    }

    private void AddMessage( string message )
    {
        Debug.Log($"[Client] {message}");
        chat.AddMessage($"[Client] {message}");
    }
}   