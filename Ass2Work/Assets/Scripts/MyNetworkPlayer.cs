using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine;

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
    private void HandleDisplayColorUpdate(Color oldColor,Color newColor)
    {
        displayColorRenderer.material.SetColor("_BaseColor", newColor);
    }

    private void HandleDisplayNameUpdate(string oldName, string  newName)
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
    #endregion
}
