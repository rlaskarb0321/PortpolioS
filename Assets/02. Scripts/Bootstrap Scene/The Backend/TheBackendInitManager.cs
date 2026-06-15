using UnityEngine;
using BackEnd;

public class TheBackendInitManager : MonoBehaviour
{
    void Start()
    {
        var bro = Backend.Initialize();

        if(bro.IsSuccess())
        {
            Debug.Log("[TheBackendInitManager] Successfully Init: " + bro); // If successful, statusCode 204 Success
        } 
        else
        {
            Debug.LogError("[TheBackendInitManager] Failed to Init: " + bro); // In case of failure, a status code in the 400 range error occurs.
        }
    }
}
