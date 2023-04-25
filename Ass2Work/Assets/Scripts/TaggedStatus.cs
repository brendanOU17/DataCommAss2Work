using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TaggedStatus : NetworkBehaviour
{
     [SerializeField] private NetworkIdentity networkIdentity;
    [Server]
    public void TransferAuthority( NetworkConnectionToClient newOwnerConnection)
    {
        // Check if the object already has an owner and remove the authority
        if (networkIdentity.connectionToClient != null)
        {
            networkIdentity.RemoveClientAuthority();
            Debug.Log("Removed existing authority");
        }

        // Assign authority to the new owner
        networkIdentity.AssignClientAuthority(newOwnerConnection);
        Debug.Log($"Transferred authority. New tagged player connection ID: {newOwnerConnection.connectionId}. Authority status: {newOwnerConnection.identity.isOwned}");
    }



    [Server]
    public void AssignClientAuthority( NetworkConnectionToClient newOwnerConnection)
    {
        networkIdentity.AssignClientAuthority(newOwnerConnection);
        Debug.Log($"Assigned client authority. New tagged player connection ID: {newOwnerConnection.connectionId}.Authority status: {networkIdentity.isOwned}");

    }

}