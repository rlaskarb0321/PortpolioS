using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MultiplayStageElementView : MonoBehaviour
{
    public UnityEvent OnClickElementView;
    
    [SerializeField] private Text sessionName;

    private Button elementButton;

    public void InitElementView()
    {
        
    }
    
    private void Awake()
    {
        elementButton = GetComponent<Button>();
    }

    private void Start()
    {
        elementButton.onClick.AddListener(() => OnClickElementView.Invoke());
    }
}