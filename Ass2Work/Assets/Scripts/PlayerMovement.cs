using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;
public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent agent = null;
    private Camera mainCamera;

    #region server
    [Command]
    private void CmdMove(Vector3 position)
    {
        if(!NavMesh.SamplePosition(position,out NavMeshHit hit, 1f, NavMesh.AllAreas))
        {
            return;
        }
        agent.SetDestination(hit.position);
    }


#endregion

    #region client

    // Start method for the client who wons the object
    public override void OnStartAuthority()
    {
        mainCamera = Camera.main; // Camera reference
    }

    [ClientCallback] //makes it client only update (all client)

    private void Update()
    {
        //make sure object belongs to the client
        if (!isOwned)//if (!hasAuthority) is the old function
        {
            return;
        }

        //check the W, A, S, D input
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        //calculate the new position for the player
        Vector3 newPosition = transform.position + new Vector3(horizontalInput, 0f, verticalInput);

        //call the CmdMove() function with the new position
        CmdMove(newPosition);
    }
    #endregion
}
