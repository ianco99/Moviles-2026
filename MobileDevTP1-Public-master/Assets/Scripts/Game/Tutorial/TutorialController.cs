using System;
using System.Collections.Generic;
using Game.States;
using ianco99.ToolBox.Events;
using ianco99.ToolBox.Services;
using UnityEngine;
using UnityEngine.InputSystem;

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
		
		triggers.Add("Left", (int)Triggers.PressedLeft);

		fsm = new FSM(Enum.GetValues(typeof(States)).Length, triggers);
		
		fsm.AddState<TutorialStart>((int)States.Start, updateParametersPointer: () => new object[] { playerInput });
		
		
		SetUpTutorial();
	}

	
	
	private void SetUpTutorial()
	{
		animator.SetTrigger("Start");
	}
}
