using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private GameObject winnerPanel;
    [SerializeField] private TextMeshProUGUI winnerText;
    [SerializeField] private List<TextMeshProUGUI> playerNames;
    [SerializeField] private List<TextMeshProUGUI> playerScores;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void CreateInstance()
    {
        GameObject uiManagerGameObject = new GameObject("UIManager");
        instance = uiManagerGameObject.AddComponent<UIManager>();
        DontDestroyOnLoad(uiManagerGameObject);
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

    public void UpdatePlayerInfo(int playerIndex, string playerName, int playerScore)
    {
        if (playerIndex < MyNetworkManager.MaxPlayers)
        {
            playerNames[playerIndex].text = $"{playerIndex + 1} {playerName}:";
            playerScores[playerIndex].text = $"{playerScore:00}";
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


