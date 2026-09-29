using Game.Bank;
using Game.Events;
using ianco99.ToolBox.Events;
using ianco99.ToolBox.Services;
using UnityEngine;

namespace Game.Audio
{
	[RequireComponent(typeof(AudioSource))]
	[RequireComponent(typeof(TruckController))]
	public class TruckEngineAudio : MonoBehaviour
	{
		[SerializeField] private AudioClip engineLoop;

		[Header("Pitch")] [SerializeField] private float minPitch = 0.8f;
		[SerializeField] private float maxPitch = 2.0f;
		[SerializeField] private float throttlePitchBoost = 0.25f;

		[Header("Volume")] [SerializeField] private float idleVolume = 0.35f;
		[SerializeField] private float revVolume = 0.8f;

		[SerializeField] private float smoothing = 6f;
		[SerializeField] private float stopFadeTime = 1f;

		private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();

		private AudioSource source;
		private TruckController truck;
		private bool running;
		private bool stopping;

		private void Awake()
		{
			source = GetComponent<AudioSource>();
			truck = GetComponent<TruckController>();

			source.clip = engineLoop;
			source.loop = true;
			source.playOnAwake = false;
			// 2D: split screen has several listeners, so positional audio would pan unreliably
			source.spatialBlend = 0f;
			source.pitch = minPitch;
			source.volume = idleVolume;
		}

		private void Start()
		{
			EventBus.Subscribe<GameStartedEvent>(OnGameStarted);
			EventBus.Subscribe<StartMinigameEvent>(OnStartMinigame);
			EventBus.Subscribe<EndMinigameEvent>(OnEndMinigame);
			EventBus.Subscribe<GameTimerEndedEvent>(OnGameTimerEnded);
		}

		private void OnDestroy()
		{
			EventBus.UnSubscribe<GameStartedEvent>(OnGameStarted);
			EventBus.UnSubscribe<StartMinigameEvent>(OnStartMinigame);
			EventBus.UnSubscribe<EndMinigameEvent>(OnEndMinigame);
			EventBus.UnSubscribe<GameTimerEndedEvent>(OnGameTimerEnded);
		}

		private void Update()
		{
			if (!running) return;

			if (stopping)
			{
				source.volume = Mathf.MoveTowards(source.volume, 0f, Time.deltaTime / stopFadeTime);
				if (source.volume <= 0f)
				{
					source.Stop();
					running = false;
				}
				return;
			}

			// Holding the gas revs the engine above what the current speed alone gives
			float throttle = Mathf.Max(0f, truck.Throttle);
			float targetPitch = Mathf.Lerp(minPitch, maxPitch, truck.SpeedFactor) + throttle * throttlePitchBoost;
			float targetVolume = Mathf.Lerp(idleVolume, revVolume, Mathf.Max(throttle, truck.SpeedFactor));

			float t = smoothing * Time.deltaTime;
			source.pitch = Mathf.Lerp(source.pitch, targetPitch, t);
			source.volume = Mathf.Lerp(source.volume, targetVolume, t);
		}

		private void OnGameStarted(in GameStartedEvent callback)
		{
			if (engineLoop == null) return;

			running = true;
			source.Play();
		}

		private void OnStartMinigame(in StartMinigameEvent callback)
		{
			if (callback.playerID == truck.playerID && running)
				source.Pause();
		}

		private void OnEndMinigame(in EndMinigameEvent callback)
		{
			if (callback.playerID == truck.playerID && running && !stopping)
				source.UnPause();
		}

		private void OnGameTimerEnded(in GameTimerEndedEvent callback)
		{
			stopping = true;
		}
	}
}
