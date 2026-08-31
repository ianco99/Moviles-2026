using System;
using ianco99.ToolBox.Events;
using ianco99.ToolBox.Services;
using UnityEngine;

namespace MoneyBags
{
	public sealed class MoneyBag : MonoBehaviour
	{
		public const string DEFAULTVARIANT = "default";
		EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();

		[SerializeField] private ParticleSystem pickUpParticles;

		[SerializeField] private MeshRenderer meshRenderer;
		[SerializeField] private SphereCollider sphereCollider;
		[SerializeField] private Light light;
		

		public float MoneyValue;

		public void SetMaterial(Material mat)
		{
			for (int i = 0; i < meshRenderer.materials.Length; i++)
			{
				meshRenderer.materials[i] = mat;
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
				
				OnPickUp();
				
			}
			else if (other.tag == "Player2")
			{
				EventBus.Raise<PickUpMoneyEvent>(1, MoneyValue);
				
				OnPickUp();
				
			}
		}

		private void OnPickUp()
		{
			pickUpParticles.Play();
			sphereCollider.enabled = false;
			meshRenderer.enabled = false;
			light.enabled = false;
			
			Destroy(gameObject, 4.0f);
		}
		
	}
}