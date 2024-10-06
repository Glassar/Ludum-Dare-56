using System;
using UnityEngine;

public class GasCloudComponent : MonoBehaviour
{
    public static event Action gasDamage;
    [SerializeField] private float damageTickTime = 1f;
    [SerializeField] private string playerTag;

    // void OntriggerEnter(Collider other) {
    //     if (other == null) return;
    // }

    void OnTriggerStay(Collider other)
    {
        if (other == null) return;
        if (other.CompareTag(playerTag))
        {
            if (PlayerController.instance.gasTimer > damageTickTime)
            {
                gasDamage.Invoke();
                PlayerController.instance.gasTimer = 0f;
            }
            else
            {
                PlayerController.instance.gasTimer += Time.deltaTime;
            }
        }
    }
}
