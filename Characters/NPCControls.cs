using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class NPCControls : BaseCharacter {

    //UI
    [SerializeField]
    private Slider healthSlider;
    [SerializeField]
    private Text healthText;

    private states myState;

    //Walking State
    public GameObject target;
    [SerializeField]
    private float movementSpeed = 1;
    private NavMeshAgent navMeshAgent;

    //Melee Attack
    private bool attackStarted;
    private float attackTimer;
    private int attackCounter = 0;
    private float currentAttackLength;

    //Ranged Attack State
    private bool rangedAttackStarted;
    private float rangedAttackTimer;
    [SerializeField]
    private bool isRanged;
    [SerializeField]
    private float rotateSpeed;
    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private float bulletSpeed;
    [SerializeField]
    private float bulletSize;
    [SerializeField]
    private float bulletDamageModifier;

    //Melee Attacking State; Minimum size is 2
    [SerializeField]
    private AnimationClip[] myAnimClips = new AnimationClip[2];

    private enum states
    {
        walking,
        attackingMelee,
        attackingRange,
    }

    // Use this for initialization
    private void Awake () {
        Initialize();
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updatePosition = false;
        navMeshAgent.updateRotation = false;
        myState = states.walking;
    }

    // Update is called once per frame
    public new void Update () {
        //Set UI
        healthSlider.value = health / healthMax * 100;
        healthText.text = health + " / " + healthMax;

        //Dying
        if (health <= 0)
        {
            gameObject.SetActive(false);
        }

        //Set Target to player
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player");
        }

        //Only do stuff when not kinematic
        if (rb.isKinematic)
        {
            myAnimator.speed = 0;
        }
        else
        {
            myAnimator.speed = 1;
            if (!HitState())
            {
                if (myState == states.walking)
                {
                    applyRootMotion = false;
                    if (target != null)
                    {
                        navMeshAgent.SetDestination(target.transform.position);
                    }
                    myAnimator.Play("Walk");
                    if (!isRanged)
                    {
                        if (Vector3.Distance(transform.position, target.transform.position) < 1)
                        {
                            myState = states.attackingMelee;
                        }
                    }
                    else
                    {
                        if (Vector3.Distance(transform.position, target.transform.position) < 10)
                        {
                            myState = states.attackingRange;
                        }
                    }
                }
                if (myState == states.attackingMelee)
                {
                    if (!attackStarted)
                    {
                        attackTimer = myAnimClips[attackCounter].length;
                        currentAttackLength = myAnimClips[attackCounter].length;
                        applyRootMotion = true;
                        myAnimator.Play(myAnimClips[attackCounter].name);
                        MeleeProperties.instance.GetProperties(myAnimClips[attackCounter].name, GetComponent<BaseCharacter>(),false);
                        attackStarted = true;
                        if (attackCounter < myAnimClips.Length-1)
                            attackCounter += 1;
                        else
                            attackCounter = 0;
                    }
                    else
                    {
                        
                        attackTimer -= Time.deltaTime;
                        if (attackTimer<=0)
                        {
                            attackStarted = false;
                            if (!(Vector3.Distance(transform.position, target.transform.position) < 1))
                            {
                                myState = states.walking;
                            }
                        }
                    }
                }
                if (myState == states.attackingRange)
                {
                    if (!rangedAttackStarted)
                    {
                        if (Physics.Raycast(transform.position+Vector3.up,transform.forward,1000,1 << 11))
                        {

                            Bullet bullet = Instantiate(bulletPrefab,transform.position+Vector3.up+transform.forward,Quaternion.identity).GetComponent<Bullet>();
                            bullet.transform.localScale = bullet.transform.localScale*bulletSize;
                            bullet.myVelocity = transform.forward * bulletSpeed;
                            rangedAttackStarted = true;
                            rangedAttackTimer = 2;
                            myAnimator.Play("PistolShooting");
                        }
                    }
                    else
                    {
                        rangedAttackTimer -= Time.deltaTime;
                        if (rangedAttackTimer <= 0)
                        {
                            rangedAttackStarted = false;
                            myAnimator.Play("Idle");
                        }
                    }
                }
            }
            else
            {
                applyRootMotion = false;
            }
        }
    }

    private void FixedUpdate()
    {
        if (!rb.isKinematic)
        {
            if (target != null && myState == states.walking)
            {
                navMeshAgent.nextPosition = transform.position;
                navMeshAgent.destination = target.transform.position;
                myVelocities["MoveVelocity"] = navMeshAgent.desiredVelocity.normalized * movementSpeed;
                if (new Vector3(myVelocities["MoveVelocity"].x, 0, myVelocities["MoveVelocity"].z) != Vector3.zero)
                    rb.MoveRotation(Quaternion.LookRotation(new Vector3(myVelocities["MoveVelocity"].x, 0, myVelocities["MoveVelocity"].z), transform.up));
            }
            if (target != null && myState == states.attackingMelee)
            {
                myVelocities["MoveVelocity"] = Vector3.zero;
                if (attackTimer > currentAttackLength / 4)
                    rb.MoveRotation(Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, target.transform.position - transform.position, rotateSpeed * Time.deltaTime, 0), transform.up));
            }
            if (target != null && myState == states.attackingRange)
            {
                myVelocities["MoveVelocity"] = Vector3.zero;
                rb.MoveRotation(Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, target.transform.position - transform.position, rotateSpeed * Time.deltaTime, 0), transform.up));
            }
            PhysicsCheck(transform.position);
        }
    }
}
