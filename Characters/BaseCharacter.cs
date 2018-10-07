using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//Base class for all characters: Dummy, NPCs and Player
public class BaseCharacter : MonoBehaviour {

    protected GameManager gameManager;

    //Audio
    protected AudioSource myAudioSource;

    //Animations
    public Animator myAnimator { get; private set; }
    public bool applyRootMotion;

    //Model
    public Transform myModel;
    protected Transform rightHand;

    //Health
    public float health;
    public float healthMax;

    //Skill
    public float skill;
    public float skillMax;
    public float skillBonus;
    public float skillMali;

    //Poise
    public int poise;
    public int poiseMax;

    //Hitbox
    [SerializeField]
    protected GameObject myHitBox;

    //AdjustBoundingBox
    //float highestPoint = 0;
    //float lowestPoint = 0;
    //public List<Transform> bodyParts { get; private set; } = new List<Transform>();

    //Velocity
    public Dictionary<string, Vector3> myVelocities { get; private set; } = new Dictionary<string, Vector3>();

    //HitReactions
    [SerializeField]
    protected bool gotHit;
    [SerializeField]
    protected float stunTime;
    protected bool startHitState = true;

    //Physics
    public bool grounded { get; private set; }
    public Rigidbody rb;

    //Actions
    public List<BaseAction> actions = new List<BaseAction>();
    public List<object[]> actionValues = new List<object[]>();
    protected int actionsIterator;
    public BaseAction currentAction;
    public object[] currentActionValue;
    protected bool allowCommands;
    private float timeCount;
    public bool startAction = false;
    [HideInInspector]
    public bool finishedAction = true;
    public float remainingCurrentActionTime;
    [SerializeField]
    private float passedCurrentActionTime;
    public GameObject myActionsContainer;
    public bool usedSwitchAction = false;

    public void Initialize()
    {
        //Basic Variables
        myModel = transform.Find("Model");
        applyRootMotion = false;
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        myAnimator = myModel.GetComponent<Animator>();
        grounded = true;
        rb = GetComponent<Rigidbody>();
        health = healthMax;
        myAudioSource = GetComponent<AudioSource>();
        //Setup Velocities for later use
        myVelocities.Add("GravityVelocity", Vector3.zero);
        myVelocities.Add("HitVelocity", Vector3.zero);
    }

    virtual public void Update()
    {
        //Clamp skill and health variable
        health = Mathf.Clamp(health, 0, healthMax);
        skill = Mathf.Clamp(skill, 0, skillMax);
        //Only update character when not kinematic
        if (rb.isKinematic)
        {
            myAnimator.speed = 0;
        }
        else
        {
            myAnimator.speed = 1;
            HitState();
            UpdateCharacter();
        }
    }

    private void FixedUpdate()
    {
        PhysicsCheck(transform.position);
    }

    virtual public void UpdateCharacter()
    {
        //If simulation is activated, do actions in order
        if (!rb.isKinematic)
        {
                if (actions.Count > actionsIterator)
                {
                    if (finishedAction)
                    {
                        currentAction = actions[0];
                        currentActionValue = actionValues[0];
                        finishedAction = false;
                        startAction = true;
                        remainingCurrentActionTime = 255;
                        if (currentAction.myCharacter != this)
                            currentAction.myCharacter = this;
                        currentAction.Execution(currentActionValue);
                    }
                else
                    {
                        currentAction.Execution(currentActionValue);
                    }
                }
                else if (actions.Count > 0)
                {
                    if (!finishedAction)
                    {
                        currentAction.Execution(currentActionValue);
                    }
                }
        }
    }

    //The HitState
    virtual public bool HitState()
    {            
        if (gotHit)
        {
            if (GetComponent<PlayerCharacter>())
            {
                myVelocities["MovementVelocity"] = Vector3.zero;
                myVelocities["JumpVelocity"] = Vector3.zero;
                stunTime -=  Time.deltaTime;
                if (stunTime <= 0)
                {
                    myVelocities["HitVelocity"] = Vector3.zero;
                    gotHit = false;
                }
                return false;
            }
            else
            {
                myVelocities["MovementVelocity"] = Vector3.zero;
                myVelocities["JumpVelocity"] = Vector3.zero;
                if (startHitState)
                {
                    startHitState = false;
                    myAnimator.Play("Normal Hit");
                }
                //Leave hitstate if stunTime is over
                stunTime -= Time.deltaTime;
                if (stunTime <= 0)
                {
                    myVelocities["HitVelocity"] = Vector3.zero;
                    gotHit = false;
                }
                return true;
            }
        }
        else
        {
            return false;
        }
    }

