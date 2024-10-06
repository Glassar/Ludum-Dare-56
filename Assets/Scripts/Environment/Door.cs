using System;
using UnityEngine;

public class Door : Interactables
{
    [SerializeField] private string tooltip = "Open Door";
    private Color defaultColor;
    public Color highlightColor;

    private MeshRenderer mesh;
    private Boolean highlighted = false;
    private float timeOut;
    public float maxTimeOut = 0.1f;

    public float openAngle = 90;
    private int direction = -1;

    void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        defaultColor = mesh.material.color;
    }
    public override void Interact()
    {
        direction = -direction;

        Vector3 angle = transform.localRotation.eulerAngles;
        transform.localRotation = Quaternion.Euler(angle.x, angle.y + direction * openAngle, angle.z);
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
        if (timeOut > 0 && highlighted)
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
