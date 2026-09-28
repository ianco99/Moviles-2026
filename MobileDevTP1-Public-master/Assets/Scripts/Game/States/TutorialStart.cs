using UnityEngine;

namespace Game.States
{
	public class TutorialStart : State
	{
		public override BehaviourActions GetOnEnterBehaviours(params object[] parameters)
		{
			BehaviourActions actions = new BehaviourActions();
			
			GameObject visualPrompt = parameters[1] as GameObject;
			
			actions.AddMainThreadableBehaviour(0, () =>
			{
				visualPrompt.SetActive(true);
			});
			
			return actions;
		}

		public override BehaviourActions GetTickBehaviours(params object[] parameters)
		{
			BehaviourActions actions = new BehaviourActions();

			SwipeInput input = parameters[0] as SwipeInput;

			actions.AddMainThreadableBehaviour(0, () =>
			{
				Vector2 readValue = input.ReadDirection();
				if(readValue.y > 0.5f)
				{
					OnTrigger?.Invoke(TutorialController.Triggers.PressedUp.ToString());
				}
			});

			return actions;
		}

		public override BehaviourActions GetOnExitBehaviours(params object[] parameters)
		{
			BehaviourActions actions = new BehaviourActions();
			
			GameObject visualPrompt = parameters[0] as GameObject;
			
			actions.AddMainThreadableBehaviour(0, () =>
			{
				visualPrompt.SetActive(false);
			});
			
			return actions;
		}
	}

	public class TutorialLeft : State
	{
		private Sprite tutorialPrompt;
		
		public override BehaviourActions GetOnEnterBehaviours(params object[] parameters)
		{
			BehaviourActions actions = new BehaviourActions();
			
			Animator animator = parameters[0] as Animator;
			GameObject visualPrompt = parameters[1] as GameObject;
			
			actions.AddMainThreadableBehaviour(0, () =>
			{
				animator.SetTrigger("Up");
				visualPrompt.SetActive(true);
			});
			
			return actions;
		}

		public override BehaviourActions GetTickBehaviours(params object[] parameters)
		{
			BehaviourActions actions = new BehaviourActions();

			SwipeInput input = parameters[0] as SwipeInput;

			actions.AddMainThreadableBehaviour(0, () =>
			{
				Vector2 readValue = input.ReadDirection();
				if(readValue.x < -0.5f)
				{
					OnTrigger?.Invoke(TutorialController.Triggers.PressedLeft.ToString());
				}
			});

			return actions;
		}

		public override BehaviourActions GetOnExitBehaviours(params object[] parameters)
		{
			BehaviourActions actions = new BehaviourActions();
			
			GameObject visualPrompt = parameters[0] as GameObject;
			
			actions.AddMainThreadableBehaviour(0, () =>
			{
				visualPrompt.SetActive(false);
			});
			
			return actions;
		}
	}

	public class TutorialRight : State
	{
	
		public override BehaviourActions GetOnEnterBehaviours(params object[] parameters)
		{
			BehaviourActions actions = new BehaviourActions();
			
			Animator animator = parameters[0] as Animator;
			GameObject visualPrompt = parameters[1] as GameObject;
			
			actions.AddMainThreadableBehaviour(0, () =>
			{
				animator.SetTrigger("Down");
				visualPrompt.SetActive(true);
			});

			return actions;
		}

		public override BehaviourActions GetTickBehaviours(params object[] parameters)
		{
			BehaviourActions actions = new BehaviourActions();

			SwipeInput input = parameters[0] as SwipeInput;

			actions.AddMainThreadableBehaviour(0, () =>
			{
				Vector2 readValue = input.ReadDirection();
				if(readValue.x > 0.5f)
				{
					OnTrigger?.Invoke(TutorialController.Triggers.PressedRight.ToString());
				}
			});

			return actions;
		}

		public override BehaviourActions GetOnExitBehaviours(params object[] parameters)
		{
			BehaviourActions actions = new BehaviourActions();
			
			GameObject visualPrompt = parameters[0] as GameObject;
			
			actions.AddMainThreadableBehaviour(0, () =>
			{
				visualPrompt.SetActive(false);
			});
			
			return actions;
		}
	}

	public class TutorialDown : State
	{
		public override BehaviourActions GetOnEnterBehaviours(params object[] parameters)
		{
			BehaviourActions actions = new BehaviourActions();
			
			Animator animator = parameters[0] as Animator;
			GameObject visualPrompt = parameters[1] as GameObject;
			
			actions.AddMainThreadableBehaviour(0, () =>
			{
				animator.SetTrigger("Left");
				visualPrompt.SetActive(true);
			});

			return actions;
		}

		public override BehaviourActions GetTickBehaviours(params object[] parameters)
		{
			BehaviourActions actions = new BehaviourActions();

			SwipeInput input = parameters[0] as SwipeInput;

			actions.AddMainThreadableBehaviour(0, () =>
			{
				Vector2 readValue = input.ReadDirection();
				if(readValue.y < -0.5f)
				{
					OnTrigger?.Invoke(TutorialController.Triggers.PressedDown.ToString());
				}
			});

			return actions;
		}

		public override BehaviourActions GetOnExitBehaviours(params object[] parameters)
		{
			BehaviourActions actions = new BehaviourActions();
			
			GameObject visualPrompt = parameters[0] as GameObject;
			
			actions.AddMainThreadableBehaviour(0, () =>
			{
				visualPrompt.SetActive(false);
			});
			
			return actions;
		}
	}
	
	public class TutorialWin : State
	{
		public override BehaviourActions GetOnEnterBehaviours(params object[] parameters)
		{
			BehaviourActions actions = new BehaviourActions();
			
			Animator animator = parameters[0] as Animator;
			GameObject visualPrompt = parameters[1] as GameObject;

			actions.AddMainThreadableBehaviour(0, () =>
			{
				animator.SetTrigger("Right");
				visualPrompt.SetActive(true);
			});

			return actions;
		}

		public override BehaviourActions GetTickBehaviours(params object[] parameters)
		{
			return new BehaviourActions();
		}

		public override BehaviourActions GetOnExitBehaviours(params object[] parameters)
		{
			return new BehaviourActions();
		}
	}
}