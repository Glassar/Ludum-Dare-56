using Rellac.Audio;
using UnityEngine;
using UnityEngine.AI;

public class CrawlerController : Enemy
{
    public NavMeshAgent agent;
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private Animator animator;
    private bool isAnimating = false;
    private bool dead = false;

    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;

    public float health;

    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;
    public float patrolSpeed;
    public float chaseSpeed;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public float attackDamage;

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private void Start()
    {
        player = PlayerController.instance.transform;
        agent = GetComponent<NavMeshAgent>();
    }

    public override void TakeDamage(float dmg) {
        health -= dmg;
        soundManager.PlayOneShotRandomPitch("crawlerDamage", 0.05f);

        if (health <= 0) Invoke(nameof(DestroyEnemy), 0.5f);
    }

    private void Update()
    {
        if (!dead){
            //Check for sight and attack range
            playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

            if (!playerInSightRange && !playerInAttackRange) Patroling();
            if (playerInSightRange && !playerInAttackRange) ChasePlayer();
            if (playerInAttackRange && playerInSightRange) AttackPlayer();
        }

        UpdateAnimations();
    }

    private void UpdateAnimations(){
        if (!dead) {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1) {   
                animator.SetTrigger("Walk");
            }
        }
        else {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1) {   
                animator.SetTrigger("DeadIdle");
            }
        }
    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }
    private void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        if (dead) return;
        //Make sure enemy doesn't move
        agent.SetDestination(transform.position);

        //transform.LookAt(player);

        if (!alreadyAttacked)
        {
            ///Attack code here
            Debug.Log("Attacking");
            animator.SetTrigger("Attack");
            soundManager.PlayOneShotRandomPitch("crawlerAttack", 0.05f);
            PlayerController.instance.TakeDamage(attackDamage);
            ///End of attack code

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }
    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    private void DestroyEnemy()
    {
        soundManager.PlayOneShotRandomPitch("crawlerDeath", 0.05f);
        transform.GetComponent<Collider>().enabled = false;
        agent.enabled = false;
        dead = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
