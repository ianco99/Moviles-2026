using Game.Bank;
using ianco99.ToolBox.Events;
using ianco99.ToolBox.Services;
using MoneyBags;
using UnityEngine;

namespace Game.Audio
{
	public class GameplaySfx : MonoBehaviour
	{
		private const float MaxBagBonus = 100000f;

		[Header("Money bags")] [SerializeField] private AudioClip moneyPickup;
		[SerializeField] private float moneyPickupVolume = 1f;
		[SerializeField] private float moneyPickupPitchVariation = 0.08f;

		[Header("Deposit minigame")] [SerializeField] private AudioClip cashIn;
		[SerializeField] private float cashInVolume = 1f;
		[SerializeField] private float cashInMinPitch = 0.85f;
		[SerializeField] private float cashInMaxPitch = 1.2f;
		[SerializeField] private AudioClip minigameEnd;
		[SerializeField] private float minigameEndVolume = 1f;

		private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();

		private void Start()
		{
			EventBus.Subscribe<PickUpMoneyAcceptedEvent>(OnPickUpMoney);
			EventBus.Subscribe<BagBonusCollectedEvent>(OnBagBonusCollected);
			EventBus.Subscribe<EndMinigameEvent>(OnEndMinigame);
		}

		private void OnDestroy()
		{
			EventBus.UnSubscribe<PickUpMoneyAcceptedEvent>(OnPickUpMoney);
			EventBus.UnSubscribe<BagBonusCollectedEvent>(OnBagBonusCollected);
			EventBus.UnSubscribe<EndMinigameEvent>(OnEndMinigame);
		}

		private void OnPickUpMoney(in PickUpMoneyAcceptedEvent callback)
		{
			// Slight random pitch so picking several bags in a row doesn't sound repetitive
			float pitch = 1f + Random.Range(-moneyPickupPitchVariation, moneyPickupPitchVariation);
			Play(moneyPickup, moneyPickupVolume, pitch);
		}

		private void OnBagBonusCollected(in BagBonusCollectedEvent callback)
		{
			// Faster deposits keep more bonus and sound higher
			float t = Mathf.Clamp01(callback.bonusAmount / MaxBagBonus);
			Play(cashIn, cashInVolume, Mathf.Lerp(cashInMinPitch, cashInMaxPitch, t));
		}

		private void OnEndMinigame(in EndMinigameEvent callback)
		{
			Play(minigameEnd, minigameEndVolume);
		}

		private static void Play(AudioClip clip, float volume, float pitch = 1f)
		{
			if (AudioManager.Instance == null) return;

			AudioManager.Instance.PlaySfx(clip, volume, pitch);
		}
	}
}
