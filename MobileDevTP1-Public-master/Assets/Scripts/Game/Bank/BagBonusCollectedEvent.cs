using ianco99.ToolBox.Events;

namespace Game.Bank
{
    public struct BagBonusCollectedEvent : IEvent
    {
        public int playerID;
        public int bonusAmount;

        public void Assign(params object[] parameters)
        {
            playerID = (int)parameters[0];
            bonusAmount = (int)parameters[1];
        }

        public void Reset()
        {
            playerID = default;
            bonusAmount = default;
        }
    }
}
