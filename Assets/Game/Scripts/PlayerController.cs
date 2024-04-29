using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviourPun, IPunObservable // IPunObservable 이 인터페이스가 있으면 PhotonView에 Observed에 들어갈수있음
{
    [SerializeField] Rigidbody rigid;
    [SerializeField] List<Color> colorList;

    [SerializeField] PlayerInput input;

    [SerializeField] Bullet bulletPrefab;

    [SerializeField] float moveSpeed;
    [SerializeField] float rotateSpeed;

    [SerializeField] float movePower;
    [SerializeField] float maxSpeed;
    [SerializeField] int fireCount;
    [SerializeField] float fireCoolTime;

    private Vector3 moveDir;
    private float currentSpeed;
    private float lastFireTime = float.MinValue;

    private void Awake()
    {
        // 내 캐릭터가 아니면 못움직이에 PlayerInput를 삭제
        if ( photonView.IsMine == false )
        {
            Destroy(input);
        }
        SetPlayerColor();

    }
    private void Update()
    {
        Accelate();
        Rotate();
        //transform.Translate(moveDir * moveSpeed * Time.deltaTime);
    }

    private void Accelate()
    {
        rigid.AddForce(moveDir.z * transform.forward * movePower, ForceMode.Force);
        if ( rigid.velocity.magnitude > maxSpeed )
        {
            rigid.velocity = rigid.velocity.normalized * maxSpeed;
        }
        currentSpeed = rigid.velocity.magnitude;
    }

    private void Rotate()
    {
        transform.Rotate(Vector3.up, moveDir.x * rotateSpeed * Time.deltaTime);
    }

    private void OnMove( InputValue value )
    {
        moveDir.x = value.Get<Vector2>().x;
        moveDir.z = value.Get<Vector2>().y;
    }

    private void OnFire( InputValue value )
    {
        // 아이디가 필요하니 포톤뷰를 기준으로 RPC함수를 호출
        photonView.RPC("RequestCreateBullet", RpcTarget.MasterClient);
    }

    [PunRPC]
    private void RequestCreateBullet()
    {
        if ( Time.time < lastFireTime + fireCoolTime )
            return;

        Debug.Log("요청");
        lastFireTime =Time.time;
        photonView.RPC("ResultCreateBullet", RpcTarget.AllViaServer, transform.position, transform.rotation);
    }
    // PunRPC은 함수를 원격으로 호출할수 있게 해줌 (이름이 같으면 안됨)
    // RPC는 그럼 네트워크한테 데이터를 바로주나?
    // 지연보상 방법
    [PunRPC]
    private void ResultCreateBullet(Vector3 position, Quaternion rotation, PhotonMessageInfo info)
    {
        float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));

        fireCount++;
        Bullet bullet = Instantiate(bulletPrefab, position, rotation);
        bullet.transform.position += bullet.Velocity * lag;
        //bullet.ApplyLay(lag);
        //PhotonNetwork.Instantiate(bulletPrefab.name, position, rotation);
    }
    private void SetPlayerColor()
    {
        int playerNumber = photonView.Owner.GetPlayerNumber();
        if ( colorList == null || colorList.Count <= playerNumber )
            return;
        Renderer render = GetComponent<Renderer>();
        render.material.color = colorList [playerNumber];

        if ( photonView.IsMine )
        {
            render.material.color = Color.green;
        }
    }

    // 얘로 내가 상대방에게 보내고 싶은 데이터들을 써주면됨
    public void OnPhotonSerializeView( PhotonStream stream, PhotonMessageInfo info )
    {
        // 참조들은 전달을 못함
        // Rigidbody rigid = PhotonView.Find(photonView.ViewID).GetComponent<Rigidbody>(); // 포톤뷰의 아이디를 찾아서 그 리지드바디를 가져와서 보내줄수있다.
        
        // SendNext 순서로 ReceiveNext도 같은 순서로 해야된다.
        // 안하면 순서가 바뀌게 되어 받을떄 같은 자료형이면 1에 있던 데이터가 2에서 표현될수도있고
        // 다른 자료형이면 오류가 날수도 있다.
        if ( stream.IsWriting )             // 다른곳을 보낼때
        {
            stream.SendNext(fireCount);
            stream.SendNext(currentSpeed);
        }
        else //stream.IsReading 다른곳에 받을때
        {
            fireCount = ( int )stream.ReceiveNext();
            currentSpeed = ( float )stream.ReceiveNext();
        }
    }
}