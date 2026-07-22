using Cinemachine;
using Cysharp.Threading.Tasks;
using Fusion;
using UnityEngine;

public class PlayerCharacterSpawnFactory : MonoBehaviour
{
    [SerializeField] private GameObject cameraPrefab;

    public async UniTask SpawnPlayerCharacter(GameObject playerModel, Vector3 spawnPos, PlayerRef player)
    {
        var playerInstance = BootstrapSceneInstance.Instance.NetworkRunner.SpawnAsync
        (
            playerModel,
            spawnPos,
            inputAuthority: player
        );
        
        await UniTask.WaitUntil(() => playerInstance.IsSpawned == true);
        
        SetFollowCamera(playerInstance);
    }

    private void SetFollowCamera(NetworkSpawnOp player)
    {
        var vCam = Instantiate(cameraPrefab).GetComponent<CinemachineVirtualCamera>();

        vCam.Follow = player.Object.transform;
        vCam.LookAt = player.Object.transform;
    }
}
