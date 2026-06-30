using System;
using UnityEngine;

public class GoogleCredentialManagerBridge : MonoBehaviour
{
    private static GoogleCredentialManagerBridge _instance;
    private Action<bool, string, string> _callback;

    public static void SignIn(string webClientId, Action<bool, string, string> callback)
    {
        if (_instance == null)
        {
            var go = new GameObject("GoogleCredentialManagerBridge");
            DontDestroyOnLoad(go);
            _instance = go.AddComponent<GoogleCredentialManagerBridge>();
        }

        _instance._callback = callback;

#if UNITY_ANDROID && !UNITY_EDITOR
        using (var pluginClass = new AndroidJavaClass("com.myproject.googlelogin.GoogleCredentialManagerPlugin"))
        {
            pluginClass.CallStatic("signIn", webClientId, "GoogleCredentialManagerBridge");
        }
#else
        Debug.LogWarning("GoogleCredentialManagerBridge is only supported on Android");
        callback?.Invoke(false, "Not supported on this platform", null);
#endif
    }

    // Called from Java via UnitySendMessage
    private void OnGoogleCredentialResult(string result)
    {
        if (result.StartsWith("SUCCESS:"))
        {
            string token = result.Substring("SUCCESS:".Length);
            Debug.Log("[GoogleCredentialBridge] Sign-in success");
            _callback?.Invoke(true, null, token);
        }
        else if (result.StartsWith("ERROR:"))
        {
            string error = result.Substring("ERROR:".Length);
            Debug.LogError($"[GoogleCredentialBridge] Sign-in failed: {error}");
            _callback?.Invoke(false, error, null);
        }

        _callback = null;
    }
}
