using System;
using UnityEngine;

public class Teleport : Interactables
{
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
        PlayerController.instance.transform.position = tpTarget.position;
    }

    public override void Highlight()
    {
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
            highlighted = false;
            mesh.material.color = defaultColor;
        }

    }
}
