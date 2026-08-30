using UnityEngine;

namespace MoneyBags
{
	[CreateAssetMenu(menuName = "Scriptable Objects/Money Bag", fileName = "MoneyBagVariant")]
	public sealed class MoneyBagSO : ScriptableObject
	{
		public string VariantName = MoneyBag.DEFAULTVARIANT;
		public float MoneyValue;
		public Material BagMaterial;
	}
}