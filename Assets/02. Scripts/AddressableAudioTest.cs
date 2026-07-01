using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

/// <summary>
/// Remote_Stage_BGM Addressable 로드 테스트용 — 확인 후 삭제할 것
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class AddressableAudioTest : MonoBehaviour
{
    [SerializeField] private string _address = "call_to_adventure_loop";
    [SerializeField] private bool _printAllKeys = true;

    private AudioSource _audioSource;
    private AsyncOperationHandle<AudioClip> _handle;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private IEnumerator Start()
    {
        yield return Addressables.InitializeAsync();

        if (_printAllKeys)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("[AddressableAudioTest] === Registered Keys ===");
            foreach (var locator in Addressables.ResourceLocators)
                foreach (var key in locator.Keys)
                    sb.AppendLine(key.ToString());
            Debug.Log(sb.ToString());
        }

        Debug.Log($"[AddressableAudioTest] Loading: {_address}");

        _handle = Addressables.LoadAssetAsync<AudioClip>(_address);
        yield return _handle;

        if (_handle.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log($"[AddressableAudioTest] Load success: {_handle.Result.name}");
            _audioSource.clip = _handle.Result;
            _audioSource.loop = true;
            _audioSource.Play();
        }
        else
        {
            Debug.LogError($"[AddressableAudioTest] Load failed: {_handle.OperationException}");
        }
    }

    private void OnDestroy()
    {
        if (_handle.IsValid())
            Addressables.Release(_handle);
    }
}
