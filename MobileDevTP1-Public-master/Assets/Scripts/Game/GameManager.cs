using System;
using System.Collections.Generic;
using Game.Events;
using ianco99.ToolBox.Events;
using ianco99.ToolBox.Services;
using UnityEngine;

namespace Game
{
	[DefaultExecutionOrder(-1000)]
	public class GameManager : MonoBehaviour
	{
		private const float GameDuration = 60f;

		private float elapsedTime;
		private bool timerEnded;

		private bool gameStarted;
		private int expectedTutorials;
		private readonly HashSet<int> completedTutorials = new HashSet<int>();

		EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();

		private void Awake()
		{
			if (ServiceProvider.Instance.ContainsService<EventBus>())
				ServiceProvider.Instance.RemoveService<EventBus>();
			ServiceProvider.Instance.AddService<EventBus>(new EventBus());

			EventBus.Subscribe<StartSinglePlayerEvent>(OnSinglePlayerHandle);
			EventBus.Subscribe<StartMultiPlayerEvent>(OnMultiPlayerHandle);
			EventBus.Subscribe<TutorialCompletedEvent>(OnTutorialCompleted);
		}

		private void Start()
		{
			expectedTutorials = FindObjectsByType<TutorialController>(FindObjectsSortMode.None).Length;
		}

		private void Update()
		{
			if (!gameStarted && expectedTutorials == 0)
				StartGame();

			if (!gameStarted || timerEnded) return;

			elapsedTime += Time.deltaTime;
			if (elapsedTime >= GameDuration)
			{
				timerEnded = true;
				EventBus.Raise<GameTimerEndedEvent>();
			}
		}

		private void OnDestroy()
		{
			EventBus.UnSubscribe<StartSinglePlayerEvent>(OnSinglePlayerHandle);
			EventBus.UnSubscribe<StartMultiPlayerEvent>(OnMultiPlayerHandle);
			EventBus.UnSubscribe<TutorialCompletedEvent>(OnTutorialCompleted);
		}

		private void OnSinglePlayerHandle(in StartSinglePlayerEvent startSinglePlayerEvent)
		{
			PlayerPrefs.SetInt("PlayerCount", 1);
		}

		private void OnMultiPlayerHandle(in StartMultiPlayerEvent startSinglePlayerEvent)
		{
			PlayerPrefs.SetInt("PlayerCount", 2);
		}

		private void OnTutorialCompleted(in TutorialCompletedEvent callback)
		{
			if (gameStarted) return;

			completedTutorials.Add(callback.playerID);
			if (completedTutorials.Count >= expectedTutorials)
				StartGame();
		}

		private void StartGame()
		{
			gameStarted = true;
			EventBus.Raise<GameStartedEvent>();
		}
	}
}
