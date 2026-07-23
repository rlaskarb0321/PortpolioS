using Cinemachine;
using Fusion;
using UnityEngine;

public class PlayerCameraRig : NetworkBehaviour
{
    [SerializeField] private GameObject followCamPrefab;

    private CinemachineVirtualCamera vCam;

    public override void Spawned()
    {
        base.Spawned();
        var listener = GetComponent<AudioListener>();
        if (listener != null)
            listener.enabled = HasInputAuthority;

        if (HasInputAuthority == false)
            return;

        vCam = Instantiate(followCamPrefab).GetComponent<CinemachineVirtualCamera>();
        vCam.Follow = transform;
        vCam.LookAt = transform;
    }
}
