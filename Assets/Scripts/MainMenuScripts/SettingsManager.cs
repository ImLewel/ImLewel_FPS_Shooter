using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour {
  private TMP_Dropdown QualityDropdown;
  private Slider FpsSlider;
  private void Start() {
    QualityDropdown = transform.Find("QualityDropdown").GetComponent<TMP_Dropdown>();
    FpsSlider = transform.Find("FpsSlider").GetComponent<Slider>();
    ChangeFps();
  }
  public void OnVSyncBtnClicked() {
    if (QualitySettings.vSyncCount != 1)
      QualitySettings.vSyncCount = 1;
    else
      QualitySettings.vSyncCount = 0;
  }
  public void ChangeFps() {
    Application.targetFrameRate = (int)FpsSlider.value;
  }

  public void ChangeQuality() {
    QualitySettings.SetQualityLevel(QualityDropdown.value, true);
    Application.targetFrameRate = (int)FpsSlider.value;
  }

}
