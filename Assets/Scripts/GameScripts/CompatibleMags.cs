using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Compatible mags", menuName = "My scriptable objects")]
public class CompatibleMags : ScriptableObject
{
  public Dictionary<WeaponKind, List<GameObject>> list;
  public enum WeaponKind
  {
    M4A4,
    GL17,
  }
  [SerializeField]
  private List<GameObject> M4A4_Mags;
  [SerializeField]
  private List<GameObject> GL17_Mags;
  
  public void Create()
  {
    list = new() {
      { WeaponKind.M4A4, M4A4_Mags},
      { WeaponKind.GL17, GL17_Mags},
    };
    Console.WriteLine(list);
  }
}
