using System;
using Unity.VisualScripting;
using UnityEngine;

public class CryoCasket : Interactables
{
    [SerializeField] private string tooltipA = "Somnum Device Requires Fuel";
    [SerializeField] private string tooltipB = "Enter Somnum Device";
    private Material defaultColor;
    public Material highlightColor;

    private MeshRenderer mesh;
    private Boolean highlighted = false;
    private float timeOut;
    public float maxTimeOut = 0.1f;

    public int cokeCount = 4;

    void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        defaultColor = mesh.material;
    }
    public override void Interact()
    {
        if (PlayerController.instance.cokes >= cokeCount)
        {
            Debug.Log("Level Finished");
            PlayerController.instance.Reset();
        }
        else
        {
            Debug.Log("Not enough cokes");
        }
    }

    public override void Highlight()
    {
        if (PlayerController.instance.cokes >= cokeCount)
        {
            TooltipHandler.instance.UpdateTooltip(tooltipB);
        }
        else
        {
            TooltipHandler.instance.UpdateTooltip(tooltipA);
        }

        mesh.material = highlightColor;
        timeOut = maxTimeOut;
        highlighted = true;
    }

    void Update()
    {
        if (timeOut > 0 && highlighted)
        {
            timeOut -= Time.deltaTime;
        }
        else
        {
            TooltipHandler.instance.UpdateTooltip("");
            highlighted = false;
            mesh.material = defaultColor;
        }
    }
}
