using System;
using Game.Events;
using ianco99.ToolBox.Events;
using ianco99.ToolBox.Services;
using UnityEngine;

namespace Game
{
	public class GameManager : MonoBehaviour
	{
		EventBus EventBus = ServiceProvider.Instance.GetService<EventBus>();
		
		private void Awake()
		{
			ServiceProvider.Instance.AddService<EventBus>(new EventBus());
			
			EventBus.Subscribe<StartSinglePlayerEvent>(OnSinglePlayerHandle);	
			EventBus.Subscribe<StartMultiPlayerEvent>(OnMultiPlayerHandle);	
		}

		private void OnDestroy()
		{
			EventBus.UnSubscribe<StartSinglePlayerEvent>(OnSinglePlayerHandle);	
			EventBus.UnSubscribe<StartMultiPlayerEvent>(OnMultiPlayerHandle);	
		}

		private void OnSinglePlayerHandle(in StartSinglePlayerEvent startSinglePlayerEvent)
		{
			PlayerPrefs.SetInt("PlayerCount", 1);
		}
		
		private void OnMultiPlayerHandle(in StartMultiPlayerEvent startSinglePlayerEvent)
		{
			PlayerPrefs.SetInt("PlayerCount", 2);
		}
	}
}