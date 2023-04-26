using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class CustomNetworkManagerHud : MonoBehaviour
{
    public NetworkManager networkManager;
    public Button hostButton;
    public Button joinButton;
    public Button stopHostButton;
    public Button stopClientButton;


    private void Start()
    {
        hostButton.onClick.AddListener(StartHost);
        joinButton.onClick.AddListener(StartClient);
        stopHostButton.onClick.AddListener(StopHost);
        stopClientButton.onClick.AddListener(StopClient);

    }

    public void StartHost()
    {
        networkManager.StartHost();
        hostButton.gameObject.SetActive(false);
        joinButton.gameObject.SetActive(false);
        stopHostButton.gameObject.SetActive(true);
    }

    public void StartClient()
    {
        networkManager.StartClient();
        hostButton.gameObject.SetActive(false);
        joinButton.gameObject.SetActive(false);
        stopClientButton.gameObject.SetActive(true);
    }

    public void StartSever()
    {
        networkManager.StartServer();
        hostButton.gameObject.SetActive(false);
        joinButton.gameObject.SetActive(false);

    }

    public void StopHost()
    {
        networkManager.StopHost();
        hostButton.gameObject.SetActive(true);
        joinButton.gameObject.SetActive(true);
        stopHostButton.gameObject.SetActive(false);
    }

    public void StopClient()
    {

        networkManager.StopClient();
        hostButton.gameObject.SetActive(true);
        joinButton.gameObject.SetActive(true);
        stopClientButton.gameObject.SetActive(false);

    }

    public void StopServer()
    {
        networkManager.StopServer();
        hostButton.gameObject.SetActive(true);
        joinButton.gameObject.SetActive(true);

    }
}
