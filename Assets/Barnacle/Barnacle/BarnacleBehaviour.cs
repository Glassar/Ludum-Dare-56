using System.Collections;
using System.Collections.Generic;
using Rellac.Audio;
using UnityEngine;

public class BarnacleBehaviour : Enemy
{
    [SerializeField] Transform mouth;
    bool dead;
    private bool dying;
    public float health;
    private Animator animator;
    private bool isAnimating;
    public bool attack;
    private bool attacking;
    public float attack_activation_range;
    public PlayerController player;
    [SerializeField] private SoundManager soundManager;
    public float attackDamage;

    //PlayerController player;
    // Start is called before the first frame update
    void Start()
    {
        dead = false;
        animator = GetComponentInChildren<Animator>();
        attacking = false;
        player = PlayerController.instance;
    }

    // Update is called once per frame
    void Update()
    {
        if(health <= 0){
            dead = true;
        }

        if(Vector3.Distance(player.transform.position , mouth.transform.position) < attack_activation_range){
             attack = true;
        }

        if(!dead){
            if(attack){
                if(!attacking){
                    Debug.Log("Attacking");
                    animator.SetTrigger("Attack");
                    attacking = true;
                }else{
                    if(animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")){
                        player.transform.position = mouth.transform.position; //Make player follow mouth towards "sack" when snatched
                    }
                    else{
                        // Attack animation done
                        attacking = false;
                        attack = false;
                        // Insert code here to kill player
                        PlayerController.instance.TakeDamage(attackDamage);
                    }
                }
            }else{
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") && !isAnimating)
                {
                    isAnimating = true; 
                }
                if (isAnimating && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                {
                    animator.SetTrigger("Idle");
                    isAnimating = false; 
                }
            }   
        }

        if(dead){
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("DeadIdle") && !isAnimating)
            {
                isAnimating = true; 
            }


             if (isAnimating && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                animator.SetTrigger("DeadIdle");
                isAnimating = false; 
            }
        }
    }

    public override void TakeDamage(float dmg) {
        health -= dmg;
        soundManager.PlayOneShotRandomPitch("crawlerDamage", 0.05f);
        if (health <= 0) Invoke(nameof(DestroyEnemy), 0.5f);
    }

    private void DestroyEnemy()
    {
        soundManager.PlayOneShotRandomPitch("crawlerDeath", 0.05f);
        transform.GetComponent<Collider>().enabled = false;
        dead = true;
    }



    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(mouth.position, attack_activation_range);
    }
}
