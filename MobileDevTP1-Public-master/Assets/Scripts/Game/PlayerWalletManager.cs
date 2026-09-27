using ianco99.ToolBox.Events;
using ianco99.ToolBox.Services;
using MoneyBags;
using UnityEngine;

namespace Game
{
	public class PlayerWalletManager : MonoBehaviour
	{
		[SerializeField] private int playerId = 0;

		public const int MAX_PLAYER_SLOTS = 3;

		private int currentPlayerSlots = 0;
		
		public float playerMoney;
		EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();
		
		private void Start()
		{
			EventBus.Subscribe<PickUpMoneyRequestEvent>(OnPickUpMoney);
		}

		private void OnPickUpMoney(in PickUpMoneyRequestEvent pickUpMoneyRequestEvent)
		{
			if (pickUpMoneyRequestEvent.PlayerId == playerId)
			{
				if (currentPlayerSlots < MAX_PLAYER_SLOTS)
				{
					currentPlayerSlots++;
					playerMoney += pickUpMoneyRequestEvent.MoneyAmount;
					EventBus.Raise<PickUpMoneyAcceptedEvent>(pickUpMoneyRequestEvent.bagId);
				}
			}
		}

		public void SetUp(int playerId)
		{
			this.playerId = playerId;
		}
	}
}