using Unity.Netcode;

public class CameraAssign : NetworkBehaviour
{
  public override void OnNetworkSpawn()
  {
    if (!IsOwner)
    {
      gameObject.SetActive(false);
    }
  }
}
