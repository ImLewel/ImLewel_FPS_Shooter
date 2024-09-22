using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class WeaponModification : MonoBehaviour
{
  public GameObject prefab;
  GameObject instance;
  public List<GameObject> modules;
  public List<string> names;
  public TMPro.TextMeshProUGUI mods;
  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {
    if (Input.GetKeyDown(KeyCode.F))
    {
      instance = Instantiate(prefab, this.transform.parent, false);
      instance.transform.rotation = Quaternion.Euler(1f, 90f, 1f);
      instance.transform.localScale = Vector3.one * 10f;
      if (instance != null)
      {
        GetAllModules(instance);
        names.ForEach(ss => mods.text += ss + "\n");
      }
    }
  }

  void GetAllModules(GameObject obj)
  {
    ModuleContainer container = obj.GetComponent<ModuleContainer>();
    if (container != null)
    {
      names.Add(container.name);
      modules.Add(container.transform.parent.gameObject);
      foreach (GameObject holder in container.moduleHolders)
      {
        GetAllModules(holder.transform.GetChild(0).gameObject);
      }
    }
  }
}
