using System.Collections;
using Game.Events;
using ianco99.ToolBox.Events;
using ianco99.ToolBox.Services;
using TMPro;
using UnityEngine;

namespace Game
{
	public class ScoreController : MonoBehaviour
	{
		[SerializeField] private float returnToMenuDelay = 20f;
		[SerializeField] private Canvas scoreCanvas;
		[SerializeField] private PlayerWalletManager[] wallets;

		[Header("Single player layout")]
		[SerializeField] private GameObject singlePlayerPanel;
		[SerializeField] private TMP_Text singlePlayerScoreText;

		[Header("Multiplayer layout")]
		[SerializeField] private GameObject multiPlayerPanel;
		[SerializeField] private TMP_Text[] multiPlayerScoreTexts;

		private bool gameEnded;

		EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();

		private bool IsMultiplayer => wallets.Length > 1;

		private void Awake()
		{
			scoreCanvas.gameObject.SetActive(false);
		}

		private void Start()
		{
			// Subscribed on Start because the GameManager creates the EventBus on its Awake
			EventBus.Subscribe<GameTimerEndedEvent>(OnGameTimerEndedHandle);
		}

		private void OnDestroy()
		{
			EventBus.UnSubscribe<GameTimerEndedEvent>(OnGameTimerEndedHandle);
		}

		private void OnGameTimerEndedHandle(in GameTimerEndedEvent gameTimerEndedEvent)
		{
			if (gameEnded) return;

			gameEnded = true;
			ShowScores();
			StartCoroutine(ReturnToMenuRoutine());
		}

		private void ShowScores()
		{
			scoreCanvas.gameObject.SetActive(true);
			singlePlayerPanel.SetActive(!IsMultiplayer);
			multiPlayerPanel.SetActive(IsMultiplayer);

			if (!IsMultiplayer)
			{
				singlePlayerScoreText.text = FormatMoney(wallets[0].playerMoney);
				return;
			}

			for (int i = 0; i < wallets.Length; i++)
				multiPlayerScoreTexts[i].text = FormatMoney(wallets[i].playerMoney);
		}

		private IEnumerator ReturnToMenuRoutine()
		{
			yield return new WaitForSecondsRealtime(returnToMenuDelay);
			SceneLoader.Instance.LoadCreditsScene();
		}

		private static string FormatMoney(float money)
		{
			return "$" + (int)money;
		}
	}
}
