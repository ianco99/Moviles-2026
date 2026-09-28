using System;
using UnityEngine;

public class BoxManager : MonoBehaviour
{
    [SerializeField] private GameObject[] boxSpawns;

    private void Start()
    {
		int difficulty = PlayerPrefs.GetInt("Difficulty");

		switch (difficulty)
		{
			case 0:
				boxSpawns[0].SetActive(true);
				break;
			case 1:
				boxSpawns[0].SetActive(true);
				boxSpawns[1].SetActive(true);
				break;
			case 2:
				boxSpawns[0].SetActive(true);
				boxSpawns[1].SetActive(true);
				boxSpawns[2].SetActive(true);
				break;
		}
    }
}
