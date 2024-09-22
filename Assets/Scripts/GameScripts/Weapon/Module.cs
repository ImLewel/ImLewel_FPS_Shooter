using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[CreateAssetMenu(fileName = "Module", menuName = "My Scriptable Objects/Weapon/Module")]
public class Module : ScriptableObject
{
  public enum ModuleType
  {
    None,
    PistolHandle,
    UpperReceiver,
    LowerReceiver,
    Magazine,
    Barrel,
    LoadingHandle,
    StockTube,
    Stock,
    TacticHandle,
    Light,
  }

  public enum PlatformType
  {
    None,
    All,
    AR,
    AR15,
    AR10,
    AK,
    AK5,
    AK7,
    Glock,
  }

  [Header("Allowed platforms")]
  [SerializeField]
  internal List<PlatformType> Platforms = new() { (PlatformType.None) };

  [Header("Module Type:")]
  [SerializeField]
  internal ModuleType Type = ModuleType.None;

  [Header("Can be attached to:")]
  [SerializeField]
  internal List<ModuleType> AttachableTo = new List<ModuleType>() { ModuleType.None };
  

  [Header("Can accept attachments:")]
  [SerializeField]
  internal List<ModuleType> Attachments = new() { (ModuleType.None) };
}