    public void ReceiveHit(float hitBoxRotationY, Vector3 knckBck, float stnTime, float dmg)
    {
        //If the character is blocking than check the rotation of the hitbox versus the character rotation. 
        if (GetComponent<PlayerCharacter>())
        {
            if (currentAction.GetType() == typeof(BlockAction))
            {
                //If this doesn't work, maybe try Vector3.Angle to see where the hitbox is in relation to the character
                //If the rotation difference is in this spectrum, the hit doesn't count because of the block
                if (Mathf.Abs(transform.rotation.eulerAngles.y - hitBoxRotationY) > 140)
                {
                    return;
                }
                else
                {
                    health -= dmg / 4;
                }
            }
                gotHit = true;
                health -= dmg;
                stunTime = stnTime;
                myVelocities["HitVelocity"] = knckBck;
        }
        else
        {
            gotHit = true;
            health -= dmg;
            stunTime = stnTime;
            myVelocities["HitVelocity"] = knckBck;
        }
    }

    //Find the highest and lowest point of the body and adjust bounding box accordingly
    /*public void AdjustBoundingBox()
    {
        if (!rb.isKinematic)
        {
            foreach (Transform tr in bodyParts)
            {
                if (bodyParts[0] == tr)
                {
                    highestPoint = tr.position.y;
                    lowestPoint = tr.position.y;
                }
                if (tr.position.y > highestPoint)
                    highestPoint = tr.position.y;

                if (tr.position.y < lowestPoint)
                    lowestPoint = tr.position.y;
            }
            GetComponent<CapsuleCollider>().center = new Vector3(0, (highestPoint + lowestPoint) / 2 - transform.position.y, 0);
            GetComponent<CapsuleCollider>().height = Mathf.Abs(highestPoint - lowestPoint);
        }
    }*/

    //Check if the character is grounded
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == 8)
            grounded = true;
    }

    virtual public void PhysicsCheck(Vector3 currentPosition)
    {
        //Apply gravity if necessary
        if (grounded)
            myVelocities["GravityVelocity"] = Vector3.zero;
        else
            myVelocities["GravityVelocity"] += Physics.gravity*Time.deltaTime;
        grounded = false;

        //Add all velocities in the dictionary together
        Vector3 myVelocity = Vector3.zero;
        foreach (KeyValuePair<string, Vector3> velocity in myVelocities)
        {
            myVelocity += velocity.Value;
        }

        rb.velocity = myVelocity;
    }

    //Make a hitbox for this character
    public void MakeHitBox(Vector3 scale, Vector3 position, float actvTime, float dlyTime, float dmg, float knckBckSpd, float stnTme)
    {
        if (gameManager.gameState == GameManager.gameStates.realTime)
        {
            Hitbox hitBox;
            hitBox = Instantiate(myHitBox, transform).GetComponent<Hitbox>();
            hitBox.transform.localScale = scale;
            hitBox.transform.localPosition = position;
            hitBox.activeTime = actvTime;
            hitBox.delayTime = dlyTime;
            hitBox.stunTime = stnTme;
            hitBox.damage = dmg;
            hitBox.knockBackSpeed = knckBckSpd;
        }
    }

    //TimeCounter for all actions
    public bool ActionTimer(float actionTime)
    {
        //Set time if it is not set, else count down
        if (remainingCurrentActionTime == 255)
        {
            remainingCurrentActionTime = Mathf.Abs(actionTime);
            //passedCurrentActionTime = 0;
        }
        remainingCurrentActionTime -= Time.deltaTime;
        passedCurrentActionTime += Time.deltaTime;
        //Return if time is over or not, be more forgiving so that end time is right
        if (remainingCurrentActionTime <= 0.0115)
            return true;
        else
            return false;
    }
}
