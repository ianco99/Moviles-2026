using UnityEngine;

namespace MoneyBags
{
	public sealed class MoneyBag : MonoBehaviour
	{
		public const string DEFAULTVARIANT = "default";

		[SerializeField] MeshRenderer _meshRenderer;
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
	}
}