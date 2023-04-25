using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CountdownTimer : NetworkBehaviour
{
    [SerializeField] private float initialTime = 90f;

    [SyncVar(hook = nameof(OnTimeChanged))]
    private float remainingTime;

    [SyncVar]
    private NetworkIdentity taggedPlayerIdentity;

    private bool isRunning = false;
    public UIManager uiManager;
    public float RemainingTime { get => remainingTime; }

    public void StartTimer()
    {
        if (!isServer) return;

        if (!isRunning)
        {
            remainingTime = initialTime;
            isRunning = true;
            StartCoroutine(TimerCoroutine());
        }
    }

    public void ResetTimer()
    {
        if (!isServer) return;

        remainingTime = initialTime;
    }

    private IEnumerator TimerCoroutine()
    {
        while (remainingTime > 0)
        {
            yield return new WaitForSeconds(1f);
            remainingTime -= 1f;
        }

        isRunning = false;

        if (taggedPlayerIdentity != null)
        {
            MyNetworkPlayer taggedPlayer = taggedPlayerIdentity.GetComponent<MyNetworkPlayer>();
            taggedPlayer.KillTaggedPlayer();
            RpcSelectRandomTaggedPlayer();
        }
    }

    private void OnTimeChanged(float oldTime, float newTime)
    {
        UIManager.instance.UpdateTimerDisplay(newTime);
    }

    [ClientRpc]
    private void RpcSelectRandomTaggedPlayer()
    {
        if (!isLocalPlayer) return;

        MyNetworkManager networkManager = FindObjectOfType<MyNetworkManager>();
        int randomIndex = Random.Range(0, networkManager.players.Count);
        MyNetworkPlayer taggedPlayer = networkManager.players[randomIndex].GetComponent<MyNetworkPlayer>();
        taggedPlayer.SetDisplayColor(Color.red);
        taggedPlayer.isTagged = true;
        taggedPlayerIdentity = taggedPlayer.GetComponent<NetworkIdentity>();
        taggedPlayerIdentity.AssignClientAuthority(connectionToClient);
        StartTimer();
        uiManager.UpdateTimerDisplay(RemainingTime);
    }

}

