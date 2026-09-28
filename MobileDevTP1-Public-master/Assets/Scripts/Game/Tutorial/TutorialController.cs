using System;
using System.Collections;
using System.Collections.Generic;
using Game.Events;
using Game.States;
using ianco99.ToolBox.Events;
using ianco99.ToolBox.Services;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialController : MonoBehaviour
{
	private enum States
	{
		Start,
		Left,
		Down,
		Right,
		Win
	}

	public enum Triggers
	{
		PressedUp,
		PressedLeft,
		PressedDown,
		PressedRight
	}

	private const float WinDisplayDuration = 1.0f;

	[SerializeField] private int playerID;
	[SerializeField] private Animator animator;
	[SerializeField] private PlayerInput playerInput;
	[SerializeField] private GameObject tutorialRoot;

	[Header("Prompts (Up, Left, Down, Right)")]
	[SerializeField] private GameObject[] visualWASDPrompts;
	[SerializeField] private GameObject[] visualArrowPrompts;
	[SerializeField] private GameObject[] visualSwipePrompts;
	[SerializeField] private GameObject visualGetReadyPrompt;

	EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();

	private FSM fsm;
	private GameObject[] visualPrompts;

	private void Awake()
	{
		HidePrompts(visualWASDPrompts);
		HidePrompts(visualArrowPrompts);
		HidePrompts(visualSwipePrompts);
		visualGetReadyPrompt.SetActive(false);

#if PC_BUILD
		visualPrompts = playerID == 0 ? visualWASDPrompts : visualArrowPrompts;
#endif

#if ANDROID_BUILD
		visualPrompts = visualSwipePrompts;
#endif


		Dictionary<string, int> triggers = new Dictionary<string, int>()
		{
			{Triggers.PressedUp.ToString(), (int)Triggers.PressedUp},
			{Triggers.PressedLeft.ToString(), (int)Triggers.PressedLeft},
			{Triggers.PressedDown.ToString(), (int)Triggers.PressedDown},
			{Triggers.PressedRight.ToString(), (int)Triggers.PressedRight}
		};

		fsm = new FSM(Enum.GetValues(typeof(States)).Length, triggers);

		fsm.AddState<TutorialStart>((int)States.Start, enterParametersPointer: () => new object[] {animator, visualPrompts[0]}, updateParametersPointer: () => new object[] { playerInput }, exitParametersPointer: () => new object[] { visualPrompts[0] });
		fsm.AddState<TutorialLeft>((int)States.Left, enterParametersPointer: () => new object[] {animator, visualPrompts[1]}, updateParametersPointer: () => new object[] { playerInput }, exitParametersPointer: () => new object[] { visualPrompts[1] });
		fsm.AddState<TutorialDown>((int)States.Down, enterParametersPointer: () => new object[] {animator, visualPrompts[2]}, updateParametersPointer: () => new object[] { playerInput }, exitParametersPointer: () => new object[] { visualPrompts[2] });
		fsm.AddState<TutorialRight>((int)States.Right, enterParametersPointer: () => new object[] {animator, visualPrompts[3]}, updateParametersPointer: () => new object[] { playerInput }, exitParametersPointer: () => new object[] { visualPrompts[3] });
		fsm.AddState<TutorialWin>((int)States.Win, enterParametersPointer: () => new object[] {animator, visualGetReadyPrompt});

		fsm.RegisterTransition((int)States.Start, Triggers.PressedUp.ToString(),(int)States.Left);
		fsm.RegisterTransition((int)States.Left, Triggers.PressedLeft.ToString(),(int)States.Down);
		fsm.RegisterTransition((int)States.Down, Triggers.PressedDown.ToString(),(int)States.Right);
		fsm.RegisterTransition((int)States.Right, Triggers.PressedRight.ToString(),(int)States.Win, OnTutorialWon);

		fsm.ForceState((int)States.Start);
	}

	private void Start()
	{
		EventBus.Subscribe<GameStartedEvent>(OnGameStarted);
	}

	private void OnDestroy()
	{
		EventBus.UnSubscribe<GameStartedEvent>(OnGameStarted);
	}

	private void Update()
	{
		fsm.Update();
	}

	private void HidePrompts(GameObject[] prompts)
	{
		foreach (GameObject prompt in prompts)
			prompt.SetActive(false);
	}

	private void OnTutorialWon()
	{
		StartCoroutine(RaiseCompletedAfterWin());
	}

	// Let the win animation play before reporting completion
	private IEnumerator RaiseCompletedAfterWin()
	{
		yield return new WaitForSeconds(WinDisplayDuration);
		EventBus.Raise<TutorialCompletedEvent>(playerID);
	}

	private void OnGameStarted(in GameStartedEvent callback)
	{
		playerInput.enabled = false;
		tutorialRoot.SetActive(false);
		enabled = false;
	}
}
