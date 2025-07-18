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

public class Init : MonoBehaviour
{
  [SerializeField] TMP_InputField nameInputField;
  [SerializeField] TMP_InputField ipInputField;
  [SerializeField] TMP_InputField portInputField;
}
