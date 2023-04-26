using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class MyNetworkPlayer : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text displayNameText = null;
    [SerializeField] private Renderer displayColorRenderer = null;
    [SerializeField] private TaggedStatus taggedStatus = null;
    [SerializeField] private PlayerMovement playerMovement = null;


    [Header("Settings")]
    [SerializeField] private float tagCooldown = 5.0f;
    [SerializeField] public GameObject taggedStatusObject;


    [Header("SyncVars")]
    [SyncVar(hook = nameof(HandleDisplayNameUpdate))]
    private string displayName = "Missing Name";

    [SyncVar(hook = nameof(HandleDisplayColorUpdate))]
    private Color displayColor = Color.black;

    [SyncVar(hook = nameof(HandleIsTaggedUpdate))]
    public bool isTagged = false;


    private bool canTag = true;
    private bool isInTaggingProcess = false;

    public string DisplayName => displayName;
    public Color DisplayColor => displayColor;

    public delegate void AuthorityChangedDelegate(bool isOwner);
    public event AuthorityChangedDelegate AuthorityChanged;
   

    private void Start()
    {
        
            taggedStatus = FindObjectOfType<TaggedStatus>();
            taggedStatusObject = taggedStatus.gameObject;
          
    }
 

    #region server
    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if (!isServer) return;

        if (other.gameObject.CompareTag("Player") && canTag)
        {
            MyNetworkPlayer otherPlayer = other.gameObject.GetComponent<MyNetworkPlayer>();

            if (displayColor == Color.red && otherPlayer.displayColor == Color.green)
            {
                if (otherPlayer.isInTaggingProcess) return;
                isInTaggingProcess = true;
                otherPlayer.isInTaggingProcess = true;

                SetDisplayColor(Color.green);
                otherPlayer.SetDisplayColor(Color.red);

                // Transfer authority to the newly tagged player
                NetworkIdentity otherPlayerIdentity = otherPlayer.GetComponent<NetworkIdentity>();
                NetworkIdentity thisPlayerIdentity = GetComponent<NetworkIdentity>();
                TransferAuthority(thisPlayerIdentity, otherPlayerIdentity, otherPlayer.connectionToClient);

                cmdSetIsTagged(false);
                otherPlayer.cmdSetIsTagged(true);
                StartCoroutine(TagCooldownCoroutine());

                StartCoroutine(ResetTaggingProcess(otherPlayer));
            }
        }
    }
   [Server]
private IEnumerator ResetTaggingProcess(MyNetworkPlayer otherPlayer)
{
    yield return new WaitForSeconds(0.5f);
    isInTaggingProcess = false;
    otherPlayer.isInTaggingProcess = false;
}

    [Command]
    public void TransferAuthority(NetworkIdentity oldPlayerIdentity, NetworkIdentity newPlayerIdentity, NetworkConnectionToClient newOwnerConnection)
    {
        //RemoveAuthority(taggedStatusObject.GetComponent<NetworkIdentity>());
        //Debug.Log($"Old tagged player's authority status: {oldPlayerIdentity.isOwned}");

        // Wait until authority is removed
        taggedStatus.TransferAuthority( newOwnerConnection);
        Debug.Log($"New tagged player's authority status: {newPlayerIdentity.isOwned}");

        // Raise AuthorityChanged event for both old and new tagged players
        oldPlayerIdentity.GetComponent<MyNetworkPlayer>().AuthorityChanged?.Invoke(false);
        newPlayerIdentity.GetComponent<MyNetworkPlayer>().AuthorityChanged?.Invoke(true);
    }

   
    [Server]
    public void cmdSetIsTagged(bool newIsTagged)
    {
        isTagged = newIsTagged;
    }
    [Server]
    private void OnAuthorityChanged(bool isOwner)
    {
        if (isOwner)
        {
            cmdSetIsTagged(true);
            playerMovement.movementSpeed = 15.0f;
        }
        else
        {
            cmdSetIsTagged(false);
            playerMovement.movementSpeed = 5.0f;
        }
    }

    [Server]
    private IEnumerator TagCooldownCoroutine()
    {
        canTag = false;
        yield return new WaitForSeconds(tagCooldown);
        canTag = true;
    }
    [Server]
    public void HandleIsTaggedUpdate(bool oldIsTagged, bool newIsTagged)
    {
        if (!isServer) return;
        isTagged = newIsTagged;
        Debug.Log($"Player {displayName} isTagged updated: {newIsTagged}");
    }

    [Server]
    public void KillTaggedPlayer()
    {
        if (displayColor == Color.red)
        {
            taggedStatus.RemoveAuthority();
            Destroy(gameObject);
            // Implement the logic for killing the tagged player.
            // For example, removing the player from the game or moving them to a respawn point.
        }
    }

  
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
    private void CmdDIsplayName(string newDisplayName)
    {
        // server authority to limit displayname into 2-20 latter length
        if (newDisplayName.Length < 2 || newDisplayName.Length > 20)
        {
            return;
        }
        RpcDisplayNewName(newDisplayName);
        SetDisplayName(newDisplayName);
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

    [ContextMenu("Set This Name")]
    private void SetThisName()
    {
        CmdDIsplayName("My New Name");
    }
    
  

    [ClientRpc]
    private void RpcDisplayNewName(string newDisplayName)
    {
        Debug.Log(newDisplayName);
    }

    [ClientRpc]
    public void RpcDeclareWinner(GameObject winner)
    {
        if (!isLocalPlayer) return;

        MyNetworkPlayer winnerPlayer = winner.GetComponent<MyNetworkPlayer>();
        FindObjectOfType<UIManager>().ShowWinner(winnerPlayer.DisplayName);
    }
    #endregion

    public override void OnStartClient()
    {
        AuthorityChanged += OnAuthorityChanged;
        HandleDisplayColorUpdate(displayColor, displayColor);
        HandleDisplayNameUpdate(displayName, displayName);
    }

    public override void OnStopClient()
    {
        AuthorityChanged -= OnAuthorityChanged;
    }
}
