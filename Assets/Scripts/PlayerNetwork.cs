using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{

  [SerializeField] private Transform spawnedObjectPrefab;
  private Transform spawnedObjectTransform;

  private NetworkVariable<MyCustomData> randomNumber = new NetworkVariable<MyCustomData>(
    new MyCustomData
    {
      _int = 0,
      _bool = false,
      message = "Hello World"
    },
    NetworkVariableReadPermission.Everyone,
    NetworkVariableWritePermission.Owner
  );

  private struct MyCustomData : INetworkSerializable
  {
    public int _int;
    public bool _bool;
    public FixedString128Bytes message;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
      serializer.SerializeValue(ref _int);
      serializer.SerializeValue(ref _bool);
      serializer.SerializeValue(ref message);
    }
  }

  public override void OnNetworkSpawn()
  {
    randomNumber.OnValueChanged += (MyCustomData oldValue, MyCustomData newValue) =>
    {
      Debug.Log(OwnerClientId + "; randomNumber: " + newValue._int + "; " + newValue._bool + "; " + newValue.message);
    };
  }

  private void Update()
  {

    if (!IsOwner) return;

    if (Input.GetKeyDown(KeyCode.Space))
    {
      spawnedObjectTransform = Instantiate(spawnedObjectPrefab);
      spawnedObjectTransform.GetComponent<NetworkObject>().Spawn(true);


      TestServerRpc(new ServerRpcParams());
      TestClientRpc(new ClientRpcParams
      {
        Send = new ClientRpcSendParams
        {
          TargetClientIds = new List<ulong> { 0 }
        }
      });
      randomNumber.Value = new MyCustomData
      {
        _int = Random.Range(0, 100),
        _bool = Random.Range(0, 2) == 0,
        message = "Hello World 2"
      };
    }

    if (Input.GetKeyDown(KeyCode.Y))
    {
      if (spawnedObjectTransform != null)
      {
        Destroy(spawnedObjectTransform.gameObject);
        spawnedObjectTransform.GetComponent<NetworkObject>().Despawn(true);
      }
    }


    Vector3 moveDir = new Vector3(0, 0, 0);

    if (Input.GetKey(KeyCode.W))
    {
      moveDir += Vector3.forward;
    }
    if (Input.GetKey(KeyCode.S))
    {
      moveDir += Vector3.back;
    }
    if (Input.GetKey(KeyCode.A))
    {
      moveDir += Vector3.left;
    }
    if (Input.GetKey(KeyCode.D))
    {
      moveDir += Vector3.right;
    }

    float moveSpeed = 5f;
    transform.position += moveDir * Time.deltaTime * moveSpeed;
  }

  [ServerRpc]
  private void TestServerRpc(ServerRpcParams serverRpcParams)
  {
    Debug.Log(OwnerClientId + " :: TestServerRpc :: " + serverRpcParams.Receive.SenderClientId);
  }

  [ClientRpc]
  private void TestClientRpc(ClientRpcParams clientRpcParams)
  {
    Debug.Log(OwnerClientId + " :: TestClientRpc :: ");
  }
}
