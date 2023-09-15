using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DestroyParticle : NetworkBehaviour
{
    private void Start()
    {
        StartCoroutine(DoDestroy());
        IEnumerator DoDestroy()
        {
            yield return new WaitForSeconds(1f);
            DestroyOnServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyOnServerRpc()
    {
        Destroy(gameObject);
    }


    private void OnCollisionEnter(Collision collision)
    {
        DestroyOnServerRpc();
    }
}
