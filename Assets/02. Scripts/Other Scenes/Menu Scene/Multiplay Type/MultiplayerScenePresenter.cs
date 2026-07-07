using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class MultiplayerScenePresenter : SubManagerBase
{
    [SerializeField] private List<AssetLabelReference> definitionLabel;
    
    public override UniTask DoInit()
    {
        Debug.Log($"[MultiplayerSceneGameMode] DoInit");
        string targetAddress = SetTargetAddress().ToString();
        
        return UniTask.CompletedTask;
    }

    private System.Text.StringBuilder SetTargetAddress()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        for (int i = 0; i < definitionLabel.Count; i++)
        {
            sb.Append(definitionLabel[i].labelString);
            if (i != definitionLabel.Count - 1)
            {
                sb.Append("/");
            }
        }

        return sb;
    }
}
