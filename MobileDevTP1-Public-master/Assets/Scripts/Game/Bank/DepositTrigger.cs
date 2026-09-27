using System;
using Game;
using Game.Bank;
using ianco99.ToolBox.Events;
using ianco99.ToolBox.Services;
using UnityEngine;

public class DepositTrigger : MonoBehaviour
{
    private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();
    private bool IsEmpty = true;
    private int occupier;
    private void Start()
    {
        EventBus.Subscribe<EnterMinigameAcceptEvent>(OnEnterMinigame);
    }

    private void OnEnterMinigame(in EnterMinigameAcceptEvent callback)
    {
        IsEmpty = false;
        occupier = callback.playerID;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsEmpty)
        {
            int pID = other.GetComponent<TruckController>().playerID;
            EventBus.Raise<EnterMinigameRequestEvent>(pID);
        }
    }
}
