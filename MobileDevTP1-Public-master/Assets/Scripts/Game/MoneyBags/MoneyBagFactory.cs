using System.Collections.Generic;
using UnityEngine;

namespace MoneyBags
{
	public class MoneyBagFactory : MonoBehaviour
	{
		[SerializeField] private MoneyBag moneyBagPrefab;
		[SerializeField] private MoneyBagSO[] _moneyBagVariants;
		[SerializeField] private Transform[] spawnLocations;
		
		private Dictionary<string, MoneyBagSO> _moneyBags = new Dictionary<string, MoneyBagSO>();

		private void Awake()
		{
			//cache for future use
			foreach (MoneyBagSO moneyBag in _moneyBagVariants)
			{
				_moneyBags.Add(moneyBag.VariantName, moneyBag);
			}
			
			SpawnAtLocations();
		}

		private void SpawnAtLocations()
		{
			for (int i = 0; i < spawnLocations.Length; i++)
			{
				SpawnMoneyBag(spawnLocations[i].position, spawnLocations[i].rotation);
			}
		}

		public void SpawnMoneyBag(Vector3 position, Quaternion rotation, string variantName = MoneyBag.DEFAULTVARIANT)
		{
			if (_moneyBags.TryGetValue(variantName, out MoneyBagSO variant))
			{
				MoneyBag moneyBag = Instantiate(moneyBagPrefab, position, rotation);

				//moneyBag.SetMaterial(variant.BagMaterial);
				moneyBag.SetValue(variant.MoneyValue);
			}
			else
			{
				Debug.LogError("Money bag variant not found.");
			}
		}
	}
}