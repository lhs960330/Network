using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviourPun, IPunObservable // IPunObservable �� �������̽��� ������ PhotonView�� Observed�� ��������
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
        // �� ĳ���Ͱ� �ƴϸ� �������̿� PlayerInput�� ����
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
        // ���̵� �ʿ��ϴ� ����並 �������� RPC�Լ��� ȣ��
        photonView.RPC("RequestCreateBullet", RpcTarget.MasterClient);
    }

    [PunRPC]
    private void RequestCreateBullet()
    {
        if ( Time.time < lastFireTime + fireCoolTime )
            return;

        Debug.Log("��û");
        lastFireTime =Time.time;
        photonView.RPC("ResultCreateBullet", RpcTarget.AllViaServer, transform.position, transform.rotation);
    }
    // PunRPC�� �Լ��� �������� ȣ���Ҽ� �ְ� ���� (�̸��� ������ �ȵ�)
    // RPC�� �׷� ��Ʈ��ũ���� �����͸� �ٷ��ֳ�?
    // �������� ���
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

    // ��� ���� ���濡�� ������ ���� �����͵��� ���ָ��
    public void OnPhotonSerializeView( PhotonStream stream, PhotonMessageInfo info )
    {
        // �������� ������ ����
        // Rigidbody rigid = PhotonView.Find(photonView.ViewID).GetComponent<Rigidbody>(); // ������� ���̵� ã�Ƽ� �� ������ٵ� �����ͼ� �����ټ��ִ�.
        
        // SendNext ������ ReceiveNext�� ���� ������ �ؾߵȴ�.
        // ���ϸ� ������ �ٲ�� �Ǿ� ������ ���� �ڷ����̸� 1�� �ִ� �����Ͱ� 2���� ǥ���ɼ����ְ�
        // �ٸ� �ڷ����̸� ������ ������ �ִ�.
        if ( stream.IsWriting )             // �ٸ����� ������
        {
            stream.SendNext(fireCount);
            stream.SendNext(currentSpeed);
        }
        else //stream.IsReading �ٸ����� ������
        {
            fireCount = ( int )stream.ReceiveNext();
            currentSpeed = ( float )stream.ReceiveNext();
        }
    }
}