using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Bank.States
{
    public class RightMinigameState : State
    {
        private Vector3 targetPos;
        private Vector3 startPos;
        private float elapsed;
        public RightMinigameState(params object[] parameters)
        {
            targetPos = (Vector3)parameters[0];
        }
        
        public override BehaviourActions GetOnEnterBehaviours(params object[] parameters)
        {
            BehaviourActions actions = new BehaviourActions();
            
            GameObject visualPrompt = parameters[0] as GameObject;
            GameObject[] visualBag = parameters[1] as GameObject[];
            int index = (int)parameters[2];
            startPos = visualBag[index].transform.position;			
            actions.AddMainThreadableBehaviour(0, () =>
            {
                visualPrompt.SetActive(true);
                
                elapsed = 0;
            });


            return actions;
        }

        public override BehaviourActions GetTickBehaviours(params object[] parameters)
        {
            BehaviourActions actions = new BehaviourActions();

            int currentIndex = (int)parameters[0];
            GameObject[] visualBags = parameters[1] as GameObject[];
            PlayerInput input = parameters[2] as PlayerInput;
            float deltaTime = (float)parameters[3];
            
            actions.AddMainThreadableBehaviour(0, () =>
            {
                Vector2 readValue = input.actions["Controls"].ReadValue<Vector2>();
                if(readValue.y < -0.5f)
                {
                    OnTrigger?.Invoke(DepositMinigameController.Triggers.PressedRight.ToString());
                }
            });
            
            actions.AddMainThreadableBehaviour(1, () =>
            {
                elapsed += deltaTime;
            });
            
            actions.AddMainThreadableBehaviour(2, () =>
            {
                visualBags[currentIndex].transform.position = Vector3.Lerp(startPos,targetPos, elapsed/0.5f);
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
}