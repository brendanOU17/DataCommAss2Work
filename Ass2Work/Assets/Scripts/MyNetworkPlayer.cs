using System.Linq;
using UnityEngine;
using Mirror;
using TMPro;

public class MyNetworkPlayer : NetworkBehaviour
{
    [SerializeField] private TMP_Text displayNameText = null;
    [SerializeField] private Renderer displayColorRenderer = null;

    [SyncVar(hook = nameof(HandleDisplayNameUpdate))]
    [SerializeField]
    private string displayName = "Missing Name";

    [SyncVar(hook = nameof(HandleDisplayColorUpdate))]
    [SerializeField]
    private Color displayColor = Color.black;

    public string DisplayName => displayName;
    public Color DisplayColor => displayColor;

    [SyncVar] public bool hasAuthorityPickup = false;
    [SyncVar] public float protectionTimer = 0f;
    [SyncVar] public int score = 0;
    [SerializeField] private float speedBoost = 1.5f;

    private void Update()
    {
        if (isLocalPlayer)
        {
            UIManager.instance.UpdatePlayerInfo(connectionToClient.connectionId, displayName, score);
        }

        if (isLocalPlayer && hasAuthorityPickup)
        {
            if (protectionTimer > 0f)
            {
                protectionTimer -= Time.deltaTime;
            }
            else
            {
                score += (int)(5 * Time.deltaTime);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isLocalPlayer || displayColor != Color.red) return;

        if (other.gameObject.CompareTag("Player") && hasAuthorityPickup && protectionTimer <= 0f)
        {
            CmdTransferAuthorityPickup(other.gameObject);
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
    public void CmdTransferAuthorityPickup(GameObject otherPlayerGameObject)
    {
        MyNetworkPlayer otherPlayer = otherPlayerGameObject.GetComponent<MyNetworkPlayer>();
        if (!hasAuthorityPickup || otherPlayer.hasAuthorityPickup || protectionTimer > 0f) return;

        SetDisplayColor(Color.green);
        hasAuthorityPickup = false;

        otherPlayer.SetDisplayColor(Color.red);
        otherPlayer.hasAuthorityPickup = true;
        otherPlayer.protectionTimer = 5f;

        // Apply speed boost
        otherPlayer.GetComponent<CharacterController>().Move(otherPlayer.transform.forward * otherPlayer.speedBoost);

        // Make the pickup a child of the player
        NetworkIdentity pickupIdentity = GetComponentInChildren<NetworkIdentity>();
        NetworkServer.ReplacePlayerForConnection(otherPlayer.connectionToClient, pickupIdentity.gameObject);
        pickupIdentity.transform.SetParent(otherPlayer.transform);
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
