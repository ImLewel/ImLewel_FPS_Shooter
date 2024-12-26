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
  public void OnPlayBtnClicked() => SceneManager.LoadScene("GameModeSelection", LoadSceneMode.Single);
  public void OnQuitBtnClicked() => Application.Quit();
  public void OnSettingsBtnClicked() {
    if (settings.activeInHierarchy) settings.SetActive(false);
    else settings.SetActive(true);
  }
}
