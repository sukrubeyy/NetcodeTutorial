using System;
using System.Collections;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerController : NetworkBehaviour, IDamageable
{
    public float speed = 5f;
    public Rigidbody rb;
    public ParticleSystem spawnParticle;
    public ParticleSystem dieParticle;

    private NetworkVariable<PlayerPosition> playerPosInfo = new NetworkVariable<PlayerPosition>(new PlayerPosition
    {
        xPos = 0,
        yPos = 0,
        zPos = 0
    }, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private const float MaxHealt = 100f;
    private float CurrentHealt = MaxHealt;
    [SerializeField] private Transform bulletTransform;
    [SerializeField] private GameObject bulletGameObject;
    public Status playerStatus;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        transform.position = SpawnManager.Instance.GetSpawnPoint((int) OwnerClientId);
        playerStatus = Status.PlayMode;
        SendSpawnParticleServerRpc();
        Debug.Log("User Spawn");
    }

    private void OnConnectedToServer()
    {
        Debug.LogError("User Connected To Server");
    }

    public float rotationSpeed = 5f;

    void Update()
    {
        if (!IsOwner) return;
        if (playerStatus is not Status.PlayMode) return;

        if (Input.GetMouseButtonDown(0))
        {
            SendBulletServerRpc();
        }

        Movement();
    }

    void Movement()
    {
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");
        var moveDirection = new Vector3(horizontal, 0f, vertical);
        moveDirection.Normalize();
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
        playerPosInfo.Value = new PlayerPosition
        {
            xPos = transform.position.x,
            yPos = transform.position.y,
            zPos = transform.position.z
        };
        if (moveDirection != Vector3.zero)
        {
            Quaternion newRot = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, newRot, rotationSpeed * Time.deltaTime);
        }
    }


    [ServerRpc]
    private void SendBulletServerRpc()
    {
        GameObject bullet = Instantiate(bulletGameObject, bulletTransform.position, bulletTransform.rotation);
        bullet.GetComponent<NetworkObject>().Spawn();
        bullet.GetComponent<Rigidbody>().AddForce(transform.forward * 15f, ForceMode.Impulse);
    }

    [ServerRpc]
    private void SendSpawnParticleServerRpc()
    {
        GameObject spawnPart = Instantiate(spawnParticle.gameObject);
        spawnPart.transform.position = SpawnManager.Instance.GetSpawnPoint((int) OwnerClientId);
        spawnPart.GetComponent<NetworkObject>().Spawn();
        spawnPart.GetComponent<ParticleSystem>().Play();
        StartCoroutine(DestroyParticle());

        IEnumerator DestroyParticle()
        {
            yield return new WaitForSeconds(0.5f);
            spawnPart.GetComponent<NetworkObject>().Despawn();
            Destroy(spawnPart.gameObject);
        }
    }

    [ServerRpc]
    private void DieServerRpc(PlayerPosition playerPosition)
    {
        var pos = new Vector3(playerPosition.xPos, playerPosition.yPos, playerPosition.zPos);
        GameObject diePart = Instantiate(dieParticle.gameObject, pos, Quaternion.identity);
        diePart.GetComponent<NetworkObject>().Spawn();
        diePart.GetComponent<ParticleSystem>().Play();
    }

    public void TakeDamage(float damage)
    {
        if (!IsOwner) return;

        CurrentHealt -= damage;
        if (CurrentHealt <= 0)
        {
            //playerStatus = Status.Die;
            DieServerRpc(playerPosInfo.Value);
            CurrentHealt = MaxHealt;
            transform.position = SpawnManager.Instance.GetSpawnPoint((int) OwnerClientId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(ulong targetId)
    {
        TakeDamageClientRpc(targetId);
    }

    [ClientRpc]
    private void TakeDamageClientRpc(ulong targetId)
    {
        if (targetId == NetworkManager.Singleton.LocalClient.ClientId)
        {
            TakeDamage(10f);
        }
    }
}

public struct PlayerPosition : INetworkSerializable
{
    public float xPos;
    public float yPos;
    public float zPos;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref xPos);
        serializer.SerializeValue(ref yPos);
        serializer.SerializeValue(ref zPos);
    }
}

public enum Status
{
    PlayMode,
    Die
}