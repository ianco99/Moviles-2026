using System;
using ianco99.ToolBox.Events;
using ianco99.ToolBox.Services;
using MoneyBags;
using Unity.Android.Gradle.Manifest;
using UnityEngine;

namespace Game
{
	public class PlayerWalletManager : MonoBehaviour
	{
		private int playerId;

		private int playerMoney;
		EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();
		
		private void Awake()
		{
			EventBus.Subscribe<PickUpMoneyEvent>(OnPickUpMoney);
		}

		private void OnPickUpMoney(in PickUpMoneyEvent pickUpMoneyEvent)
		{
			
		}

		public void SetUp(int playerId)
		{
			this.playerId = playerId;
		}
	}
}