using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class MyNetworkManager : NetworkManager
{

    public CountdownTimer countdownTimer;
    public UIManager uiManager;
    public GameObject authorityPickupPrefab;
    public List<GameObject> players = new List<GameObject>();
    public const int MaxPlayers = 8;
    private bool authorityPickupSpawned = false;





    public override void OnClientConnect()
    {
        base.OnClientConnect();
        Debug.Log("You have connected to the server");
       
    }



    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        if (players.Count < MaxPlayers)
        {
                base.OnServerAddPlayer(conn);
                Debug.Log($"Player connection: {conn}"); // Check the player's connection value
                MyNetworkPlayer player = conn.identity.GetComponent<MyNetworkPlayer>();

                player.SetDisplayName($"Player{numPlayers}");
                Color displayColor = Color.green;
                player.SetDisplayColor(displayColor);
                players.Add(conn.identity.gameObject);


                if (players.Count > 1 && !authorityPickupSpawned )
                {
                    
                        SpawnAuthorityPickup(conn);
                        authorityPickupSpawned = true;
                        countdownTimer.StartTimer();
                        uiManager.UpdateTimerDisplay(countdownTimer.RemainingTime);
                    
                }

                Debug.Log($"Current number of players: {numPlayers}");
        }
       
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        // Find the AuthorityPickup object with authority
        AuthorityPickup authorityPickup = FindObjectOfType<AuthorityPickup>();
        if (authorityPickup != null && authorityPickup.connectionToClient == conn)
        {
            // Remove authority from the client before disconnecting
            authorityPickup.GetComponent<NetworkIdentity>().RemoveClientAuthority();
        }
        players.Remove(conn.identity.gameObject);

        if (players.Count == 1)
        {
            MyNetworkPlayer remainingPlayer = players[0].GetComponent<MyNetworkPlayer>();
            remainingPlayer.RpcDeclareWinner(players[0]);
 
        }

        base.OnServerDisconnect(conn);

       
    }

    #region Server
    [Server]
    private void SpawnAuthorityPickup(NetworkConnectionToClient conn)
    {
        // Generate a random position within the specified bounds
        float randomX = Random.Range(-12f, 12f);
        float randomZ = Random.Range(-6f, 6f);
        Vector3 randomSpawnPosition = new Vector3(randomX, 0f, randomZ);

        GameObject pickup = Instantiate(authorityPickupPrefab, randomSpawnPosition, Quaternion.identity);
        NetworkServer.Spawn(pickup); // Spawn the pickup on the network

    }


    #endregion

}
