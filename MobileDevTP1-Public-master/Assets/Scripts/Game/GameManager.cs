using System;
using ianco99.ToolBox.Events;
using ianco99.ToolBox.Services;
using UnityEngine;

namespace Game
{
	public class GameManager : MonoBehaviour
	{
		private void Awake()
		{
			ServiceProvider.Instance.AddService<EventBus>(new EventBus());
		}
	}
}