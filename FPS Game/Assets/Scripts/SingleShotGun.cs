using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleShotGun : Gun
{
	[SerializeField] Camera cam;

	PhotonView PV;
    public PlayerController playerController;

	void Awake()
	{
		PV = GetComponent<PhotonView>();
	}

	public override void Use()
	{
		Shoot();
	}

	void Shoot()
	{
		Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
		ray.origin = cam.transform.position;
		if(Physics.Raycast(ray, out RaycastHit hit))
		{
			//hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damage);
			//PV.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);
		}
            PV.RPC("RPC_Shoot_Projectile", RpcTarget.All, gameObject.transform.position, gameObject.transform.rotation);
        //RPC_Shoot_Projectile();
	}

	[PunRPC]
	void RPC_Shoot(Vector3 hitPosition, Vector3 hitNormal)
	{
		Collider[] colliders = Physics.OverlapSphere(hitPosition, 0.3f);
		if(colliders.Length != 0)
		{
			GameObject bulletImpactObj = Instantiate(bulletImpactPrefab, hitPosition + hitNormal * 0.001f, Quaternion.LookRotation(hitNormal, Vector3.up) * bulletImpactPrefab.transform.rotation);
			Destroy(bulletImpactObj, 10f);
			bulletImpactObj.transform.SetParent(colliders[0].transform);
		}
	}

    [PunRPC]
    void RPC_Shoot_Projectile(Vector3 pos, Quaternion quat)
    {
        Debug.Log("Spawning Projectile Bullet");
        GameObject bullet = playerController.BulletPool.SpawnPooledGameObject();

        bullet.transform.position = pos;
        bullet.transform.rotation = quat;
        bullet.SetActive(true);
    }
}
