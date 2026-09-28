using Game.Events;
using ianco99.ToolBox.Events;
using ianco99.ToolBox.Services;
using UnityEngine;

namespace Game
{
	public class EndGameManager : MonoBehaviour
	{
		EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();

		private void Awake()
		{
			EventBus.Subscribe<GameTimerEndedEvent>(OnGameTimerEndedHandle);
		}

		private void OnDestroy()
		{
			EventBus.UnSubscribe<GameTimerEndedEvent>(OnGameTimerEndedHandle);
		}

		private void OnGameTimerEndedHandle(in GameTimerEndedEvent gameTimerEndedEvent)
		{
			Debug.Log("Game timer ended");
			EndGame();
		}

		private void EndGame()
		{
		}
	}
}
