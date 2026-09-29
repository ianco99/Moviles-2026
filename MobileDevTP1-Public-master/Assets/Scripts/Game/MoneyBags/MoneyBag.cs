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

		[Header("Bobbing")]
		[SerializeField] private float bobAmplitude = 0.25f;
		[SerializeField] private float bobFrequency = 1.5f;

		public float MoneyValue;
		public uint id;

		private Vector3 startPosition;
		private float bobPhase;
		private bool pickedUp;

		private void Start()
		{
			startPosition = transform.position;
			// Random phase so neighbouring bags don't bob in sync
			bobPhase = UnityEngine.Random.Range(0f, 2f * Mathf.PI);
			EventBus.Subscribe<PickUpMoneyAcceptedEvent>(OnPickUpMoney);
		}

		private void Update()
		{
			if (pickedUp)
				return;

			float offset = Mathf.Sin(Time.time * bobFrequency * 2f * Mathf.PI + bobPhase) * bobAmplitude;
			transform.position = startPosition + Vector3.up * offset;
		}

		private void OnDestroy()
		{
			EventBus.UnSubscribe<PickUpMoneyAcceptedEvent>(OnPickUpMoney);
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
			// Read the id from the truck itself, both trucks share the prefab tag
			if (other.TryGetComponent(out TruckController truck))
			{
				EventBus.Raise<PickUpMoneyRequestEvent>(truck.playerID, MoneyValue, id);
			}
		}

		private void OnPickUp()
		{
			pickedUp = true;
			pickUpParticles.Play();
			sphereCollider.enabled = false;
			meshRenderer.enabled = false;
			light.enabled = false;
			
			Destroy(gameObject, 4.0f);
		}
		
	}
}