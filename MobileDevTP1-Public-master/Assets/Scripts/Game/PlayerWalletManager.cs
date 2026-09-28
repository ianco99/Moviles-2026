using Game.Bank;
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
			EventBus.Subscribe<EnterMinigameRequestEvent>(OnEnterMinigame);
			EventBus.Subscribe<EndMinigameEvent>(OnEndMinigame);
		}

		private void OnEndMinigame(in EndMinigameEvent callback)
		{
			if (callback.playerID == playerId)
			{
				currentPlayerSlots = 0;
			}
		}

		private void OnEnterMinigame(in EnterMinigameRequestEvent callback)
		{
			if (callback.playerId == playerId)
			{
				if (currentPlayerSlots > 0)
				{
					EventBus.Raise<StartMinigameEvent>(playerId, currentPlayerSlots);
				}
			}
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