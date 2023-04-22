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
    [SerializeField] private GameObject authorityPickupPrefab;
    public const int MaxPlayers = 8;



    public override void OnClientConnect()
    {
        base.OnClientConnect();
        Debug.Log("You have connected to the server");
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        Transform startPos = GetStartPosition();
        GameObject player = startPos != null
            ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
            : Instantiate(playerPrefab);

        MyNetworkPlayer networkPlayer = player.GetComponent<MyNetworkPlayer>();

        string playerName = nameInputField.text;
        networkPlayer.SetDisplayName(playerName);

        Color displayColor = Color.green;
        networkPlayer.SetDisplayColor(displayColor);

        NetworkServer.AddPlayerForConnection(conn, player);

        if (NetworkServer.connections.Count > 1)
        {
            SpawnAuthorityPickup(conn);
            countdownTimer.StartTimer();
            uiManager.UpdateTimerDisplay(countdownTimer.RemainingTime);
        }

        Debug.Log($"Current number of players: {numPlayers}");
    }

    [Server]
    private void SpawnAuthorityPickup(NetworkConnectionToClient conn)
    {
        // Generate a random position within the specified bounds
        float randomX = Random.Range(-12f, 12f);
        float randomZ = Random.Range(-6f, 6f);
        Vector3 randomSpawnPosition = new Vector3(randomX, 0f, randomZ);

        GameObject pickup = Instantiate(authorityPickupPrefab, randomSpawnPosition, Quaternion.identity);
        NetworkServer.Spawn(pickup, conn);
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);

        if (NetworkServer.connections.Count == 1)
        {
            NetworkServer.connections[0].identity.GetComponent<MyNetworkPlayer>().RpcDeclareWinner(NetworkServer.connections[0].identity.gameObject);
        }
    }
}
