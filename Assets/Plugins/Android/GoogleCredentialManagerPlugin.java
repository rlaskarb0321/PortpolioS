package com.myproject.googlelogin;

import android.app.Activity;
import android.content.Intent;

import com.unity3d.player.UnityPlayer;

public class GoogleCredentialManagerPlugin {
    static String sGameObjectName;

    public static void signIn(String webClientId, String gameObjectName) {
        sGameObjectName = gameObjectName;
        Activity activity = UnityPlayer.currentActivity;

        Intent intent = new Intent(activity, GoogleSignInActivity.class);
        intent.putExtra("webClientId", webClientId);
        activity.startActivity(intent);
    }

    static void sendResult(String result) {
        if (sGameObjectName != null) {
            UnityPlayer.UnitySendMessage(sGameObjectName, "OnGoogleCredentialResult", result);
        }
    }
}
