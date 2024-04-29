using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] Rigidbody rigid;
    [SerializeField] float moveSpeed;

    public Vector3 Velocity { get { return rigid.velocity; } }


    private void Awake()
    {
        rigid.velocity = transform.forward * moveSpeed;
        Destroy(rigid, 5f);
    }

}
