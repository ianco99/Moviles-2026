using System;
using Game;
using Game.Bank;
using ianco99.ToolBox.Events;
using ianco99.ToolBox.Services;
using UnityEngine;

public class DepositTrigger : MonoBehaviour
{
    private static readonly int ColorId = Shader.PropertyToID("_Color");

    private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();

    [SerializeField] private Color disabledColor = new Color(1f, 0.1f, 0.1f, 1f);

    private bool IsEmpty = true;
    private bool isDisabled;
    private int occupier;

    // Disabled by the difficulty: the zone turns red and trucks can't deposit here
    public void Disable()
    {
        isDisabled = true;

        Renderer zoneRenderer = GetComponent<Renderer>();
        if (zoneRenderer == null) return;

        // Property block so the shared material (and the other banks) stay untouched
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        zoneRenderer.GetPropertyBlock(block);
        block.SetColor(ColorId, disabledColor);
        zoneRenderer.SetPropertyBlock(block);
    }

    private void Start()
    {
        EventBus.Subscribe<EnterMinigameAcceptEvent>(OnEnterMinigame);
    }

    private void OnDestroy()
    {
        EventBus.UnSubscribe<EnterMinigameAcceptEvent>(OnEnterMinigame);
    }

    private void OnEnterMinigame(in EnterMinigameAcceptEvent callback)
    {
        IsEmpty = false;
        occupier = callback.playerID;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(isDisabled || other.GetComponent<TruckController>() == null)
            return;
        
        if (IsEmpty)
        {
            int pID = other.GetComponent<TruckController>().playerID;
            EventBus.Raise<EnterMinigameRequestEvent>(pID);
        }
    }
}
