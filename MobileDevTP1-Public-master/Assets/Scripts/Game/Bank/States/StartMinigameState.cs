using System;
using ianco99.ToolBox.Events;
using ianco99.ToolBox.Services;
using UnityEngine;

namespace Game.Bank.States
{
    public class StartMinigameState : State
    {
        private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();
        private Func<int> getBagsNumber;

        private GameObject[] bagsVisualObjects;
        
        private int bagsNumber;
        private Animator truckAnimator;
        
        private float elapsedTime;

        public StartMinigameState(params object[] parameters)
        {
            getBagsNumber = parameters[0] as Func<int>;
            bagsVisualObjects = parameters[1] as GameObject[];
            truckAnimator = parameters[2] as Animator;
        }
        
        public override BehaviourActions GetOnEnterBehaviours(params object[] parameters)
        {
            BehaviourActions behaviourActions = new BehaviourActions();
            
            
            
            behaviourActions.AddMainThreadableBehaviour(0, () =>
            {
                elapsedTime = 0;
                
                foreach (var bag in bagsVisualObjects)
                {
                    bag.SetActive(false);
                }
                
                bagsNumber = getBagsNumber.Invoke();
                
                for (int i = 0; i < bagsNumber; i++)
                {
                    bagsVisualObjects[i].SetActive(true);
                }
                
                truckAnimator.SetTrigger("Start");
            });
            
            return behaviourActions;
        }

        public override BehaviourActions GetTickBehaviours(params object[] parameters)
        {
            BehaviourActions behaviourActions = new BehaviourActions();

            float deltaTime = (float)parameters[0];
            
            behaviourActions.AddMainThreadableBehaviour(0, () =>
            {
                elapsedTime += deltaTime;

                if (elapsedTime > 1.5f)
                {
                    OnTrigger?.Invoke(DepositMinigameController.Triggers.ReadyForInput.ToString());
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