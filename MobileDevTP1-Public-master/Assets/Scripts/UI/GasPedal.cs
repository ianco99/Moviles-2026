using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
	public class GasPedal : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
	{
		[SerializeField] private bool isGasPedal;

		public bool IsPressed { get; private set; }

		public void OnPointerDown(PointerEventData eventData)
		{
			IsPressed = true;
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			IsPressed = false;
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			if (!hasFocus)
			{
				IsPressed = false;
			}
		}

		private void OnDisable()
		{
			IsPressed = false;
		}
	}
}