using ianco99.ToolBox.Events;

namespace MoneyBags
{
    public struct PickUpMoneyRejectedEvent : IEvent
    {
        public void Assign(params object[] parameters)
        {
            
        }

        public void Reset()
        {
            throw new System.NotImplementedException();
        }
    }
}