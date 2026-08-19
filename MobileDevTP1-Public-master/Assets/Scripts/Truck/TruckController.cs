using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class TruckController : MonoBehaviour
{
    [Header("Player Properties")]
    [SerializeField] private bool player1;
    [SerializeField] private PlayerInput input;
    
    [Header("Car Properties")]
    public float motorTorque = 2000f;
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
    
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.centerOfMass += Vector3.up * centreOfGravityOffset;
        wheels = GetComponentsInChildren<WheelControl>();
        steeringAction = input.actions["Steer"];
        gasAction = input.actions["Gas"];
        brakeAction = input.actions["Brake"];
    }

    void FixedUpdate()
    {
        float vInput = 0;
        float hInput = 0;

        if (player1)
        {
            hInput = steeringAction.ReadValue<float>();
            vInput = gasAction.IsPressed() ? vInput = 1.0f : 0f;
            vInput = brakeAction.IsPressed() ? vInput = -1.0f : vInput;
        }
        else
        {
            
        }
        
        float forwardSpeed = Vector3.Dot(transform.forward, rigidBody.linearVelocity);
        float speedFactor = Mathf.Clamp01(Mathf.Abs(forwardSpeed) / maxSpeed);

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