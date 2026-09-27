using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.States
{
	public class TutorialStart : State
	{
		public override BehaviourActions GetOnEnterBehaviours(params object[] parameters)
		{
			return new BehaviourActions();
		}

		public override BehaviourActions GetTickBehaviours(params object[] parameters)
		{
			BehaviourActions actions = new BehaviourActions();

			PlayerInput input = parameters[0] as PlayerInput;

			actions.AddMainThreadableBehaviour(0, () =>
			{
				Vector2 readValue = input.actions["Controls"].ReadValue<Vector2>();
				if(readValue.y > 0.5f)
				{
					OnTrigger?.Invoke(TutorialController.Triggers.PressedUp.ToString());
				}
			});

			return actions;
		}

		public override BehaviourActions GetOnExitBehaviours(params object[] parameters)
		{
			return new BehaviourActions();
		}
	}

	public class TutorialLeft : State
	{
		private Sprite tutorialPrompt;
		
		public override BehaviourActions GetOnEnterBehaviours(params object[] parameters)
		{
			BehaviourActions actions = new BehaviourActions();
			
			Animator animator = parameters[0] as Animator;
			
			actions.AddMainThreadableBehaviour(0, () =>
			{
				animator.SetTrigger("Up");
			});
			
			return actions;
		}

		public override BehaviourActions GetTickBehaviours(params object[] parameters)
		{
			BehaviourActions actions = new BehaviourActions();

			PlayerInput input = parameters[0] as PlayerInput;

			actions.AddMainThreadableBehaviour(0, () =>
			{
				Vector2 readValue = input.actions["Controls"].ReadValue<Vector2>();
				if(readValue.x < -0.5f)
				{
					OnTrigger?.Invoke(TutorialController.Triggers.PressedLeft.ToString());
				}
			});

			return actions;
		}

		public override BehaviourActions GetOnExitBehaviours(params object[] parameters)
		{
			return new BehaviourActions();
		}
	}

	public class TutorialRight : State
	{
	
		public override BehaviourActions GetOnEnterBehaviours(params object[] parameters)
		{
			BehaviourActions actions = new BehaviourActions();
			
			Animator animator = parameters[0] as Animator;
			
			actions.AddMainThreadableBehaviour(0, () =>
			{
				animator.SetTrigger("Down");
			});

			return actions;
		}

		public override BehaviourActions GetTickBehaviours(params object[] parameters)
		{
			BehaviourActions actions = new BehaviourActions();

			PlayerInput input = parameters[0] as PlayerInput;

			actions.AddMainThreadableBehaviour(0, () =>
			{
				Vector2 readValue = input.actions["Controls"].ReadValue<Vector2>();
				if(readValue.x > 0.5f)
				{
					OnTrigger?.Invoke(TutorialController.Triggers.PressedRight.ToString());
				}
			});

			return actions;
		}

		public override BehaviourActions GetOnExitBehaviours(params object[] parameters)
		{
			return new BehaviourActions();
		}
	}

	public class TutorialDown : State
	{
		public override BehaviourActions GetOnEnterBehaviours(params object[] parameters)
		{
			BehaviourActions actions = new BehaviourActions();
			
			Animator animator = parameters[0] as Animator;
			
			actions.AddMainThreadableBehaviour(0, () =>
			{
				animator.SetTrigger("Left");
			});

			return actions;
		}

		public override BehaviourActions GetTickBehaviours(params object[] parameters)
		{
			BehaviourActions actions = new BehaviourActions();

			PlayerInput input = parameters[0] as PlayerInput;

			actions.AddMainThreadableBehaviour(0, () =>
			{
				Vector2 readValue = input.actions["Controls"].ReadValue<Vector2>();
				if(readValue.y < -0.5f)
				{
					OnTrigger?.Invoke(TutorialController.Triggers.PressedDown.ToString());
				}
			});

			return actions;
		}

		public override BehaviourActions GetOnExitBehaviours(params object[] parameters)
		{
			return new BehaviourActions();
		}
	}
	
	public class TutorialWin : State
	{
		public override BehaviourActions GetOnEnterBehaviours(params object[] parameters)
		{
			BehaviourActions actions = new BehaviourActions();
			
			Animator animator = parameters[0] as Animator;
			
			actions.AddMainThreadableBehaviour(0, () =>
			{
				animator.SetTrigger("Right");
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