using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ProjectileBullet : MonoBehaviour
{
    public int damage;
    public float bulletSpeed;
    public RaycastHit[] hits;
    Vector3 prePos;


    void FixedUpdate()
    {
        prePos = transform.position;

        if (gameObject.activeSelf)
            transform.Translate(Vector3.forward * Time.deltaTime * bulletSpeed); // Moves the bullet at 'bulletSpeed' units per second

        hits = Physics.RaycastAll(new Ray(prePos, (transform.position - prePos).normalized), (transform.position - prePos).magnitude);//, layerMask);

        for (int i = 0; i < hits.Length; i++)
        {
            GameObject hit = hits[i].collider.gameObject;
            if (hit.GetComponent<PlayerController>() != null)
            {
                Debug.Log("Hit Player Controller: " + hit.name);
                hit.GetComponent<IDamageable>()?.TakeDamage(10);
            }
            else
            {
                Debug.Log("Unknow bullet behaviour");
            }

        }

        Debug.DrawLine(transform.position, prePos);
    }
}
