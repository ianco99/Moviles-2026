using ianco99.ToolBox.Events;

namespace MoneyBags
{
	public struct PickUpMoneyRequestEvent : IEvent
	{
		public int PlayerId;
		public float MoneyAmount;
		public uint bagId;
		public void Assign(params object[] parameters)
		{
			PlayerId = (int)parameters[0];
			MoneyAmount = (float)parameters[1];
			bagId = (uint)parameters[2];
		}

		public void Reset()
		{
			PlayerId = 0;
			MoneyAmount = 0;
			bagId = 0;
		}
	}
}