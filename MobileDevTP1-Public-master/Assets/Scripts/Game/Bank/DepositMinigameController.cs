using System;
using System.Collections;
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

        [SerializeField] private GameObject[] bagsVisuals;
        [SerializeField] private Transform[] bagTargetPos;
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private Animator animator;
        [SerializeField] private Transform bagEndPos;
        [SerializeField] private GameObject minigameParent;
        [SerializeField] private GameObject[] visualPCPrompts;
        [SerializeField] private GameObject[] visualPhonePrompts;
        EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();

        private int GetCurrentBags => currentBagsRemaining;

        private enum States
        {
            Start,
            Left,
            Down,
            Right,
            End
        }

        public enum Triggers
        {
            ReadyForInput,
            PressedLeft,
            PressedDown,
            PressedRight
        }


        private int currentBagsRemaining;

        private FSM fsm;

        private void Start()
        {
            EventBus.Subscribe<StartMinigameEvent>(OnStartMinigame);
            EventBus.Subscribe<BagReachedEndEvent>(OnBagReachedEnd);
            EventBus.Subscribe<EndMinigameEvent>(OnEndMinigame);


            Dictionary<string, int> triggers = new Dictionary<string, int>()
            {
                { Triggers.PressedLeft.ToString(), (int)Triggers.PressedLeft },
                { Triggers.PressedDown.ToString(), (int)Triggers.PressedDown },
                { Triggers.PressedRight.ToString(), (int)Triggers.PressedRight }
            };

            fsm = new FSM(Enum.GetValues(typeof(States)).Length, triggers);

            fsm.AddState<StartMinigameState>((int)States.Start,
                constructionParameters: new object[] { GetCurrentBags, bagsVisuals, animator });
            
            fsm.AddState<LeftMinigameState>((int)States.Left, constructionParameters: new object[] { bagTargetPos[0] },
                enterParametersPointer: () => new object[] { visualPCPrompts[0], bagsVisuals, GetCurrentBags },
                updateParametersPointer: () => new object[]
                    { GetCurrentBags, bagsVisuals, playerInput, Time.deltaTime },
                exitParametersPointer: () => new object[] { visualPCPrompts[0] });
            
            fsm.AddState<DownMinigameState>((int)States.Down, constructionParameters: new object[] { bagTargetPos[1] },
                enterParametersPointer: () => new object[] { visualPCPrompts[1], bagsVisuals, GetCurrentBags },
                updateParametersPointer: () => new object[]
                    { GetCurrentBags, bagsVisuals, playerInput, Time.deltaTime },
                exitParametersPointer: () => new object[] { visualPCPrompts[1] });
            
            fsm.AddState<RightMinigameState>((int)States.Right, constructionParameters: new object[] { bagTargetPos[2] },
                enterParametersPointer: () => new object[] { visualPCPrompts[2], bagsVisuals, GetCurrentBags },
                updateParametersPointer: () => new object[]
                    { GetCurrentBags, bagsVisuals, playerInput, Time.deltaTime },
                exitParametersPointer: () => new object[] { visualPCPrompts[2] });
            
            fsm.AddState<EndMinigameState>((int)States.End, constructionParameters: new object[] {animator}, updateParametersPointer: () => new object[] {Time.deltaTime});
            
            fsm.RegisterTransition((int)States.Start, Triggers.ReadyForInput.ToString(),(int)States.Left);
            fsm.RegisterTransition((int)States.Left, Triggers.PressedLeft.ToString(),(int)States.Down);
            fsm.RegisterTransition((int)States.Down, Triggers.PressedDown.ToString(),(int)States.Right);
            fsm.RegisterTransition((int)States.Right, Triggers.PressedRight.ToString(),(int)States.End);
        }

        private void OnEndMinigame(in EndMinigameEvent callback)
        {
            if (playerID == callback.playerID)
            {
                minigameParent.SetActive(false);
            }
        }

        private void OnBagReachedEnd(in BagReachedEndEvent callback)
        {
            StartCoroutine(TrackBag(bagsVisuals[currentBagsRemaining]));
            currentBagsRemaining--;
            if (currentBagsRemaining < 0)
                fsm.ForceState((int)States.End);
            else
                fsm.ForceState((int)States.Left);
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
            minigameParent.SetActive(true);
            currentBagsRemaining = bagsNumber;
            fsm.ForceState((int)States.Left);
        }

        IEnumerator TrackBag(GameObject bag)
        {
            Vector3 startPos = bag.transform.position;
            float elapsedTime = 0.0f;

            while (elapsedTime > 3.0f)
            {
                elapsedTime += Time.deltaTime;
                bag.transform.position = Vector3.Lerp(startPos, bagEndPos.position, elapsedTime / 3.0f);
                yield return new WaitForEndOfFrame();
            }
        }
    }
}