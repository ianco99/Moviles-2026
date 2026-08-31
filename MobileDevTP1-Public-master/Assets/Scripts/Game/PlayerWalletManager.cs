using ianco99.ToolBox.Events;
using ianco99.ToolBox.Services;
using MoneyBags;
using UnityEngine;

namespace Game
{
	public class PlayerWalletManager : MonoBehaviour
	{
		private int playerId = 0;

		public float playerMoney;
		EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();
		
		private void Start()
		{
			EventBus.Subscribe<PickUpMoneyEvent>(OnPickUpMoney);
		}

		private void OnPickUpMoney(in PickUpMoneyEvent pickUpMoneyEvent)
		{
			if (pickUpMoneyEvent.PlayerId == playerId)
				playerMoney += pickUpMoneyEvent.MoneyAmount;
		}

		public void SetUp(int playerId)
		{
			this.playerId = playerId;
		}
	}
}