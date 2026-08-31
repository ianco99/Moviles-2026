using UnityEngine.InputSystem;

namespace Game.States
{
	public class TutorialStart : State
	{
		public override BehaviourActions GetOnEnterBehaviours(params object[] parameters)
		{
			throw new System.NotImplementedException();
		}

		public override BehaviourActions GetTickBehaviours(params object[] parameters)
		{
			BehaviourActions actions = new BehaviourActions();
			
			PlayerInput input = parameters[0] as PlayerInput;
			
			actions.AddMainThreadableBehaviour(0,() => 
			{
				if (input.actions["Up"].IsPressed())
				{
					OnTrigger?.Invoke(TutorialController.Triggers.PressedUp.ToString());
				}
			});
			
			return actions;
		}

		public override BehaviourActions GetOnExitBehaviours(params object[] parameters)
		{
			throw new System.NotImplementedException();
		}
	}
}