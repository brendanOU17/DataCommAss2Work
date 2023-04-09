using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class MyNetworkManager : NetworkManager
{
    public TMP_InputField nameInputField; // reference to the input field in the scene

    public override void OnClientConnect()
    {
        base.OnClientConnect();
        Debug.Log("you have connected to server");
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);
        MyNetworkPlayer player = conn.identity.GetComponent<MyNetworkPlayer>();

        string playerName = nameInputField.text; // get name from the input field

        player.SetDisplayName(playerName);

        Color displayColor = new Color(Random.RandomRange(0f, 1f), Random.RandomRange(0f, 1f), Random.Range(0f, 1f));

        player.SetDisplayColor(displayColor);

        Debug.Log($"current number of players: {numPlayers}");
    }
}
