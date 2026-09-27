using System;
using System.Collections.Generic;
using Game.Bank.States;
using Game.States;
using ianco99.ToolBox.Events;
using ianco99.ToolBox.Services;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Bank
{
    public class DepositMinigameController : MonoBehaviour
    {
        [SerializeField] private int playerID;

        [SerializeField]
        private GameObject[] bagsVisuals;
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private Animator animator;
        EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();

        private int GetCurrentBags => currentBagsRemaining;
        
        private enum States
        {
            Start,
            Left,
            Down,
            Right
        }

        private enum Triggers
        {
            PressedUp,
            PressedLeft,
            PressedDown,
            PressedRight
        }


        private int currentBagsRemaining;

        private FSM fsm;

        private void Start()
        {
            EventBus.Subscribe<StartMinigameEvent>(OnStartMinigame);


            Dictionary<string, int> triggers = new Dictionary<string, int>()
            {
                { Triggers.PressedUp.ToString(), (int)Triggers.PressedUp },
                { Triggers.PressedLeft.ToString(), (int)Triggers.PressedLeft },
                { Triggers.PressedDown.ToString(), (int)Triggers.PressedDown },
                { Triggers.PressedRight.ToString(), (int)Triggers.PressedRight }
            };

            fsm = new FSM(Enum.GetValues(typeof(States)).Length, triggers);

            fsm.AddState<StartMinigameState>((int)States.Start, constructionParameters: new object[] {GetCurrentBags,bagsVisuals,animator });

        }

        private void OnStartMinigame(in StartMinigameEvent callback)
        {
            if (playerID == callback.playerID)
            {
                StartMinigame(callback.bagsNumber);
            }
        }

        private void StartMinigame(int bagsNumber)
        {
            fsm.ForceState((int)States.Left);
        }
    }
}