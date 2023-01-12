using UnityEngine;

public class FpsTarget : MonoBehaviour {
  [SerializeField] private int target = 120;

  void Awake() {
    QualitySettings.vSyncCount = 0;
    Application.targetFrameRate = target;
  }

  void Update() {
    if (Application.targetFrameRate != target)
      Application.targetFrameRate = target;
  }
}
