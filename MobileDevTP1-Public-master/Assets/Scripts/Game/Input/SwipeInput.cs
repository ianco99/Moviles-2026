using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace Game
{
	// Directional input for the tutorial and minigame steps:
	// PC reads the "Controls" action of the current map, mobile reads whole swipes inside this player's screen region
	public class SwipeInput : MonoBehaviour
	{
		[SerializeField] private PlayerInput playerInput;

		[Header("Swipe")]
		[Tooltip("Normalized screen area where this player's swipes start (left half is 0, 0, 0.5, 1)")]
		[SerializeField] private Rect screenRegion = new Rect(0f, 0f, 1f, 1f);
		[Tooltip("Minimum swipe length, as a fraction of the screen height")]
		[SerializeField] private float minSwipeDistance = 0.08f;
		[SerializeField] private float maxSwipeDuration = 0.5f;
		[Tooltip("How many times bigger the main axis must be than the other one")]
		[SerializeField] private float axisDominance = 2f;

#if ANDROID_BUILD
		private Vector2 swipeDirection;
		private int swipeFrame = -1;
#endif

		public Vector2 ReadDirection()
		{
#if PC_BUILD
			return playerInput.currentActionMap["Controls"].ReadValue<Vector2>();
#elif ANDROID_BUILD
			// A swipe is only valid right after it ends and can be read once
			if (Time.frameCount - swipeFrame > 1)
				return Vector2.zero;

			Vector2 direction = swipeDirection;
			swipeDirection = Vector2.zero;
			return direction;
#else
			return Vector2.zero;
#endif
		}

#if ANDROID_BUILD
		private void OnEnable()
		{
			EnhancedTouchSupport.Enable();
		}

		private void OnDisable()
		{
			EnhancedTouchSupport.Disable();
		}

		private void Update()
		{
			foreach (Touch touch in Touch.activeTouches)
			{
				if (touch.phase != TouchPhase.Ended || !IsInRegion(touch.startScreenPosition))
					continue;

				if (TryGetSwipeDirection(touch, out Vector2 direction))
				{
					swipeDirection = direction;
					swipeFrame = Time.frameCount;
				}
			}
		}

		private bool IsInRegion(Vector2 screenPosition)
		{
			Vector2 normalized = new Vector2(screenPosition.x / Screen.width, screenPosition.y / Screen.height);
			return screenRegion.Contains(normalized);
		}

		private bool TryGetSwipeDirection(Touch touch, out Vector2 direction)
		{
			direction = Vector2.zero;

			float duration = (float)(touch.time - touch.startTime);
			if (duration > maxSwipeDuration)
				return false;

			Vector2 delta = touch.screenPosition - touch.startScreenPosition;
			if (delta.magnitude < minSwipeDistance * Screen.height)
				return false;

			float absX = Mathf.Abs(delta.x);
			float absY = Mathf.Abs(delta.y);

			if (absX >= absY * axisDominance)
				direction = new Vector2(Mathf.Sign(delta.x), 0f);
			else if (absY >= absX * axisDominance)
				direction = new Vector2(0f, Mathf.Sign(delta.y));
			else
				return false;

			return true;
		}
#endif
	}
}
