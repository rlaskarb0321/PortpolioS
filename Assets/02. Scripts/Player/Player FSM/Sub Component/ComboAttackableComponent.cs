using System;
using UnityEngine;

public class ComboAttackableComponent : MonoBehaviour
{
    public event Action ComboInput;
    
    [SerializeField] private EComboInputtableState canInputState = EComboInputtableState.None;
    [SerializeField] private EComboInputState currentInputState = EComboInputState.None;

    private bool isComboEnd;
    
    public EComboInputtableState CanInput => canInputState;
    public EComboInputState CurrentInputState => currentInputState;
    public bool IsComboEnd => isComboEnd;

    public void HandleComboInput()
    {
        ComboInput?.Invoke();
    }
    
    public void SetCanInput()
    {
        canInputState = EComboInputtableState.CanInput;
    }
    
    public void SetCannotInput()
    {
        canInputState = EComboInputtableState.CannotInput;
    }

    public void SetComboReceived()
    {
        if (currentInputState == EComboInputState.InputReceived)
            return;

        currentInputState = EComboInputState.InputReceived;
    }

    public void InitComboInputState()
    {
        isComboEnd = false;
        canInputState = EComboInputtableState.CannotInput;
        currentInputState = EComboInputState.InputNonReceived;
    }

    public void SetComboEnd()
    {
        isComboEnd = true;
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
