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

    private void OnTriggerEnter(Collider other)
    {
        MyNetworkPlayer player = other.GetComponent<MyNetworkPlayer>();
        if (player != null)
        {
            Debug.Log("Player detected");

            if (!player.hasAuthorityPickup && player.protectionTimer <= 0f)
            {
                Debug.Log("Player does not have authority pickup and no protection timer");

                // Check if this object has server authority
                if (isOwned)
                {
                    Debug.Log("Object is owned by server");
                    // Call the command directly on this script
                    CmdRequestAuthorityTransfer(player.connectionToClient, GetComponent<NetworkIdentity>());
                }
                else
                {
                    Debug.Log("Object is not owned by server");
                }
            }
        }
    }

    [Server]
    public void AssignServerAuthority()
    {
        NetworkIdentity networkIdentity = GetComponent<NetworkIdentity>();
        if (networkIdentity != null && networkIdentity.connectionToClient != null)
        {
            networkIdentity.RemoveClientAuthority();
        }
    }

    [Command]
    public void CmdRequestAuthorityTransfer(NetworkConnectionToClient conn, NetworkIdentity networkIdentity)
    {
        if (networkIdentity != null && !networkIdentity.isOwned)
        {
            networkIdentity.AssignClientAuthority(conn);
        }
    }
}

