using System;
using Unity.VisualScripting;
using UnityEngine;

public class Coke : Interactables
{
    [SerializeField] private string tooltip = "Pick up Carbonated BioFuel™";
    private Material defaultColor;
    public Material highlightColor;

    private MeshRenderer mesh;
    private Boolean highlighted = false;
    private float timeOut;
    public float maxTimeOut = 0.1f;

    void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        defaultColor = mesh.material;
    }
    public override void Interact()
    {
        PlayerController.instance.cokes++;
        Debug.Log("You picked up a coke, you now have " + PlayerController.instance.cokes);
        Destroy(gameObject);
    }

    public override void Highlight()
    {
        TooltipHandler.instance.UpdateTooltip(tooltip);
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
