using ianco99.ToolBox.Events;

namespace Game.Bank
{
    public struct EnterMinigameRequestEvent : IEvent
    {
        public int playerId;
        public void Assign(params object[] parameters)
        {
            playerId = (int)parameters[0];
        }

        public void Reset()
        {
            playerId = 0;
        }
    }
}