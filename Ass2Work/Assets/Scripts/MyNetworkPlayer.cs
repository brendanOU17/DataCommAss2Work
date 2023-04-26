using System.Linq;
using UnityEngine;
using Mirror;
using TMPro;

public class MyNetworkPlayer : NetworkBehaviour
{
    [SerializeField] private TMP_Text displayNameText = null;
    [SerializeField] private Renderer displayColorRenderer = null;
    [SerializeField] private PlayerMovement playerMovement = null;
    [SerializeField] private AuthorityPickup authorityitem = null;

    [SyncVar(hook = nameof(HandleDisplayNameUpdate))]
    [SerializeField]
    private string displayName = "Missing Name";

    [SyncVar(hook = nameof(HandleDisplayColorUpdate))]
    [SerializeField] public Color displayColor = Color.black;

    private float pickupCooldown = 0f;
    private float pickupDelay = 0.5f;

    public string DisplayName => displayName;
    public Color DisplayColor => displayColor;

    private void Update()
    {
        if (pickupCooldown > 0)
        {
            pickupCooldown -= Time.deltaTime;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!isLocalPlayer || pickupCooldown > 0) return;

        if (other.gameObject.CompareTag("AuthorityPickUp"))
        {
            CmdApplyAuthorityPickup(other.gameObject);
        }


    }

   

    #region server 

    [Server]
    public void SetDisplayName(string newDisplayName)
    {
        displayName = newDisplayName;
    }

    [Server]
    public void SetDisplayColor(Color newDisplayColor)
    {
        displayColor = newDisplayColor;
    }

  

    [Command]
    public void CmdApplyAuthorityPickup(GameObject authorityGameObject)
    {
        // Get the NetworkIdentity component of the pickup object
        NetworkIdentity pickupIdentity = authorityGameObject.GetComponent<NetworkIdentity>();
        // Move the pickup to a random position
        float randomX = Random.Range(-12f, 12f);
        float randomZ = Random.Range(-6f, 6f);
        Vector3 randomSpawnPosition = new Vector3(randomX, 0f, randomZ);

        if (displayColor == Color.red)
        {
            // Client already has authority and is red, just move the pickup object
            authorityGameObject.transform.position = randomSpawnPosition;
        }
        else if (displayColor == Color.green)
        {
            // Remove authority from the current owner
            if (pickupIdentity.connectionToClient != null)
            {
                MyNetworkPlayer previousOwner = pickupIdentity.connectionToClient.identity.GetComponent<MyNetworkPlayer>();
                previousOwner.SetDisplayColor(Color.green);
                previousOwner.playerMovement.movementSpeed = 5.0f;
                Debug.Log($"Authority removed from {pickupIdentity.connectionToClient.identity.name}");
                pickupIdentity.RemoveClientAuthority();
            }

            // Assign authority to the current client
            pickupIdentity.AssignClientAuthority(connectionToClient);

            SetDisplayColor(Color.red);
            playerMovement.movementSpeed = 15.0f;
            Debug.Log($"Authority assigned to {connectionToClient.identity.name}");

            // Move the pickup object
            authorityGameObject.transform.position = randomSpawnPosition;
        }
    }
  
    [Command]
    public void CmdChangeName(string newName)
    {
        
        HandleDisplayNameUpdate(displayName, newName);
    }



    #endregion

    #region client 
    private void HandleDisplayColorUpdate(Color oldColor, Color newColor)
    {
        displayColorRenderer.material.SetColor("_BaseColor", newColor);
    }

    private void HandleDisplayNameUpdate(string oldName, string newName)
    {
        displayNameText.text = newName;
    }


    [ClientRpc]
    public void RpcDeclareWinner(GameObject winner)
    {
        if (!isLocalPlayer) return;

        MyNetworkPlayer winnerPlayer = winner.GetComponent<MyNetworkPlayer>();
        FindObjectOfType<UIManager>().ShowWinner(winnerPlayer.DisplayName);
    }

  
  
    #endregion
}
