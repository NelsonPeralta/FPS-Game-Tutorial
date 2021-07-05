using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SerializeTest : MonoBehaviourPunCallbacks, IPunObservable
{
    Vector3 realPosition;
    Quaternion realRotation;

    [SerializeField] GameObject cameraHolder;
    public Camera cam;

    float verticalLookRotation;
    [SerializeField] float mouseSensitivity, sprintSpeed, walkSpeed, smoothTime;

    Vector3 smoothMoveVelocity;
    Vector3 moveAmount;

    ObjectPool BulletPool;

    void Start()
    {
        BulletPool = GameObject.FindGameObjectWithTag("BulletPool").GetComponent<ObjectPool>();
        if (photonView.IsMine)
        {

        }
        else
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
        }
    }

    private void Update()
    {
        if (photonView.IsMine)
        {

            Look();
            Move();
            if (Input.GetMouseButtonDown(0))
                Shoot();
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, realPosition, .1f);
            transform.rotation = Quaternion.Lerp(transform.rotation, realRotation, .1f);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            realPosition = (Vector3)stream.ReceiveNext();
            realRotation = (Quaternion)stream.ReceiveNext();
        }
    }

    void Look()
    {
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);
        transform.Rotate(Vector3.left * Input.GetAxisRaw("Mouse Y") * mouseSensitivity);

        verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);

        cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
    }

    void Move()
    {
        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed), ref smoothMoveVelocity, smoothTime);
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine)
            return;

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
    }

    void Shoot()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        ray.origin = cam.transform.position;
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            //hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damage);
            //PV.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);
        }
        photonView.RPC("RPC_Shoot_Projectile", RpcTarget.All);
        //RPC_Shoot_Projectile();
    }


    [PunRPC]
    void RPC_Shoot_Projectile()
    {
        Debug.Log("Spawning Projectile Bullet");
        GameObject bullet = BulletPool.SpawnPooledGameObject();

        if (photonView.IsMine)
        {
            bullet.transform.position = gameObject.transform.position;
            bullet.transform.rotation = gameObject.transform.rotation;
        }
        else
        {
            bullet.transform.position = realPosition;
            bullet.transform.rotation = realRotation;

        }
        bullet.SetActive(true);
    }
}
