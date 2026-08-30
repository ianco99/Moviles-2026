using ianco99.ToolBox.Events;

namespace MoneyBags
{
	public struct PickUpMoneyEvent : IEvent
	{
		public int PlayerId;
		public int MoneyAmount;
		public void Assign(params object[] parameters)
		{
			PlayerId = (int)parameters[0];
			MoneyAmount = (int)parameters[1];
		}

		public void Reset()
		{
			PlayerId = 0;
			MoneyAmount = 0;
		}
	}
}