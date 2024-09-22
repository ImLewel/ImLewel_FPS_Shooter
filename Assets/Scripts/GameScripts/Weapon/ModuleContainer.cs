using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ModuleContainer : MonoBehaviour
{
  [SerializeField]
  public Module config;
  [SerializeField]
  public List<Module.ModuleType> moduleHoldersTypes;
  [SerializeField]
  public List<GameObject> moduleHolders;

  public void SetModule(GameObject newModule)
  {
    Module.ModuleType mType = newModule.GetComponent<Module>().Type;
    if (moduleHoldersTypes.Contains(mType))
    {
      GameObject holder = moduleHolders[moduleHoldersTypes.IndexOf(mType)];
      Destroy(holder.transform.GetChild(0).gameObject);
      Instantiate(newModule, holder.transform.position, holder.transform.rotation);
      newModule.transform.parent = holder.transform;
    }
  }

  public void Start()
  {
  }

  public void Update()
  {

  }
}
