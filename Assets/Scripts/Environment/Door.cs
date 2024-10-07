using System;
using Rellac.Audio;
using UnityEngine;

public class Door : Interactables
{
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private string tooltipOpen = "Open Door";
    [SerializeField] private string tooltipClose = "Close Door";
    private Color defaultColor;
    public Color highlightColor;

    private MeshRenderer mesh;
    private Boolean highlighted = false;
    private float timeOut;
    public float maxTimeOut = 0.1f;

    public float openAngle = 90;
    private int direction = -1;
    private bool open = false;

    void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        defaultColor = mesh.material.color;
    }
    public override void Interact()
    {
        if (open) soundManager.PlayOneShotRandomPitch("doorOpen", 0.05f);
        else soundManager.PlayOneShotRandomPitch("doorClose", 0.05f);
        direction = -direction;
        open = !open;
        Vector3 angle = transform.localRotation.eulerAngles;
        transform.localRotation = Quaternion.Euler(angle.x, angle.y + direction * openAngle, angle.z);
    }

    public override void Highlight()
    {
        TooltipHandler.instance.UpdateTooltip(direction == -1 ? tooltipOpen : tooltipClose);
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
