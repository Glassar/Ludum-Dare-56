using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthVisualsHandler : MonoBehaviour
{
    [SerializeField] private RawImage healthOverlay;

    public void UpdateHealthImage(float healthPercentage) {
        float transparency  = 1f - healthPercentage;
        Color imgColor = Color.white;
        imgColor.a = transparency;
        healthOverlay.color = imgColor;
    }
}
