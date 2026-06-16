using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AppLaunchManager : MonoBehaviour
{
    [SerializeField] private AppLaunchModule[] appLaunchModules;
    [SerializeField] private AppLaunchModule lastLaunchModule;
    
    private void Start()
    {
        
    }

    #if UNITY_EDITOR
    private void OnValidate()
    {
        if (appLaunchModules == null) return;

        for (int i = 0; i < appLaunchModules.Length; i++)
        {
            if (appLaunchModules[i].moduleName != string.Empty) continue;
            if (appLaunchModules[i].appLaunchModuleBase == null) continue;

            appLaunchModules[i].moduleName = appLaunchModules[i].appLaunchModuleBase.ModuleName;
        }

        if (lastLaunchModule.moduleName == string.Empty && lastLaunchModule.appLaunchModuleBase != null)
        {
            lastLaunchModule.moduleName = lastLaunchModule.appLaunchModuleBase.ModuleName;
        }
    }
    #endif
}

[System.Serializable]
public struct AppLaunchModule
{
    public string moduleName;
    public AppLaunchModuleBase appLaunchModuleBase;
    public float delay;
    
}