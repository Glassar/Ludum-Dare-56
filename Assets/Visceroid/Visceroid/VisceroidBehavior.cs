using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisceroidBehavior : MonoBehaviour
{
    bool dead;
    private bool dying;
    public int health;
    private Animator animator;
    private bool isAnimating;
    public bool attack;
    private bool attacking;
    public int attack_int;
    public float attack_activation_range;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        attacking = false;
        dead = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(health<1){
            dead = true;
        }
        

        if(!dead){

            if(attack){
                if(!attacking){
                    Debug.Log("Attacking");
                    animator.SetTrigger("Attack");
                    attacking = true;
                }else{
                    if(!animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")){
                        // Attack animation done
                        attacking = false;
                        attack = false;
                        // Insert code here to kill player
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
}
