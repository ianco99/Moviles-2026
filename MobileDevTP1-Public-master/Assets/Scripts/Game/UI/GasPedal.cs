using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
	public class GasPedal : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
	{
		[SerializeField] private bool isGasPedal;

		public bool IsPressed { get; private set; }

		private Image _visual;
		
		private void Awake()
		{
			_visual = GetComponent<Image>();
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			IsPressed = true;
			_visual.color = Color.gray;
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			_visual.color = Color.white;
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