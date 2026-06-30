using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class SubManagerBase : MonoBehaviour
{
    public abstract UniTask DoInit();
}
