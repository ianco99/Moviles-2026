using ianco99.ToolBox.Events;

namespace Game.Bank
{
    public struct StartMinigameEvent : IEvent
    {
        public int playerID;
        public int bagsNumber;
        public void Assign(params object[] parameters)
        {
            playerID = (int)parameters[0];
            bagsNumber = (int)parameters[1];
        }

        public void Reset()
        {
            playerID = 0;
            bagsNumber = 0;
        }
    }
}