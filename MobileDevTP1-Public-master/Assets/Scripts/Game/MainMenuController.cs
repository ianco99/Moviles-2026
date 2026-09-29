using UnityEngine;

namespace Game
{
	public class MainMenuController : MonoBehaviour
	{
		// Set before loading the menu scene when the credits must be shown first (end of a match)
		public static bool OpenCreditsOnLoad;

		[SerializeField] private GameObject mainMenu;
		[SerializeField] private GameObject creditsMenu;

		private void Awake()
		{
			if (!OpenCreditsOnLoad) return;

			OpenCreditsOnLoad = false;
			mainMenu.SetActive(false);
			creditsMenu.SetActive(true);
		}
	}
}
