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
		public uint id;

		private void Start()
		{
			EventBus.Subscribe<PickUpMoneyAcceptedEvent>(OnPickUpMoney);
		}

		private void OnPickUpMoney(in PickUpMoneyAcceptedEvent callback)
		{
			if (callback.id == id)
			{
				OnPickUp();
			}
		}

		public void SetMaterial(Material mat)
		{
			for (int i = 0; i < meshRenderer.materials.Length; i++)
			{
				meshRenderer.materials[i] = mat;
			}
		}

		public void SetUp(float value, uint id)
		{
			MoneyValue = value;
			this.id = id;
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.tag == "Player1")
			{
				EventBus.Raise<PickUpMoneyRequestEvent>(0, MoneyValue, id);
			}
			else if (other.tag == "Player2")
			{
				EventBus.Raise<PickUpMoneyRequestEvent>(1, MoneyValue, id);
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