using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour {
  GameObject settings;
  private void Start() {
    if (gameObject.name == "SettingsBtn") {
      settings = GameObject.Find("SettingsHolder");
      settings.SetActive(false);
    }
  }
  public void OnPlayBtnClicked() => SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
  public void OnQuitBtnClicked() => Application.Quit();
  public void OnSettingsBtnClicked() {
    if (settings.active) settings.SetActive(false);
    else settings.SetActive(true);
  }
}
