using System;
using System.Collections.Generic;
using System.IO.Enumeration;
using Game.States;
using ianco99.ToolBox.Events;
using ianco99.ToolBox.Services;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;

public class TutorialController : MonoBehaviour
{
	enum States
	{
		Start,
		Left,
		Down,
		Right
	}

	public enum Triggers
	{
		PressedUp,
		PressedLeft,
		PressedDown,
		PressedRight
	}
	
	[SerializeField] private Animator animator;
	[SerializeField] private PlayerInput playerInput;
	
	EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();
	
	private int playerCount;

	private FSM fsm;
	
	private void Awake()
	{
		playerCount = PlayerPrefs.GetInt("PlayerCount", 0);

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
		fsm.AddState<TutorialWin>((int)States.Right+1, enterParametersPointer: () => new object[] {animator});
		
		fsm.RegisterTransition((int)States.Start, Triggers.PressedUp.ToString(),(int)States.Left);
		fsm.RegisterTransition((int)States.Left, Triggers.PressedLeft.ToString(),(int)States.Down);
		fsm.RegisterTransition((int)States.Down, Triggers.PressedDown.ToString(),(int)States.Right);
		fsm.RegisterTransition((int)States.Right, Triggers.PressedRight.ToString(),(int)States.Right+1);
		
		fsm.ForceState(0);
	}

	private void Update()
	{
		fsm.Update();
	}
}
