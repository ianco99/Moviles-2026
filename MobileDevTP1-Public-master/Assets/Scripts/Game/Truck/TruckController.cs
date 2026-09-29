using Game.Bank;
using Game.Events;
using ianco99.ToolBox.Events;
using ianco99.ToolBox.Services;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class TruckController : MonoBehaviour
{
	private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();
	
	[Header("Player Properties")] [SerializeField]
	public int playerID;

	[SerializeField] private PlayerInput input;
	[SerializeField] private Camera camera;
	[SerializeField] private SteeringWheel steeringWheel;
	[SerializeField] private UIPedals uiPedals;

	[Header("Car Properties")] public float motorTorque = 2000f;
	public float brakeTorque = 2000f;
	public float maxSpeed = 20f;
	public float steeringRange = 30f;
	public float steeringRangeAtMaxSpeed = 10f;
	public float centreOfGravityOffset = -1f;

	private WheelControl[] wheels;
	private Rigidbody rigidBody;

	private InputAction steeringAction;
	private InputAction gasAction;
	private InputAction brakeAction;

	private bool canDrive;

	// Read by the engine audio
	public float Throttle { get; private set; }
	public float SpeedFactor { get; private set; }

	void Start()
	{
		rigidBody = GetComponent<Rigidbody>();
		rigidBody.centerOfMass += Vector3.up * centreOfGravityOffset;
		wheels = GetComponentsInChildren<WheelControl>();

		EventBus.Subscribe<StartMinigameEvent>(OnStartMinigame);
		EventBus.Subscribe<EndMinigameEvent>(OnEndMinigame);
		EventBus.Subscribe<GameStartedEvent>(OnGameStarted);

		// Waits for every player to finish the tutorial
		camera.gameObject.SetActive(false);
		steeringWheel.gameObject.SetActive(false);
		uiPedals.gameObject.SetActive(false);

#if PC_BUILD
        steeringAction = input.actions["Steer"];
		gasAction = input.actions["Gas"];
		brakeAction = input.actions["Brake"];
#endif
	}

	private void OnDestroy()
	{
		EventBus.UnSubscribe<StartMinigameEvent>(OnStartMinigame);
		EventBus.UnSubscribe<EndMinigameEvent>(OnEndMinigame);
		EventBus.UnSubscribe<GameStartedEvent>(OnGameStarted);
	}

	private void OnGameStarted(in GameStartedEvent callback)
	{
		canDrive = true;
		camera.gameObject.SetActive(true);

#if ANDROID_BUILD
		steeringWheel.gameObject.SetActive(true);
		uiPedals.gameObject.SetActive(true);
#endif
	}

	private void OnEndMinigame(in EndMinigameEvent callback)
	{
		if (callback.playerID == playerID)
		{
		camera.gameObject.SetActive(true);
		}
	}

	private void OnStartMinigame(in StartMinigameEvent callback)
	{
		if (callback.playerID == playerID)
		{
			camera.gameObject.SetActive(false);
			rigidBody.linearVelocity = Vector3.zero;
			rigidBody.angularVelocity = Vector3.zero;
		}
	}

	void FixedUpdate()
	{
		if (!canDrive)
		{
			foreach (WheelControl wheel in wheels)
			{
				wheel.WheelCollider.motorTorque = 0f;
				wheel.WheelCollider.brakeTorque = brakeTorque;
			}
			Throttle = 0f;
			SpeedFactor = 0f;
			return;
		}

		float vInput = 0;
		float hInput = 0;

#if PC_BUILD
		hInput = steeringAction.ReadValue<float>();
		vInput = gasAction.IsPressed() ? 1.0f : 0.0f;
		vInput = brakeAction.IsPressed() ? vInput = -1.0f : vInput;
#endif

#if ANDROID_BUILD

		hInput = steeringWheel.TurnDir;
		vInput = uiPedals.pedalGas.IsPressed ? vInput = 1.0f : 0f;
		vInput = uiPedals.pedalBrake.IsPressed ? vInput = -1.0f : vInput;
#endif

		float forwardSpeed = Vector3.Dot(transform.forward, rigidBody.linearVelocity);
		float speedFactor = Mathf.Clamp01(Mathf.Abs(forwardSpeed) / maxSpeed);

		Throttle = vInput;
		SpeedFactor = speedFactor;

		float steerAngle = hInput * Mathf.Lerp(steeringRange, steeringRangeAtMaxSpeed, speedFactor);
		bool isAccelerating = vInput * forwardSpeed >= 0f;

		float motor = isAccelerating ? vInput * motorTorque * (1f - speedFactor) : 0f;
		float brake = isAccelerating ? 0f : Mathf.Abs(vInput) * brakeTorque;

		foreach (WheelControl wheel in wheels)
		{
			if (wheel.steerable) wheel.WheelCollider.steerAngle = steerAngle;
			if (wheel.motorized) wheel.WheelCollider.motorTorque = motor;
			wheel.WheelCollider.brakeTorque = brake;
		}
	}
}