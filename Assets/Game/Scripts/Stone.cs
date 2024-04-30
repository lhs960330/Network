using Photon.Pun;
using UnityEngine;

public class Stone : MonoBehaviourPun
{
    [SerializeField] Rigidbody rigid;

    private void Awake()
    {
        if ( photonView.InstantiationData != null )
        {
            Vector3 force = ( Vector3 )photonView.InstantiationData [0];
            Vector3 torque = ( Vector3 )photonView.InstantiationData [1];

            // 미는 힘 
            rigid.AddForce(force, ForceMode.Impulse);
            // 회전 시키는 힘
            rigid.AddTorque(torque, ForceMode.Impulse);
        }
    }
    private void Update()
    {
        if ( photonView.IsMine == false )
            return;

        if ( transform.position.sqrMagnitude > 40000 )
        {
            // 네트워크에서 같이 지워줘야됨
            PhotonNetwork.Destroy(gameObject);
        }
    }

    private void OnCollisionEnter( Collision collision )
    {
    }
}
