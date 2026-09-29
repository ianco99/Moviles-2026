using System;
using Game.Bank;
using ianco99.ToolBox.Events;
using ianco99.ToolBox.Services;
using MoneyBags;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class PlayerWalletManager : MonoBehaviour
    {
        [SerializeField] private Sprite[] inventoryImages;
        [SerializeField] private Image[] inventory;
        [SerializeField] private TMPro.TMP_Text[] moneyText;
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
            EventBus.Subscribe<BagBonusCollectedEvent>(OnBagBonusCollected);
        }

        private void OnDestroy()
        {
            EventBus.UnSubscribe<PickUpMoneyRequestEvent>(OnPickUpMoney);
            EventBus.UnSubscribe<EnterMinigameRequestEvent>(OnEnterMinigame);
            EventBus.UnSubscribe<EndMinigameEvent>(OnEndMinigame);
            EventBus.UnSubscribe<BagBonusCollectedEvent>(OnBagBonusCollected);
        }

        private void Update()
        {
            moneyText[0].text = "$" + (int)playerMoney;
            moneyText[1].text = "$" + (int)playerMoney;
        }

        private void OnBagBonusCollected(in BagBonusCollectedEvent callback)
        {
            if (callback.playerID == playerId)
            {
                playerMoney += callback.bonusAmount;
            }
        }

        private void OnEndMinigame(in EndMinigameEvent callback)
        {
            if (callback.playerID == playerId)
            {
                currentPlayerSlots = 0;
                inventory[0].sprite = inventoryImages[0];
                inventory[1].sprite = inventoryImages[0];
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
                    inventory[0].sprite = inventoryImages[currentPlayerSlots];
                    inventory[1].sprite = inventoryImages[currentPlayerSlots];
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