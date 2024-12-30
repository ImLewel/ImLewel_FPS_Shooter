using System.Collections;
using System.Collections.Generic;
using Unity.Services.Core;
using Unity.Services.Authentication;
using UnityEngine;
using TMPro;
using UnityEditor;
using UnityEngine.SceneManagement;
using Unity.VisualScripting.Antlr3.Runtime;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.VisualScripting;

public class Init : NetworkBehaviour
{
  [SerializeField] TMP_InputField nameInputField;
  [SerializeField] TMP_InputField ipInputField;
  [SerializeField] TMP_InputField portInputField;
  [SerializeField] UnityTransport transport;
  [SerializeField] NetworkManager networkManager;
  string username;
  string userId;
  // Start is called before the first frame update
  async void Start()
  {
    await UnityServices.InitializeAsync();
    if (UnityServices.State == ServicesInitializationState.Initialized)
    {
      AuthenticationService.Instance.SignedIn += OnSignedIn;

      await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }
  }

  private void OnSignedIn()
  {
    userId = AuthenticationService.Instance.PlayerId;
  }

  public void OnHostOrJoinButtonClick()
  {
    username = nameInputField.text;
    if (username == "")
    {
      username = "Player" + (int)Random.Range(0f, 256f);
    }
    PlayerPrefs.SetString("username", username);
    Debug.Log($"{userId} signed as {username}");
  }

  public void OnHostGame()
  {
    networkManager.enabled = true;
    networkManager.StartHost();
    if (AuthenticationService.Instance.IsSignedIn)
    {
      NetworkManager.Singleton.SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
    }
  }

  public void OnJoinGame()
  {
    transport.ConnectionData.Address = 
      ipInputField.text == "" ? "127.0.0.1": ipInputField.text;
    transport.ConnectionData.Port = 
      portInputField.text == "" ? (ushort)7777: portInputField.text.ConvertTo<ushort>();

    networkManager.enabled = true;
    Debug.Log($"Joining {transport.ConnectionData.Address}:{transport.ConnectionData.Port}");
    networkManager.StartClient();
  }

  // Update is called once per frame
  void Update()
  {

  }
}
