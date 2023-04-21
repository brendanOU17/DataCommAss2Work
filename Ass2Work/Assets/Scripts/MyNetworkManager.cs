using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class MyNetworkManager : NetworkManager
{
    public TMP_InputField nameInputField; // reference to the input field in the scene
    public CountdownTimer countdownTimer;
    public UIManager uiManager;

    public List<GameObject> players = new List<GameObject>();

    public override void OnClientConnect()
    {
        base.OnClientConnect();
        Debug.Log("You have connected to the server");
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);
        MyNetworkPlayer player = conn.identity.GetComponent<MyNetworkPlayer>();

        string playerName = nameInputField.text; // get name from the input field
        player.SetDisplayName(playerName);

        Color displayColor = Color.green;
        player.SetDisplayColor(displayColor);

        players.Add(conn.identity.gameObject);

        if (players.Count > 1)
        {
            conn.identity.GetComponent<MyNetworkPlayer>().RpcSelectRandomTaggedPlayer();
            countdownTimer.StartTimer();
            uiManager.UpdateTimerDisplay(countdownTimer.RemainingTime);
        }

        Debug.Log($"Current number of players: {numPlayers}");
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        players.Remove(conn.identity.gameObject);
        base.OnServerDisconnect(conn);

        if (players.Count == 1)
        {
            players[0].GetComponent<MyNetworkPlayer>().RpcDeclareWinner(players[0]);
        }
    }

  
  
}
