using ianco99.ToolBox.Events;

namespace Game.Bank
{
    public struct EnterMinigameAcceptEvent : IEvent
    {
        public int playerID;
        public void Assign(params object[] parameters)
        {
            playerID = (int)parameters[0];
        }

        public void Reset()
        {
            playerID = 0;
        }
    }
}