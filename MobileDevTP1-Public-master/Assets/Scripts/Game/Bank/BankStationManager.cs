using System.Collections.Generic;
using UnityEngine;

namespace Game.Bank
{
	// Disables random deposit zones so harder difficulties have fewer places to deposit
	public class BankStationManager : MonoBehaviour
	{
		private const int MinPlayerCount = 1;

		private const int BanksToDisableEasy = 0;
		private const int BanksToDisableMedium = 3;
		private const int BanksToDisableHard = 5;

		private const int BanksRestoredPerExtraPlayer = 1;

		// At least one bank always stays usable so the game can be played
		private const int MinEnabledBanks = 1;

		private void Awake()
		{
			List<DepositTrigger> deposits = new List<DepositTrigger>(
				FindObjectsByType<DepositTrigger>(FindObjectsInactive.Exclude, FindObjectsSortMode.None));

			int banksToDisable = Mathf.Min(GetBanksToDisableCount(), deposits.Count - MinEnabledBanks);
			for (int i = 0; i < banksToDisable; i++)
			{
				int randomIndex = Random.Range(0, deposits.Count);
				deposits[randomIndex].Disable();
				deposits.RemoveAt(randomIndex);
			}
		}

		private static int GetBanksToDisableCount()
		{
			int difficulty = PlayerPrefs.GetInt("Difficulty", 0);
			int playerCount = PlayerPrefs.GetInt("PlayerCount", MinPlayerCount);

			int banksToDisable = difficulty switch
			{
				0 => BanksToDisableEasy,
				1 => BanksToDisableMedium,
				2 => BanksToDisableHard,
				_ => BanksToDisableEasy
			};

			int extraPlayers = Mathf.Max(0, playerCount - MinPlayerCount);
			banksToDisable -= extraPlayers * BanksRestoredPerExtraPlayer;

			return Mathf.Max(0, banksToDisable);
		}
	}
}
