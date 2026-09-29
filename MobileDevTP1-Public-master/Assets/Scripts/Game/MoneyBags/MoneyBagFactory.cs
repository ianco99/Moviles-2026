using System.Collections.Generic;
using UnityEngine;

namespace MoneyBags
{
	public class MoneyBagFactory : MonoBehaviour
	{
		private const int MinPlayerCount = 1;

		private const int BagsToRemoveEasy = 0;
		private const int BagsToRemoveMedium = 6;
		private const int BagsToRemoveHard = 10;

		private const int BagsRestoredPerExtraPlayer = 2;

		// Path inside Assets/Resources, the prefab is loaded at runtime instead of being referenced by the scene
		private const string MoneyBagPrefabPath = "MoneyBag";

		[SerializeField] private MoneyBagSO[] _moneyBagVariants;
		[SerializeField] private Transform[] spawnLocations;

		private Dictionary<string, MoneyBagSO> _moneyBags = new Dictionary<string, MoneyBagSO>();
		private MoneyBag moneyBagPrefab;

		private uint moneyBagId = 0;

		private void Awake()
		{
			moneyBagPrefab = Resources.Load<MoneyBag>(MoneyBagPrefabPath);
			if (moneyBagPrefab == null)
			{
				Debug.LogError($"Money bag prefab not found at Resources/{MoneyBagPrefabPath}.");
				return;
			}

			//cache for future use
			foreach (MoneyBagSO moneyBag in _moneyBagVariants)
			{
				_moneyBags.Add(moneyBag.VariantName, moneyBag);
			}

			SpawnAtLocations();
		}

		private void SpawnAtLocations()
		{
			List<int> availableIndices = new List<int>(spawnLocations.Length);
			for (int i = 0; i < spawnLocations.Length; i++)
			{
				availableIndices.Add(i);
			}

			int bagsToRemove = GetBagsToRemoveCount();
			for (int i = 0; i < bagsToRemove && availableIndices.Count > 0; i++)
			{
				int randomIndex = Random.Range(0, availableIndices.Count);
				availableIndices.RemoveAt(randomIndex);
			}

			foreach (int index in availableIndices)
			{
				SpawnMoneyBag(spawnLocations[index].position, spawnLocations[index].rotation);
			}
		}

		private int GetBagsToRemoveCount()
		{
			int difficulty = PlayerPrefs.GetInt("Difficulty", 0);
			int playerCount = PlayerPrefs.GetInt("PlayerCount", MinPlayerCount);

			int bagsToRemove = difficulty switch
			{
				0 => BagsToRemoveEasy,
				1 => BagsToRemoveMedium,
				2 => BagsToRemoveHard,
				_ => BagsToRemoveEasy
			};

			int extraPlayers = Mathf.Max(0, playerCount - MinPlayerCount);
			bagsToRemove -= extraPlayers * BagsRestoredPerExtraPlayer;

			return Mathf.Clamp(bagsToRemove, 0, spawnLocations.Length);
		}

		public void SpawnMoneyBag(Vector3 position, Quaternion rotation, string variantName = MoneyBag.DEFAULTVARIANT)
		{
			if (_moneyBags.TryGetValue(variantName, out MoneyBagSO variant))
			{
				MoneyBag moneyBag = Instantiate(moneyBagPrefab, position, rotation);

				//moneyBag.SetMaterial(variant.BagMaterial);
				moneyBag.SetUp(variant.MoneyValue, moneyBagId);
				moneyBagId++;
			}
			else
			{
				Debug.LogError("Money bag variant not found.");
			}
		}
	}
}