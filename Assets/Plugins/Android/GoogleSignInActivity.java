package com.myproject.googlelogin;

import android.app.Activity;
import android.os.Bundle;
import android.os.CancellationSignal;
import android.util.Log;

import androidx.credentials.CredentialManager;
import androidx.credentials.CredentialManagerCallback;
import androidx.credentials.GetCredentialRequest;
import androidx.credentials.GetCredentialResponse;
import androidx.credentials.exceptions.GetCredentialException;

import com.google.android.libraries.identity.googleid.GetSignInWithGoogleOption;
import com.google.android.libraries.identity.googleid.GoogleIdTokenCredential;

import java.util.concurrent.Executors;

public class GoogleSignInActivity extends Activity {
    private static final String TAG = "GoogleSignInActivity";

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        String webClientId = getIntent().getStringExtra("webClientId");
        if (webClientId == null) {
            Log.e(TAG, "webClientId is null");
            GoogleCredentialManagerPlugin.sendResult("ERROR:webClientId is null");
            finish();
            return;
        }

        startCredentialManager(webClientId);
    }

    private void startCredentialManager(String webClientId) {
        GetSignInWithGoogleOption signInOption = new GetSignInWithGoogleOption.Builder(webClientId)
                .build();

        GetCredentialRequest request = new GetCredentialRequest.Builder()
                .addCredentialOption(signInOption)
                .build();

        CredentialManager credentialManager = CredentialManager.create(this);

        credentialManager.getCredentialAsync(
                this,
                request,
                new CancellationSignal(),
                Executors.newSingleThreadExecutor(),
                new CredentialManagerCallback<GetCredentialResponse, GetCredentialException>() {
                    @Override
                    public void onResult(GetCredentialResponse result) {
                        try {
                            GoogleIdTokenCredential credential =
                                    GoogleIdTokenCredential.createFrom(result.getCredential().getData());
                            String idToken = credential.getIdToken();
                            Log.i(TAG, "Google sign-in success");
                            GoogleCredentialManagerPlugin.sendResult("SUCCESS:" + idToken);
                        } catch (Exception e) {
                            Log.e(TAG, "Failed to parse credential: " + e.getMessage());
                            GoogleCredentialManagerPlugin.sendResult("ERROR:" + e.getMessage());
                        }
                        finish();
                    }

                    @Override
                    public void onError(GetCredentialException e) {
                        Log.e(TAG, "Google sign-in failed: " + e.getType() + " - " + e.getMessage());
                        GoogleCredentialManagerPlugin.sendResult("ERROR:" + e.getType() + ": " + e.getMessage());
                        finish();
                    }
                }
        );
    }
}
