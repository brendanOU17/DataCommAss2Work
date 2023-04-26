using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CountdownTimer : NetworkBehaviour
{
    [SerializeField] private float initialTime = 120f; // 2 minutes

    [SyncVar(hook = nameof(OnTimeChanged))]
    private float remainingTime;

    private bool isRunning = false;

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

        MyNetworkPlayer winner = null;
        foreach (var conn in NetworkServer.connections.Values)
        {
            MyNetworkPlayer player = conn.identity.GetComponent<MyNetworkPlayer>();
            if (player.displayColor == Color.red) // Check if the player has authority over the object (based on display color)
            {
                winner = player;
                break;
            }
        }
        if (winner != null)
        {
            winner.RpcDeclareWinner(winner.gameObject);
        }
    }

    private void OnTimeChanged(float oldTime, float newTime)
    {
        UIManager.instance.UpdateTimerDisplay(newTime);
    }
}
