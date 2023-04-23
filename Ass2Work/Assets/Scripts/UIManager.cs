using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private GameObject winnerPanel;
    [SerializeField] private TextMeshProUGUI winnerText;
    [SerializeField] private List<TextMeshProUGUI> playerNames;
    [SerializeField] private List<TextMeshProUGUI> playerScores;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }


    public void UpdateTimerDisplay(float remainingTime)
    {
        int minutes = (int)(remainingTime / 60);
        int seconds = (int)(remainingTime % 60);

        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    public void ShowWinner(string winnerName)
    {
        winnerPanel.SetActive(true);
        winnerText.text = $"{winnerName} wins!";
    }

    public int GetMaxPlayers()
    {
        return MyNetworkManager.MaxPlayers;
    }
    public void SubmitNameCoroutine(string playerName)
    {
        StartCoroutine(SubmitName(playerName));
    }

    public IEnumerator SubmitName(string playerName)
    {
        if (string.IsNullOrWhiteSpace(playerName))
        {
            Debug.LogWarning("Player name cannot be empty");
            yield break;
        }

        MyNetworkPlayer localPlayer = null;
        while (localPlayer == null)
        {
            if (NetworkClient.connection != null && NetworkClient.connection.identity != null)
            {
                localPlayer = NetworkClient.connection.identity.GetComponent<MyNetworkPlayer>();
            }
            yield return new WaitForSeconds(0.1f);
        }

        if (localPlayer != null)
        {
            localPlayer.CmdChangeName(playerName);
        }
        else
        {
            Debug.LogWarning("Local player has not been initialized yet.");
        }
    }



    public void UpdatePlayerInfo(int playerIndex, string playerName, int playerScore)
    {
        if (playerIndex < MyNetworkManager.MaxPlayers)
        {
            if (playerNames[playerIndex] != null)
            {
                playerNames[playerIndex].text = $"{playerIndex + 1} {playerName}:";
            }
            else
            {
                Debug.LogError($"PlayerNames[{playerIndex}] is not assigned in the UIManager script.");
            }

            if (playerScores[playerIndex] != null)
            {
                playerScores[playerIndex].text = $"{playerScore:00}";
            }
            else
            {
                Debug.LogError($"PlayerScores[{playerIndex}] is not assigned in the UIManager script.");
            }
        }
    }


    public void ResetPlayerInfo()
    {
        foreach (var playerName in playerNames)
        {
            playerName.text = "";
        }

        foreach (var playerScore in playerScores)
        {
            playerScore.text = "";
        }
    }
}


