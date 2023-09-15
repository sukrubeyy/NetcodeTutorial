using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class BulletController : NetworkBehaviour
{
    public ParticleSystem exploseParticle;
    [SerializeField] private Rigidbody rb;
    private int touchCount = 3;
    private float throwForce = 15f;

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsOwner) return;
        if (collision.gameObject.tag == "Player")
        {
            ulong id = collision.gameObject.GetComponent<NetworkObject>().OwnerClientId;
            collision.gameObject.GetComponent<PlayerController>().TakeDamageServerRpc(id);
            CreateExploseEffectServerRpc();
            DestroyOnServerRpc();
            return;
        }
        else
        {
            Vector3 direction = Vector3.Reflect(transform.forward, collision.transform.forward);
            if(direction == Vector3.left || direction == Vector3.right)
                DestroyOnServerRpc();
            else
            {
                direction.Normalize();
                rb.AddForce(direction * throwForce, ForceMode.Impulse);
            }
        }

        CreateExploseEffectServerRpc();
        touchCount--;
        if (touchCount <= 0)
        {
            DestroyOnServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyOnServerRpc()
    {
        Destroy(gameObject);
    }

    [ServerRpc]
    private void CreateExploseEffectServerRpc()
    {
        GameObject effectObj = Instantiate(exploseParticle.gameObject, transform.position, Quaternion.identity);
        effectObj.GetComponent<NetworkObject>().Spawn();
        effectObj.GetComponent<ParticleSystem>().Play();
    }
}