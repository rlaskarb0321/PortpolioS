using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class GameTypeManagerBase : MonoBehaviour
{
    public abstract UniTask DoInit();
}
