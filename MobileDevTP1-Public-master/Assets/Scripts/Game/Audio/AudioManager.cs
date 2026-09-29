using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game.Audio
{
	public class AudioManager : MonoBehaviour
	{
		[Serializable]
		public struct SceneMusic
		{
			public string sceneName;
			public AudioClip clip;
		}

		public static AudioManager Instance { get; private set; }

		[SerializeField] private AudioSource musicSource;
		[SerializeField] private AudioSource sfxSource;
		[SerializeField] private SceneMusic[] sceneMusic;
		[SerializeField] private float musicVolume = 0.6f;
		[SerializeField] private float musicFadeTime = 0.75f;

		[Header("UI")] [SerializeField] private AudioClip buttonClick;
		[SerializeField] private float buttonClickVolume = 1f;

		private Coroutine musicFadeRoutine;

		private void Awake()
		{
			// Every scene carries a copy so it can be played standalone, only the first one survives
			if (Instance != null && Instance != this)
			{
				Destroy(gameObject);
				return;
			}

			Instance = this;
			DontDestroyOnLoad(gameObject);

			musicSource.loop = true;
			musicSource.volume = 0f;
			SceneManager.sceneLoaded += OnSceneLoaded;
		}

		private void Start()
		{
			if (Instance != this) return;

			SetupScene(SceneManager.GetActiveScene());
		}

		private void OnDestroy()
		{
			if (Instance != this) return;

			SceneManager.sceneLoaded -= OnSceneLoaded;
			Instance = null;
		}

		private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
		{
			SetupScene(scene);
		}

		private void SetupScene(UnityEngine.SceneManagement.Scene scene)
		{
			PlayMusic(GetMusicFor(scene.name));
			HookButtons();
		}

		public void PlaySfx(AudioClip clip, float volume = 1f, float pitch = 1f)
		{
			if (clip == null) return;

			sfxSource.pitch = pitch;
			sfxSource.PlayOneShot(clip, volume);
		}

		public void PlayButtonClick()
		{
			PlaySfx(buttonClick, buttonClickVolume);
		}

		private AudioClip GetMusicFor(string sceneName)
		{
			foreach (SceneMusic entry in sceneMusic)
			{
				if (entry.sceneName == sceneName)
					return entry.clip;
			}

			return null;
		}

		private void PlayMusic(AudioClip clip)
		{
			if (clip == musicSource.clip && musicSource.isPlaying) return;

			if (musicFadeRoutine != null)
				StopCoroutine(musicFadeRoutine);
			musicFadeRoutine = StartCoroutine(CrossfadeMusic(clip));
		}

		private IEnumerator CrossfadeMusic(AudioClip clip)
		{
			// Unscaled so it keeps fading while the loading screen or a pause freezes time
			yield return FadeMusicTo(0f);

			musicSource.Stop();
			musicSource.clip = clip;
			if (clip == null) yield break;

			musicSource.Play();
			yield return FadeMusicTo(musicVolume);
		}

		private IEnumerator FadeMusicTo(float target)
		{
			float start = musicSource.volume;
			float elapsed = 0f;

			while (elapsed < musicFadeTime)
			{
				elapsed += Time.unscaledDeltaTime;
				musicSource.volume = Mathf.Lerp(start, target, elapsed / musicFadeTime);
				yield return null;
			}

			musicSource.volume = target;
		}

		private void HookButtons()
		{
			Button[] buttons = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
			foreach (Button button in buttons)
			{
				if (button.GetComponent<MuteButtonSound>() != null) continue;

				// Removed first so a button that survives a scene change never clicks twice
				button.onClick.RemoveListener(PlayButtonClick);
				button.onClick.AddListener(PlayButtonClick);
			}
		}
	}
}
