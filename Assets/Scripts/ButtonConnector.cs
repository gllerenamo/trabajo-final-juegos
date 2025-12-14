using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class ButtonConnector : MonoBehaviour
{
    void Start()
    {
        ConnectButtons();
    }

    void ConnectButtons()
    {
        // Buscar botones en la escena
        Button[] buttons = FindObjectsByType<Button>(FindObjectsSortMode.None);

        foreach (Button button in buttons)
        {
            // Conectar según el nombre del botón
            switch (button.name)
            {
                case "NextLevelButton":
                case "SiguienteNivelButton":
                case "ButtonNext":
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() => GameManager.instance.NextLevel());
                    Debug.Log("✓ NextLevelButton conectado");
                    break;

                case "RestartButton":
                case "ReiniciarnButton":
                case "ButtonRestart":
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() => GameManager.instance.RestartLevel());
                    Debug.Log("✓ RestartButton conectado");
                    break;

                case "MenuButton":
                case "MainMenuButton":
                case "ButtonMenu":
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() => GameManager.instance.MainMenu());
                    Debug.Log("✓ MenuButton conectado");
                    break;
            }
        }
    }
}