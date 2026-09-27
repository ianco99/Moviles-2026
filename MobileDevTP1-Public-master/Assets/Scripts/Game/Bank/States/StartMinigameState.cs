using System;
using UnityEngine;

namespace Game.Bank.States
{
    public class StartMinigameState : State
    {
        private Func<int> getBagsNumber;

        private GameObject[] bagsVisualObjects;
        
        private int bagsNumber;
        private Animator truckAnimator;

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
                foreach (var bag in bagsVisualObjects)
                {
                    bag.SetActive(false);
                }
                
                bagsNumber = getBagsNumber.Invoke();
                
                for (int i = 0; i < bagsNumber; i++)
                {
                    bagsVisualObjects[i].SetActive(true);
                }
            });
            
            return behaviourActions;
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