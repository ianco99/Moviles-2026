using System.Collections;
using Game.Events;
using ianco99.ToolBox.Events;
using ianco99.ToolBox.Services;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
	public class SceneLoader : MonoBehaviour
	{
		public static SceneLoader Instance { get; private set; }

		private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();

		[SerializeField] private LoadingScreen loadingScreen;
		[SerializeField] private float minimumDisplayTime = 1.5f;
		[SerializeField] private float barSpeed = 2f;

		private void Awake()
		{
			if (Instance != null && Instance != this)
			{
				Destroy(gameObject);
				return;
			}

			Instance = this;
			DontDestroyOnLoad(gameObject);
		}

		public void LoadSingleplayerScene()
		{
			EventBus.Raise<StartSinglePlayerEvent>();
			Load("Gameplay1P");
		}

		public void LoadMultiplayerScene()
		{
			EventBus.Raise<StartMultiPlayerEvent>();
			Load("Gameplay2P");
		}

		public void LoadMenuScene()
		{
			Load("MenuScene");
		}

		private void Load(string sceneName)
		{
			StartCoroutine(LoadRoutine(sceneName));
		}

		private IEnumerator LoadRoutine(string sceneName)
		{
			loadingScreen.Show();
			yield return loadingScreen.FadeIn();

			AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
 			operation.allowSceneActivation = false;

			float startTime = Time.unscaledTime;
			float displayed = 0f;

			while (displayed < 1f)
			{
				float real = Mathf.Clamp01(operation.progress / 0.9f);
				float timeGate = minimumDisplayTime > 0f
					? Mathf.Clamp01((Time.unscaledTime - startTime) / minimumDisplayTime)
					: 1f;

				float target = Mathf.Min(real, timeGate);
				displayed = Mathf.MoveTowards(displayed, target, barSpeed * Time.unscaledDeltaTime);

				loadingScreen.SetProgress(displayed);
				yield return null;
			}

			operation.allowSceneActivation = true;
			yield return operation;

			yield return loadingScreen.FadeOut();
			loadingScreen.Hide();
		}
	}
}