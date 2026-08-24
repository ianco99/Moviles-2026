using UnityEngine;
using UnityEngine.EventSystems;

public class SteeringWheel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
	[SerializeField] private RectTransform visualWheel;

	private const float deadZoneRadius = 0.05f;

	private RectTransform hitArea;
	private bool isHeld;
	private Vector2 activeScreenPosition;
	private Camera activeCamera;

	public float TurnDir = 0;

	private void Awake()
	{
		hitArea = GetComponent<RectTransform>();
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		isHeld = true;
		activeScreenPosition = eventData.position;
		activeCamera = eventData.pressEventCamera;
	}

	public void OnDrag(PointerEventData eventData)
	{
		activeScreenPosition = eventData.position;
		activeCamera = eventData.pressEventCamera;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		isHeld = false;
	}

	private void Update()
	{
		if (isHeld)
		{
			Vector2 unitCoord = GetNormalizedPoint(activeScreenPosition, activeCamera);
			Vector2 fromCenter = unitCoord - new Vector2(0.5f, 0.5f);

			if (fromCenter.sqrMagnitude < deadZoneRadius * deadZoneRadius)
			{
				return;
			}

			float angle = Mathf.Atan2(fromCenter.x, fromCenter.y) * Mathf.Rad2Deg;
			visualWheel.localRotation = Quaternion.Euler(0f, 0f, -angle);
		}

		Vector3 sas = new Vector3(1f, 0, 0);

		TurnDir = Vector3.Project(visualWheel.up, sas).x;
	}

	private Vector2 GetNormalizedPoint(Vector2 screenPos, Camera cam)
	{
		RectTransformUtility.ScreenPointToLocalPointInRectangle(hitArea, screenPos, cam, out Vector2 localPoint);
		Rect rect = hitArea.rect;
		return new Vector2((localPoint.x - rect.x) / rect.width, (localPoint.y - rect.y) / rect.height);
	}
}