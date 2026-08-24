using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scene
{
	public class CityRegion : MonoBehaviour
	{
		[SerializeField] private BoxCollider boxCollider;

		private Dictionary<Transform, bool> _playerTransform;

		public int RegionId;

		public Action<int> playerLeftArea;
		public Action<int> playerEnteredArea;

		private void Update()
		{
			foreach (KeyValuePair<Transform, bool> playerTransform in _playerTransform)
			{
				bool inside = CheckInside(playerTransform.Key);

				if (playerTransform.Key != inside)
				{
					if (inside)
						playerEnteredArea?.Invoke(RegionId);
					else
						playerLeftArea?.Invoke(RegionId);
					
					_playerTransform[playerTransform.Key] = inside;
				}
			}
		}

		public void SetUp(Transform[] playerTransform)
		{
			foreach (Transform player in playerTransform)
			{
				bool sas = CheckInside(player);
				
				
			}
		}

		public bool CheckInside(Transform player)
		{
			if (boxCollider.bounds.Contains(player.transform.position))
			{
				return true;
			}

			return false;
		}
	}
}