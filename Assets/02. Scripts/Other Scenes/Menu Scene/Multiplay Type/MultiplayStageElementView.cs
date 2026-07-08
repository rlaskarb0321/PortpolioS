using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MultiplayStageElementView : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Text sessionName;

    private Button elementButton;
    
    public Button ElementButton => elementButton;

    public void InitElementView(MultiplayStageDefinition inDefinition)
    {
        sessionName.text = inDefinition.sessionName;
    }
    
    private void Awake()
    {
        elementButton = GetComponent<Button>();
    }
}