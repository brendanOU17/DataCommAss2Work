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

        //check the right mouse button input
        if (!Input.GetMouseButtonDown(0))
        {
            return;
        }

        //grab mouse cursor information
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        //check the scene where it is hit
        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity)) { return; }

        CmdMove(hit.point);
    }
    #endregion
}
