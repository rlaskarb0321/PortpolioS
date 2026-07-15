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

    public void InitElementView(in MultiplayStageDefinition inDefinition)
    {
        sessionName.text = inDefinition.sessionName.ToString();
    }
    
    private void Awake()
    {
        elementButton = GetComponent<Button>();
    }
}