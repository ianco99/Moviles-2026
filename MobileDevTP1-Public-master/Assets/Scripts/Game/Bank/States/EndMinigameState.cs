using ianco99.ToolBox.Events;
using ianco99.ToolBox.Services;
using UnityEngine;

namespace Game.Bank.States
{
    public class EndMinigameState : State
    {
        private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();
        
        private Animator truckAnimator;

        private float elapsed;
        
        public EndMinigameState(params object[] parameters)
        {
            truckAnimator = parameters[0] as Animator;
        }
        
        public override BehaviourActions GetOnEnterBehaviours(params object[] parameters)
        {
            BehaviourActions behaviourActions = new BehaviourActions();
            
            
            behaviourActions.AddMainThreadableBehaviour(0, () =>
            {
                elapsed = 0;
                truckAnimator.SetTrigger("End");
            });
            
            return behaviourActions;
        }

        public override BehaviourActions GetTickBehaviours(params object[] parameters)
        {
            BehaviourActions behaviourActions = new BehaviourActions();
            
            float deltaTime = (float)parameters[0]; 
            
            behaviourActions.AddMainThreadableBehaviour(0, () => 
            {
                elapsed += deltaTime;

                if (elapsed >= 1.5f)
                {
                    EventBus.Raise<EndMinigameEvent>();
                }
            });
            
            return behaviourActions;
        }

        public override BehaviourActions GetOnExitBehaviours(params object[] parameters)
        {
            return new BehaviourActions();
        }
    }
}