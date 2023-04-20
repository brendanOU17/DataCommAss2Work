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
    public Button serverButton;
    public Button stopHostButton;
    public Button stopClientButton;
    public Button stopServerButton;

    private void Start()
    {
        hostButton.onClick.AddListener(StartHost);
        joinButton.onClick.AddListener(StartClient);
        serverButton.onClick.AddListener(StartSever);
        stopHostButton.onClick.AddListener(StopHost);
        stopClientButton.onClick.AddListener(StopClient);
        stopServerButton.onClick.AddListener(StopServer);
    }

    public void StartHost()
    {
        networkManager.StartHost();
        hostButton.gameObject.SetActive(false);
        joinButton.gameObject.SetActive(false);
        serverButton.gameObject.SetActive(false);
        stopHostButton.gameObject.SetActive(true);
    }

    public void StartClient()
    {
        networkManager.StartClient();
        hostButton.gameObject.SetActive(false);
        joinButton.gameObject.SetActive(false);
        serverButton.gameObject.SetActive(false);
        stopClientButton.gameObject.SetActive(true);
    }

    public void StartSever()
    {
        networkManager.StartServer();
        hostButton.gameObject.SetActive(false);
        joinButton.gameObject.SetActive(false);
        serverButton.gameObject.SetActive(false);
        stopServerButton.gameObject.SetActive(true );

    }

    public void StopHost()
    {
        networkManager.StopHost();
        hostButton.gameObject.SetActive(true);
        joinButton.gameObject.SetActive(true);
        serverButton.gameObject.SetActive(true);
        stopHostButton.gameObject.SetActive(false);
    }

    public void StopClient()
    {

        networkManager.StopClient();
        hostButton.gameObject.SetActive(true);
        joinButton.gameObject.SetActive(true);
        serverButton.gameObject.SetActive(true);
        stopClientButton.gameObject.SetActive(false);

    }

    public void StopServer()
    {
        networkManager.StopServer();
        hostButton.gameObject.SetActive(true);
        joinButton.gameObject.SetActive(true);
        serverButton.gameObject.SetActive(true);
        stopServerButton.gameObject.SetActive(false);
    }
}
