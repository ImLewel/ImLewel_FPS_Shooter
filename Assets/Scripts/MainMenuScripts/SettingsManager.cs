using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour {
  private FpsTarget fpsManager;
  private void Awake() {
      fpsManager = GameObject.Find("SceneMod").GetComponent<FpsTarget>();
  }
  public void OnVSyncBtnClicked() {
    if (QualitySettings.vSyncCount != 1)
      fpsManager.UpdateValue(0);
    else
      fpsManager.UpdateValue(1);
  }
  public void OnFpsSliderValueChange() {
    fpsManager.target = (int)gameObject.GetComponent<Slider>().value;
  }
}
