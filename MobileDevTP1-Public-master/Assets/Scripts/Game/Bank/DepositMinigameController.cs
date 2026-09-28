using System;
using System.Collections;
using System.Collections.Generic;
using Game.Bank.States;
using Game.States;
using ianco99.ToolBox.Events;
using ianco99.ToolBox.Services;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Game.Bank
{
    public class DepositMinigameController : MonoBehaviour
    {
        [SerializeField] private int playerID;

        [SerializeField] private GameObject[] bagsVisuals;
        [SerializeField] private Transform[] bagTargetPos;
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private SwipeInput swipeInput;
        [SerializeField] private Animator animator;
        [SerializeField] private Transform bagEndPos;
        [SerializeField] private GameObject minigameParent;
        [SerializeField] private GameObject[] visualPCPrompts;
        [SerializeField] private GameObject[] visualPhonePrompts;
        [SerializeField] private Image fillUI;
        [SerializeField] private GameObject fillParent;
        EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();

        private int GetCurrentBags => currentBagsRemaining;

        private int GetCurrentBagIndex => currentBagsRemaining - 1;

        private Func<int> dad => Coso;

        private int Coso()
        {
            return GetCurrentBags;
        }

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


        private const int BagBonusStart = 100000;
        private const float BagBonusDiminishDuration = 3.0f;

        private int currentBagsRemaining;

        private int currentBagBonus = BagBonusStart;
        private float bagBonusElapsed;
        private bool isDiminishingBagBonus;

        private Transform[] bagInitialParents;
        private Vector3[] bagInitialLocalPositions;
        private Quaternion[] bagInitialLocalRotations;
        private readonly List<Coroutine> trackBagCoroutines = new List<Coroutine>();

        private FSM fsm;
        private GameObject[] visualPrompts;

        private void Start()
        {
            HidePrompts(visualPCPrompts);
            HidePrompts(visualPhonePrompts);

#if PC_BUILD
            visualPrompts = visualPCPrompts;
#endif
#if ANDROID_BUILD
            visualPrompts = visualPhonePrompts;
#endif

            bagInitialParents = new Transform[bagsVisuals.Length];
            bagInitialLocalPositions = new Vector3[bagsVisuals.Length];
            bagInitialLocalRotations = new Quaternion[bagsVisuals.Length];
            for (int i = 0; i < bagsVisuals.Length; i++)
            {
                bagInitialParents[i] = bagsVisuals[i].transform.parent;
                bagInitialLocalPositions[i] = bagsVisuals[i].transform.localPosition;
                bagInitialLocalRotations[i] = bagsVisuals[i].transform.localRotation;
            }

            EventBus.Subscribe<StartMinigameEvent>(OnStartMinigame);
            EventBus.Subscribe<BagReachedEndEvent>(OnBagReachedEnd);
            EventBus.Subscribe<EndMinigameEvent>(OnEndMinigame);


            Dictionary<string, int> triggers = new Dictionary<string, int>()
            {
                { Triggers.ReadyForInput.ToString(), (int)Triggers.ReadyForInput },
                { Triggers.PressedLeft.ToString(), (int)Triggers.PressedLeft },
                { Triggers.PressedDown.ToString(), (int)Triggers.PressedDown },
                { Triggers.PressedRight.ToString(), (int)Triggers.PressedRight }
            };

            fsm = new FSM(Enum.GetValues(typeof(States)).Length, triggers);

            fsm.AddState<StartMinigameState>((int)States.Start,
                constructionParameters: new object[] { dad, bagsVisuals, animator }, updateParametersPointer:
                () => new object[] { Time.deltaTime });

            fsm.AddState<LeftMinigameState>((int)States.Left,
                constructionParameters: new object[] { bagTargetPos[0].position },
                enterParametersPointer: () => new object[] { visualPrompts[1], bagsVisuals, GetCurrentBagIndex },
                updateParametersPointer: () => new object[]
                    { GetCurrentBagIndex, bagsVisuals, swipeInput, Time.deltaTime },
                exitParametersPointer: () => new object[] { visualPrompts[1] });

            fsm.AddState<DownMinigameState>((int)States.Down,
                constructionParameters: new object[] { bagTargetPos[1].position },
                enterParametersPointer: () => new object[] { visualPrompts[2], bagsVisuals, GetCurrentBagIndex },
                updateParametersPointer: () => new object[]
                    { GetCurrentBagIndex, bagsVisuals, swipeInput, Time.deltaTime },
                exitParametersPointer: () => new object[] { visualPrompts[2] });

            fsm.AddState<RightMinigameState>((int)States.Right,
                constructionParameters: new object[] { bagTargetPos[2].position },
                enterParametersPointer: () => new object[] { visualPrompts[3], bagsVisuals, GetCurrentBagIndex },
                updateParametersPointer: () => new object[]
                    { GetCurrentBagIndex, bagsVisuals, swipeInput, Time.deltaTime },
                exitParametersPointer: () => new object[] { visualPrompts[3] });

            fsm.AddState<EndMinigameState>((int)States.End, constructionParameters: new object[] { animator, playerID },
                updateParametersPointer: () => new object[] { Time.deltaTime });

            fsm.RegisterTransition((int)States.Start, Triggers.ReadyForInput.ToString(), (int)States.Left, UnparentCurrentBag);
            fsm.RegisterTransition((int)States.Left, Triggers.PressedLeft.ToString(), (int)States.Down, StartBagBonusDiminish);
            fsm.RegisterTransition((int)States.Down, Triggers.PressedDown.ToString(), (int)States.Right);
        }

        private void Update()
        {
            fsm.Update();
            UpdateBagBonus();
        }

        private void HidePrompts(GameObject[] prompts)
        {
            foreach (GameObject prompt in prompts)
                prompt.SetActive(false);
        }

        private void UpdateBagBonus()
        {
            if (!isDiminishingBagBonus)
                return;

            bagBonusElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(bagBonusElapsed / BagBonusDiminishDuration);
            currentBagBonus = (int)Mathf.Lerp(BagBonusStart, 0, t);

            fillUI.fillAmount = 1f - t;
            
            if (t >= 1.0f)
                isDiminishingBagBonus = false;
        }

        private void StartBagBonusDiminish()
        {
            bagBonusElapsed = 0f;
            isDiminishingBagBonus = true;
            fillParent.SetActive(true);
        }

        private void ResetBagBonus()
        {
            currentBagBonus = BagBonusStart;
            bagBonusElapsed = 0f;
            isDiminishingBagBonus = false;
            fillParent.SetActive(false);
        }

        private void UnparentCurrentBag()
        {
            bagsVisuals[GetCurrentBagIndex].transform.SetParent(null, true);
        }

        private void ResetAndReparentBags()
        {
            // TrackBag outlives the End state, so stop it or it keeps dragging bags after the reset
            foreach (Coroutine coroutine in trackBagCoroutines)
            {
                if (coroutine != null)
                    StopCoroutine(coroutine);
            }
            trackBagCoroutines.Clear();

            for (int i = 0; i < bagsVisuals.Length; i++)
            {
                Transform bagTransform = bagsVisuals[i].transform;
                bagTransform.SetParent(bagInitialParents[i], false);
                bagTransform.localPosition = bagInitialLocalPositions[i];
                bagTransform.localRotation = bagInitialLocalRotations[i];
            }
        }

        private void OnEndMinigame(in EndMinigameEvent callback)
        {
            if (playerID == callback.playerID)
            {
                minigameParent.SetActive(false);
                ResetAndReparentBags();

                if (playerID == 0)
                    playerInput.SwitchCurrentActionMap("Truck");
                else
                    playerInput.SwitchCurrentActionMap("Truck2P");
            }
        }

        private void OnBagReachedEnd(in BagReachedEndEvent callback)
        {
            trackBagCoroutines.Add(StartCoroutine(TrackBag(bagsVisuals[GetCurrentBagIndex])));
            EventBus.Raise<BagBonusCollectedEvent>(playerID, currentBagBonus);
            currentBagsRemaining--;
            if (currentBagsRemaining <= 0)
                fsm.ForceState((int)States.End);
            else
            {
                ResetBagBonus();
                UnparentCurrentBag();
                fsm.ForceState((int)States.Left);
            }
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
            if (playerID == 0)
                playerInput.SwitchCurrentActionMap("Download");
            else
                playerInput.SwitchCurrentActionMap("Download2P");

            minigameParent.SetActive(true);
            ResetAndReparentBags();
            currentBagsRemaining = bagsNumber;
            ResetBagBonus();
            fsm.ForceState((int)States.Start);
        }

        IEnumerator TrackBag(GameObject bag)
        {
            Vector3 startPos = bag.transform.position;
            float elapsedTime = 0.0f;

            while (elapsedTime < 3.0f)
            {
                elapsedTime += Time.deltaTime;
                bag.transform.position = Vector3.Lerp(startPos, bagEndPos.position, elapsedTime / 3.0f);
                yield return new WaitForEndOfFrame();
            }
        }
    }
}