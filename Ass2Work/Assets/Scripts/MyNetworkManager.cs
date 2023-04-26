using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class MyNetworkManager : NetworkManager
{

    public CountdownTimer countdownTimer;
    public UIManager uiManager;
    public MyNetworkPlayer mynetworkplayer;
    public List<GameObject> players = new List<GameObject>();
    public bool isTaggedPlayerPresent;
 


    public override void OnClientConnect()
    {
        base.OnClientConnect();
        Debug.Log("You have connected to the server");
       


    }



    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);
        Debug.Log($"Player connection: {conn}"); // Check the player's connection value
        MyNetworkPlayer player = conn.identity.GetComponent<MyNetworkPlayer>();

        player.SetDisplayName($"Player{numPlayers}");
        Color displayColor = Color.green;
        player.SetDisplayColor(displayColor);
        players.Add(conn.identity.gameObject);
        RandomAssignAuthority(players);

        Debug.Log($"Current number of players: {numPlayers}");

    }

    public void RandomAssignAuthority(List<GameObject> players)
    {
        Debug.Log("randomAssign is being used");
        if (players.Count > 1)
        {
            isTaggedPlayerPresent = false;

            foreach (GameObject playerObj in players)
            {
                MyNetworkPlayer playerInstance = playerObj.GetComponent<MyNetworkPlayer>();
                if (playerInstance.isTagged)
                {
                    isTaggedPlayerPresent = true;
                    break;
                }
            }

            if (!isTaggedPlayerPresent)
            {
                Debug.Log($"Now Assgining TaggedPlayer");
                MyNetworkPlayer taggedPlayer = players[Random.Range(0, players.Count)].GetComponent<MyNetworkPlayer>();
                taggedPlayer.SetDisplayColor(Color.red);
                taggedPlayer.isTagged = true;
                taggedPlayer.GetComponent<NetworkIdentity>();
                Debug.Log($"Tagged Player: {taggedPlayer.DisplayName} (isTagged: {taggedPlayer.isTagged})");
                countdownTimer.StartTimer();
                uiManager.UpdateTimerDisplay(countdownTimer.RemainingTime);

                TaggedStatus taggedStatusInstance = FindObjectOfType<TaggedStatus>();
                taggedStatusInstance.AssignClientAuthority(taggedPlayer.connectionToClient);
                Debug.Log($"Tagged Player {taggedPlayer.DisplayName} has authority? : {taggedStatusInstance.isOwned}");
            }
        }
    }


    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        players.Remove(conn.identity.gameObject);
        base.OnServerDisconnect(conn);

        if (players.Count == 1)
        {
            MyNetworkPlayer remainingPlayer = players[0].GetComponent<MyNetworkPlayer>();
            if (remainingPlayer.isTagged)
            {
                remainingPlayer.RpcDeclareWinner(players[0]);
            }
        }
    }

  
  
}
