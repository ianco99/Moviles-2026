using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scene
{
	[RequireComponent(typeof(BoxCollider))]
	public class CityRegion : MonoBehaviour
	{
		private BoxCollider boxCollider;

		private Dictionary<Transform, bool> playerTransform;

		public int RegionId;

		public Action<int> playerLeftArea;
		public Action<int> playerEnteredArea;

		private void Awake()
		{
			boxCollider = GetComponent<BoxCollider>();
		}

		private void Update()
		{
			foreach (KeyValuePair<Transform, bool> playerTransform in playerTransform)
			{
				bool inside = CheckInside(playerTransform.Key);

				if (playerTransform.Key != inside)
				{
					if (inside)
						playerEnteredArea?.Invoke(RegionId);
					else
						playerLeftArea?.Invoke(RegionId);
					
					this.playerTransform[playerTransform.Key] = inside;
				}
			}
		}

		public void SetUp(Transform[] playerTransform)
		{
			this.playerTransform = new Dictionary<Transform, bool>();
			
			return;
			
			foreach (Transform player in playerTransform)
			{
				bool isPlayerInside = CheckInside(player);
				if (isPlayerInside)
				{
					
					return;
				}

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