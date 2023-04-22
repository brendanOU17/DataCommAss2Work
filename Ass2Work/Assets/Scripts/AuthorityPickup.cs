using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AuthorityPickup : NetworkBehaviour
{
    private Vector3 initialPosition;

    private void Start()
    {
        initialPosition = transform.position;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        MyNetworkPlayer player = other.GetComponent<MyNetworkPlayer>();
        if (player != null && !player.hasAuthorityPickup && player.protectionTimer <= 0f)
        {
            player.CmdTransferAuthorityPickup(other.gameObject);
        }
    }
}
