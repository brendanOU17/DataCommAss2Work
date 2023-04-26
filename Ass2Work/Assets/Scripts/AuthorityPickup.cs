using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AuthorityPickup : NetworkBehaviour
{
    public NetworkIdentity authorityPickUpIdentity;

    public NetworkConnection connectionToClient;

    public override void OnStartAuthority()
    {
        if (NetworkServer.spawned.ContainsKey(netId))
        {
            connectionToClient = NetworkServer.spawned[netId].connectionToClient;
        }
    }
}

