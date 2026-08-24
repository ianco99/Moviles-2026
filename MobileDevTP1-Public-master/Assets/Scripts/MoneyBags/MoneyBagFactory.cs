using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class MoneyBag : MonoBehaviour
{
	public const string DEFAULTVARIANT = "default";
	
	[SerializeField] MeshRenderer _meshRenderer;
	public float MoneyValue;
	
	public void SetMaterial(Material mat)
	{
		_meshRenderer.material = mat;
	}

	public void SetValue(float value)
	{
		MoneyValue = value;
	}
}

[CreateAssetMenu(menuName = "Scriptable Objects/Money Bag", fileName = "MoneyBagVariant")]
public sealed class MoneyBagSO : ScriptableObject
{
	public string VariantName = MoneyBag.DEFAULTVARIANT;
	public float MoneyValue;
	public Material BagMaterial;
}

public class MoneyBagFactory : MonoBehaviour
{
	[SerializeField] private MoneyBag moneyBagPrefab;
	[SerializeField] private List<MoneyBagSO> _moneyBagVariants;
	
	private Dictionary<string, MoneyBagSO> _moneyBags;

	private void Awake()
	{
		//cache for future use
		foreach (MoneyBagSO moneyBag in _moneyBagVariants)
		{
			_moneyBags.Add(moneyBag.VariantName, moneyBag);
		}
	}

	public void SpawnMoneyBag(Vector3 position, Quaternion rotation, string variantName = MoneyBag.DEFAULTVARIANT)
	{
		if (_moneyBags.TryGetValue(variantName, out MoneyBagSO variant))
		{
			MoneyBag moneyBag = Instantiate(moneyBagPrefab, position, rotation);
			
			moneyBag.SetMaterial(variant.BagMaterial);
			moneyBag.SetValue(variant.MoneyValue);
		}
		else
		{
			Debug.LogError("Money bag variant not found.");
		}
	}
}