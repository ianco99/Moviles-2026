using ianco99.ToolBox.Events;
using ianco99.ToolBox.Services;
using UnityEngine;

namespace MoneyBags
{
	public sealed class MoneyBag : MonoBehaviour
	{
		public const string DEFAULTVARIANT = "default";

		[SerializeField] MeshRenderer _meshRenderer;

		EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();

		public float MoneyValue;

		public void SetMaterial(Material mat)
		{
			for (int i = 0; i < _meshRenderer.materials.Length; i++)
			{
				_meshRenderer.materials[i] = mat;
			}
		}

		public void SetValue(float value)
		{
			MoneyValue = value;
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.tag == "Player1")
			{
				EventBus.Raise<PickUpMoneyEvent>(0, MoneyValue);
				Destroy(gameObject);
			}
			else if (other.tag == "Player2")
			{
				EventBus.Raise<PickUpMoneyEvent>(1, MoneyValue);
				Destroy(gameObject);
			}
		}
	}
}