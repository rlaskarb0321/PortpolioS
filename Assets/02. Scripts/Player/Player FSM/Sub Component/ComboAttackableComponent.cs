using UnityEngine;

public class ComboAttackableComponent : MonoBehaviour
{
    [SerializeField] private EComboInputtableState canInputState = EComboInputtableState.None;
    [SerializeField] private EComboInputState currentInputState = EComboInputState.None;
    
    public EComboInputtableState CanInput => canInputState;
    public EComboInputState CurrentInputState => currentInputState;

    public void HandleComboInput()
    {
        if (currentInputState != EComboInputState.InputReceived)
        {
                        
            return;
        }
    }
    
    public void SetCanInput()
    {
        canInputState = EComboInputtableState.CanInput;
    }
    
    public void SetCannotInput()
    {
        canInputState =  EComboInputtableState.CannotInput;
    }

    public void SetComboReceived()
    {
        if (currentInputState == EComboInputState.InputReceived)
            return;

        currentInputState = EComboInputState.InputReceived;
    }

    public void InitComboInputState()
    {
        currentInputState = EComboInputState.InputNonReceived;
    }
}

public enum EComboInputtableState
{
    None,
    CanInput,
    CannotInput,
    Count
}

public enum EComboInputState
{
    None,
    InputReceived,
    InputNonReceived,
    Count
}
