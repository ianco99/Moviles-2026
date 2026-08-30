using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scene
{
	public sealed class CityWaveController : MonoBehaviour
	{
		[SerializeField] private CityRegion[] cityRegions;
		private Dictionary<Transform, int> _players = new Dictionary<Transform, int>();

		public void Awake()
		{
			GetPlayers();
			SetUpRegions();
		}

		private void GetPlayers()
		{
			GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

			for (int i = 0; i < players.Length; i++)
			{
				_players.Add(players[i].transform, 0);
			}
		}

		private void SetUpRegions()
		{
			for (int i = 0; i < cityRegions.Length; i++)
			{
				cityRegions[i].RegionId = i;

				cityRegions[i].SetUp(_players.Keys.ToArray());

				cityRegions[i].playerEnteredArea += PlayerEnteredRegion;
				cityRegions[i].playerLeftArea += PlayerExitedRegion;
			}
		}

		private void PlayerEnteredRegion(int playerId)
		{
			
		}
		
		private void PlayerExitedRegion(int playerId)
		{
			
		}

		private void CheckPlayersRegions()
		{
			foreach (KeyValuePair<Transform, int> player in _players)
			{
				int playerRegion = CheckPlayerRegion(player.Key);

				if (playerRegion != -1)
				{
					CityRegion cityRegion = cityRegions[playerRegion];
				}
			}
		}

		private int CheckPlayerRegion(Transform player)
		{
			foreach (CityRegion cityRegion in cityRegions)
			{
				if (cityRegion.CheckInside(player))
				{
					return cityRegion.RegionId;
				}
			}

			return -1;
		}
	}
}