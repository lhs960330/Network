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

    private string clientName;   // 어떤 이름으로 진행하고 있는지
    private string ip;           // 어떤 Ip인지
    private int port;            // 어떤 포트인지

    public bool IsConnect { get; private set; } = false;

    private TcpClient client;       // 소켓통신을 제어하기위한 클래스
    private NetworkStream stream;   // 소켓의 받거나 보내기 위한 버퍼를 준비하는데 해당 버퍼를 형태로 쭉 읽거나, 보내기 위한 클래스
    private StreamWriter writer;    // 버퍼 형태로 담아둔다? (보내기)
    private StreamReader reader;    // 한번에 읽기위한 클래스 (읽기)
    
    /* 버퍼
     * 데이터를 한곳에서 일시적으로 보관하는 메모리의 영역
     * 
     * 사용예시
     * A가 데이터를 1초당 3개 보낼수 있고 B가 1초당 100개를 받을수있는데
     * A에서 B로 바로 보내면 B가 효율적이지 못해 버퍼를 사용하여? A에 데이터들을 모아서 한번에 B에게 보내는데 사용된다?
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
        //없어질때 삭제
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
        ip = ipField.text == "" ? "127.0.0.1" : ipField.text; // 127.0.0.1 현재 컴퓨터의 ip
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
            writer.Flush(); // 지꺼기없애기
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