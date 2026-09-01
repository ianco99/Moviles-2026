using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
	public class LoadingScreen : MonoBehaviour
	{
		[SerializeField] private CanvasGroup canvasGroup;
		[SerializeField] private Image progressFill;
		[SerializeField] private float fadeDuration = 0.3f;

		public void Show()
		{
			gameObject.SetActive(true);
			canvasGroup.alpha = 0f;
			canvasGroup.blocksRaycasts = true;
			SetProgress(0f);
		}

		public void Hide()
		{
			canvasGroup.blocksRaycasts = false;
			gameObject.SetActive(false);
		}

		public void SetProgress(float value)
		{
			progressFill.fillAmount = value;
		}

		public IEnumerator FadeIn()
		{
			return Fade(1f);
		}

		public IEnumerator FadeOut()
		{
			return Fade(0f);
		}

		private IEnumerator Fade(float target)
		{
			float start = canvasGroup.alpha;
			float elapsed = 0f;

			while (elapsed < fadeDuration)
			{
				elapsed += Time.unscaledDeltaTime;
				canvasGroup.alpha = Mathf.Lerp(start, target, elapsed / fadeDuration);
				yield return null;
			}

			canvasGroup.alpha = target;
		}
	}
}