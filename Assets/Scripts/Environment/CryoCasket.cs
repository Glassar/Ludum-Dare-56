using System;
using Unity.VisualScripting;
using UnityEngine;

public class CryoCasket : Interactables
{
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
            print("Level Finished");
            PlayerController.instance.Reset();
        }
        else
        {
            print("Not enough cokes");
        }
    }

    public override void Highlight()
    {
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
            highlighted = false;
            mesh.material = defaultColor;
        }
    }
}
