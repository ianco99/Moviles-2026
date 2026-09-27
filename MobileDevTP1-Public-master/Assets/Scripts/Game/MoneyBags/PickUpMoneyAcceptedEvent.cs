using ianco99.ToolBox.Events;

namespace MoneyBags
{
    public struct PickUpMoneyAcceptedEvent : IEvent
    {
        public uint id;
        public void Assign(params object[] parameters)
        {
            id = (uint)parameters[0];
        }

        public void Reset()
        {
            id = default;
        }
    }
}