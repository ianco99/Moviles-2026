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

	EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();

	private FSM fsm;

	private void Awake()
	{
		Dictionary<string, int> triggers = new Dictionary<string, int>()
		{
			{Triggers.PressedUp.ToString(), (int)Triggers.PressedUp},
			{Triggers.PressedLeft.ToString(), (int)Triggers.PressedLeft},
			{Triggers.PressedDown.ToString(), (int)Triggers.PressedDown},
			{Triggers.PressedRight.ToString(), (int)Triggers.PressedRight}
		};

		fsm = new FSM(Enum.GetValues(typeof(States)).Length, triggers);

		fsm.AddState<TutorialStart>((int)States.Start, enterParametersPointer: () => new object[] {animator}, updateParametersPointer: () => new object[] { playerInput });
		fsm.AddState<TutorialLeft>((int)States.Left, enterParametersPointer: () => new object[] {animator}, updateParametersPointer: () => new object[] { playerInput });
		fsm.AddState<TutorialDown>((int)States.Down, enterParametersPointer: () => new object[] {animator}, updateParametersPointer: () => new object[] { playerInput });
		fsm.AddState<TutorialRight>((int)States.Right, enterParametersPointer: () => new object[] {animator}, updateParametersPointer: () => new object[] { playerInput });
		fsm.AddState<TutorialWin>((int)States.Win, enterParametersPointer: () => new object[] {animator});

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
