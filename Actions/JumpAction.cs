using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpAction : BaseAction {

    //Befehl 2: Jump
    private Vector3 differenceVector;
    private float jumpMaxDistance;
    private List<Vector3> jumpArcList = new List<Vector3>();
    private float jumpRadianAngle;
    private bool startedJump;

    //Jump
    protected float gravity;
    [SerializeField]
    private float jumpVelocity = 6.5f;

    public override void Awake()
    {
        base.Awake();
        gravity = Mathf.Abs(Physics.gravity.y);
        myCharacter.myVelocities.Add("JumpVelocity", Vector3.zero);
    }

    public override void ActivateMenu(bool setTrue)
    {
        menuActive = setTrue;
        myPlayerCharacter.lnRndrr.enabled = false;
    }

    //MenuControls
    public override void Menu()
    {
        //Cursor offset
        ActionUI.instance.myCursor.transform.position = ActionUI.instance.mousePosition + new Vector3(0, 1, 0);

        //Ressourcen
        //Board To Bits Tutorial
        //Rendering a Launch Arc in Unity
        //https://github.com/IronWarrior/ProjectileShooting/tree/master
        //Khan Academy: Projectile motion, different elevations

        differenceVector = (ActionUI.instance.myCursor.transform.position - myPlayerCharacter.commandPosition).normalized;

        float xzDistance = Vector3.Distance(new Vector3(ActionUI.instance.myCursor.transform.position.x, 0, ActionUI.instance.myCursor.transform.position.z), new Vector3(myPlayerCharacter.commandPosition.x, 0, myPlayerCharacter.commandPosition.z));
        float yDistance = Mathf.Abs(ActionUI.instance.myCursor.transform.position.y - myPlayerCharacter.commandPosition.y);

        //Manipulate position and not angle
        //jumpVelocity = (xyDistance * Mathf.Sqrt(gravity) * Mathf.Sqrt(1 / Mathf.Cos(jumpRadianAngle))) / Mathf.Sqrt(2 * xyDistance * Mathf.Sin(jumpRadianAngle) + 2 * yDistance * Mathf.Cos(jumpRadianAngle));

        float angle0, angle1;

        float operandA = Mathf.Pow(jumpVelocity, 4);
        float operandB = gravity * (gravity * (xzDistance * xzDistance) + (2 * yDistance * jumpVelocity * jumpVelocity));

        if (operandB > operandA)
            return;

        float root = Mathf.Sqrt(operandA - operandB);

        angle0 = Mathf.Atan((jumpVelocity * jumpVelocity + root) / (gravity * xzDistance));
        angle1 = Mathf.Atan((jumpVelocity * jumpVelocity - root) / (gravity * xzDistance));

        jumpRadianAngle = Mathf.Min(angle0, angle1);

        jumpArcList.Clear();

        LayerMask tempLayer = gameObject.layer;
        gameObject.layer = 2;
        LayerMask collisionMask = 1 << 8;

        for (int i = 0; i <= 2000; i++)
        {
            float x = i * 0.0133333f;
            float y = x * Mathf.Tan(jumpRadianAngle) - ((gravity * x * x) / (2 * jumpVelocity * jumpVelocity * Mathf.Cos(jumpRadianAngle) * Mathf.Cos(jumpRadianAngle)));
            Vector3 nextPosition = new Vector3(myPlayerCharacter.commandPosition.x + x * differenceVector.x, myPlayerCharacter.commandPosition.y + y, myPlayerCharacter.commandPosition.z + x * differenceVector.z);
            jumpArcList.Add(nextPosition);
            if (i > 10)
                if ((Physics.CheckSphere(nextPosition, 0.0266666f, collisionMask)))
                {
                    break;
                }
        }
        float yDisplacement = Mathf.Abs(jumpArcList[jumpArcList.Count - 1].y - myPlayerCharacter.commandPosition.y);
        //if (!jumpHalf)
            myPlayerCharacter.currentActionTime = 1.666666f * (jumpVelocity * Mathf.Sin(jumpRadianAngle) + Mathf.Sqrt(jumpVelocity * Mathf.Sin(jumpRadianAngle) + 4 * gravity / 2 * yDisplacement)) / gravity;
        //else
            //myPlayerCharacter.currentActionTime = 0.5f * 1.666666f * (jumpVelocity * Mathf.Sin(jumpRadianAngle) + Mathf.Sqrt(jumpVelocity * Mathf.Sin(jumpRadianAngle) + 4 * gravity / 2 * yDisplacement)) / gravity;
        myPlayerCharacter.currentActionTime += 0.6f;
        gameObject.layer = tempLayer;
        //if (jumpHalf)
        //{
            //jumpArcList.RemoveRange(jumpArcList.Count / 2 - 1, jumpArcList.Count / 2);
        //}
        jumpArcList.RemoveAt(jumpArcList.Count - 1);
        jumpArcList.RemoveAt(jumpArcList.Count - 1);
        myPlayerCharacter.lnRndrr.positionCount = jumpArcList.Count;
        myPlayerCharacter.lnRndrr.SetPositions(jumpArcList.ToArray());
        myPlayerCharacter.lnRndrr.enabled = false;

        //If cursor is on the JumpElement, record action
        if (ActionUI.hitUIElements.Count == 0)
        {
            myPlayerCharacter.skillMali = -(int)(myPlayerCharacter.currentActionTime * 20);
            myPlayerCharacter.lnRndrr.enabled = true;
            if (Input.GetMouseButtonDown(0) && myPlayerCharacter.skill > (int)(myPlayerCharacter.currentActionTime * 20))
            {
                myPlayerCharacter.skillMali = 0;
                myPlayerCharacter.skill -= (int)(myPlayerCharacter.currentActionTime * 20);
                //Add function as string
                myPlayerCharacter.actions.Add(this);
                //Gather parameters for function
                object[] parameters = new object[4];
                parameters[0] = myPlayerCharacter.currentActionTime;
                parameters[1] = jumpRadianAngle;
                parameters[2] = differenceVector;
                parameters[3] = jumpVelocity;
                //parameters[4] = jumpHalf;
                myPlayerCharacter.actionValues.Add(parameters);
                myPlayerCharacter.usedActionTime += myPlayerCharacter.currentActionTime;
                myPlayerCharacter.UpdateDummy();
            }
        }
        else
        {
            myPlayerCharacter.currentActionTime = 0;
        }
    }

    public override void Execution(object[] parameters)
        {
        //Start Action / Do Action
        if (myCharacter.startAction)
        {
            myCharacter.myAnimator.Play("Idle");
            startedJump = false;
            myCharacter.startAction = false;
            float angle = (float)parameters[1];
            Vector3 directionVector = (Vector3)parameters[2];
            myCharacter.myVelocities["JumpVelocity"] = new Vector3(Mathf.Cos(angle) * directionVector.x, Mathf.Sin(angle), Mathf.Cos(angle) * directionVector.z) * (float)parameters[3];
            myCharacter.rb.MoveRotation(Quaternion.LookRotation(new Vector3(myCharacter.myVelocities["JumpVelocity"].x, 0, myCharacter.myVelocities["JumpVelocity"].z), transform.up));
            myCharacter.myVelocities["JumpVelocity"] = Vector3.zero;
        }

        myCharacter.myAnimator.Play("Jump");

        //End Action
        if (myCharacter.ActionTimer((float)parameters[0]))
        {
            DeleteAction();
            myCharacter.finishedAction = true;
            myCharacter.myVelocities["JumpVelocity"] = Vector3.zero;
        }

        if ((Mathf.Abs(myCharacter.remainingCurrentActionTime - (float)parameters[0]) > 0.65f) && myCharacter.grounded && startedJump)
        {
            myCharacter.myVelocities["JumpVelocity"] = Vector3.zero;
        }

        //Start Jump with delay
        if ((Mathf.Abs(myCharacter.remainingCurrentActionTime - (float)parameters[0]) >= 0.6f) && myCharacter.grounded && !startedJump)
        {
            startedJump = true;
            float angle = (float)parameters[1];
            Vector3 directionVector = (Vector3)parameters[2];
            myCharacter.myVelocities["JumpVelocity"] = new Vector3(Mathf.Cos(angle) * directionVector.x, Mathf.Sin(angle), Mathf.Cos(angle) * directionVector.z) * (float)parameters[3];
        }
    }
}
