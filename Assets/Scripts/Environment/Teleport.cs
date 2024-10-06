using System;
using UnityEngine;

public class Teleport : Interactables
{
    [SerializeField] private string tooltip = "Enter the Subway";

    private Color defaultColor;
    public Color highlightColor;

    private MeshRenderer mesh;
    private Boolean highlighted = false;
    private float timeOut;
    public float maxTimeOut = 0.1f;

    public Transform tpTarget;

    void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        defaultColor = mesh.material.color;
    }
    public override void Interact()
    {
        Rigidbody rb = PlayerController.instance.GetComponent<Rigidbody>();
        rb.position = tpTarget.position;
        rb.linearVelocity = Vector3.zero;
        print(PlayerController.instance.transform.position + " " + tpTarget.position);

        TooltipHandler.instance.UpdateTooltip("");
        highlighted = false;
        mesh.material.color = defaultColor;
    }

    public override void Highlight()
    {
        TooltipHandler.instance.UpdateTooltip(tooltip);
        mesh.material.color = highlightColor;
        timeOut = maxTimeOut;
        highlighted = true;
    }

    void Update()
    {
        if (highlighted)
        {
            if (timeOut > 0)
            {
                timeOut -= Time.deltaTime;
            }
            else
            {
                TooltipHandler.instance.UpdateTooltip("");
                highlighted = false;
                mesh.material.color = defaultColor;
            }
        }

    }
}
