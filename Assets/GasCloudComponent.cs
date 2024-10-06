using System;
using UnityEngine;

public class GasCloudComponent : MonoBehaviour
{
    public static event Action gasDamage;
    [SerializeField] private float damageTickTime = 1f;
    [SerializeField] private string playerTag;

    private float damageTickTimer = 0f;

    // void OntriggerEnter(Collider other) {
    //     if (other == null) return;
    // }
    
    void OnTriggerStay(Collider other)
    {
        if (other == null) return;
        if (other.CompareTag(playerTag)) {
            if (damageTickTimer > damageTickTime) {
                gasDamage.Invoke();
                damageTickTimer = 0f;
            } else {
                damageTickTimer += Time.deltaTime;
            }
        }
    }
}
