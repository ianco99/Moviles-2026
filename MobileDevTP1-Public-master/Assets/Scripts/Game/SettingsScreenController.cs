using UnityEngine;

public class SettingsScreenController : MonoBehaviour
{
    public void SetDifficultyOption(int option)
    {
        switch (option)
        {
            case 0:
                PlayerPrefs.SetInt("Difficulty", 0);
                break;
            case 1:
                PlayerPrefs.SetInt("Difficulty", 1);
                break;
            case 2:
                PlayerPrefs.SetInt("Difficulty", 2);
                break;
        }
    }
}
