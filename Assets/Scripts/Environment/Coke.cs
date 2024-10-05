using System;
using Unity.VisualScripting;
using UnityEngine;

public class Coke : Interactables
{
    private Material defaultColor;
    public Material highlightColor;

    private MeshRenderer mesh;
    private Boolean highlighted = false;
    private float timeOut;
    public float maxTimeOut = 0.3f;

    void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        defaultColor = mesh.material;
    }
    public override void Interact()
    {
        PlayerController.instance.cokes++;
        print("You picked up a coke, you now have " + PlayerController.instance.cokes);
        Destroy(gameObject);
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
