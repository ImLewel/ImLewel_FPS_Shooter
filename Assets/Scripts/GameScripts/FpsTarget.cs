using UnityEngine;

public class FpsTarget : MonoBehaviour {
  public int target = 144;

  private void Awake() {
    QualitySettings.vSyncCount = 0;
    Application.targetFrameRate = target;
  }

  private void Start() {
    DontDestroyOnLoad(gameObject);
  }

  private void Update() {
    if (Application.targetFrameRate != target)
      Application.targetFrameRate = target;
  }

  public void UpdateValue(int num) => QualitySettings.vSyncCount = num;
}
