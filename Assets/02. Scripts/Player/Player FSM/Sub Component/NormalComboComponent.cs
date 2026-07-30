using Fusion;
using UnityEngine;

public class NormalComboComponent : NetworkBehaviour
{
    // ─── Networked Properties ────────
    [Networked] private int SwingStartTick { get; set; }
    [Networked] public NetworkBool QueuedNext { get; set; }

    public float Elapsed => ((int)Runner.Tick - SwingStartTick) * Runner.DeltaTime;

    public void BeginSwing()
    {
        SwingStartTick = (int)Runner.Tick;
        QueuedNext     = false;
    }
}
