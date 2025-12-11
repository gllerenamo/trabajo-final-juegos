using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UIElements;
using static UnityEditor.Searcher.SearcherWindow.Alignment;
using static UnityEngine.Rendering.DebugUI;

public class LabyrintWall : MonoBehaviour
{
    [Header("Visual")]
    public Color wallColor = new Color(0.3f, 0.3f, 0.4f);
    public bool useEmission = true;

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null && renderer.material != null)
        {
            renderer.material.color = wallColor;

            if (useEmission)
            {
                renderer.material.EnableKeyword("_EMISSION");
                renderer.material.SetColor("_EmissionColor", wallColor * 0.3f);
            }
        }
    }
}