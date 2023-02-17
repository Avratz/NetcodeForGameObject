using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{

  private void Update()
  {

    if (!IsOwner) return;

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
}
