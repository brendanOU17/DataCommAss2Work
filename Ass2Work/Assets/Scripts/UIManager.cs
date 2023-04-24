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
    [SerializeField] private TMP_InputField playerNameInputField;
    private MyNetworkPlayer localPlayer;
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
    //private void Start()
    //{
    //    //// Find the local player and subscribe to the OnNameChanged event
    //    //localPlayer = FindObjectOfType<MyNetworkPlayer>();
    //    //if (localPlayer != null && localPlayer.isLocalPlayer)
    //    //{
    //    //    localPlayer.OnNameChanged += HandleNameChanged;
    //    //}
    //}

    //private void OnDestroy()
    //{
    //    if (localPlayer != null && localPlayer.isLocalPlayer)
    //    {
    //        localPlayer.OnNameChanged -= HandleNameChanged;
    //    }
    //}

    //public void OnNameInputFieldValueChanged(string newName)
    //{
        
    //        localPlayer.CmdChangeName(newName);
        
    //}

    //private void HandleNameChanged(string newName)
    //{
    //    if (localPlayer != null && localPlayer.isLocalPlayer)
    //    {
    //        int playerIndex = localPlayer.connectionToClient.connectionId % MyNetworkManager.MaxPlayers;

    //        if (playerIndex < MyNetworkManager.MaxPlayers && playerNames[playerIndex] != null)
    //        {
    //            playerNames[playerIndex].text = $"{playerIndex + 1} {newName}:";
    //        }
    //        else
    //        {
    //            Debug.LogError($"PlayerNames[{playerIndex}] is not assigned in the UIManager script.");
    //        }
    //    }
    //}

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

    //public void DisablePlayerNameInputField() 
    //{
    //    playerNameInputField.interactable = false;
    //}

    //public void SubmitName(string playerName)
    //{
    //    FindObjectOfType<MyNetworkManager>().SubmitPlayerName(playerName);
    //}

    //public void UpdatePlayerInfo(int playerIndex, string playerName, int playerScore)
    //{
    //    if (playerIndex < MyNetworkManager.MaxPlayers)
    //    {
    //        if (playerNames[playerIndex] != null)
    //        {
    //            playerNames[playerIndex].text = $"{playerIndex + 1} {playerName}:";
    //        }
    //        else
    //        {
    //            Debug.LogError($"PlayerNames[{playerIndex}] is not assigned in the UIManager script.");
    //        }

    //        if (playerScores[playerIndex] != null)
    //        {
    //            playerScores[playerIndex].text = $"{playerScore:00}";
    //        }
    //        else
    //        {
    //            Debug.LogError($"PlayerScores[{playerIndex}] is not assigned in the UIManager script.");
    //        }
    //    }
    //}

    //public void ResetPlayerInfo()
    //{
    //    foreach (var playerName in playerNames)
    //    {
    //        playerName.text = "";
    //    }

    //    foreach (var playerScore in playerScores)
    //    {
    //        playerScore.text = "";
    //    }
    //}
}
