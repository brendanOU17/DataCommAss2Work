using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CountdownTimer : NetworkBehaviour
{
    [SerializeField] private float initialTime = 90f;

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

        MyNetworkManager networkManager = FindObjectOfType<MyNetworkManager>();
        foreach (GameObject player in networkManager.players)
        {
            player.GetComponent<MyNetworkPlayer>().KillTaggedPlayer();
        }
    }

    private void OnTimeChanged(float oldTime, float newTime)
    {
        UIManager.instance.UpdateTimerDisplay(newTime);
    }
}
