using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine;


public class MyNetworkManager : NetworkManager
{
   public override void OnClientConnect()
    {
        base.OnClientConnect();
        Debug.Log("you have connected to server");
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);
        MyNetworkPlayer player = conn.identity.GetComponent<MyNetworkPlayer>();

        player.SetDisplayName($"Player{numPlayers}");

        Color displayColor = new Color(Random.RandomRange(0f, 1f), Random.RandomRange(0f, 1f), Random.Range(0f, 1f));

        player.SetDisplayColor (displayColor);

        Debug.Log($"current number of players");
    }
}
