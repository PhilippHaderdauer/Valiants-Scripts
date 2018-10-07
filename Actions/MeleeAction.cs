using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeleeAction : BaseAction
{
    //Punching
    public float animationLength;
    public bool startAnimation;
    protected string attackName;
    public AnimationClip[] myAnimClips = new AnimationClip[4];
    [HideInInspector]
    public int meleeActionNumber = 0;

    public override void Awake()
    {
        base.Awake();
    }

    public override void ActivateMenu(bool setTrue)
    {
        menuActive = setTrue;
        myPlayerCharacter.previewModel.SetActive(false);
    }

    //MenuControls
    public override void Menu()
    {
        //Change position
        ActionUI.instance.myCursor.transform.position = ActionUI.instance.mousePosition;
        Vector3 direction = (ActionUI.instance.myCursor.transform.position - myPlayerCharacter.commandPosition).normalized;

        myPlayerCharacter.currentActionTime = myAnimClips[meleeActionNumber].length;

        Quaternion tempRotation = Quaternion.LookRotation(direction, transform.up);

        MeleeProperties.instance.GetProperties(myAnimClips[meleeActionNumber].name, myCharacter, true);

        if (startAnimation == false)
        {
            startAnimation = true;
            myPlayerCharacter.ActivatePreviewModel(tempRotation, myAnimClips[meleeActionNumber]);
            animationLength = myAnimClips[meleeActionNumber].length;
        }
        else
        {
            if (animationLength > 0)
            {
                animationLength -= Time.deltaTime;
            }
            if (animationLength <= 0)
            {
                myPlayerCharacter.previewModel.transform.position = myPlayerCharacter.commandPosition;
                myPlayerCharacter.previewModel.GetComponent<Animator>().Play("Idle");
                startAnimation = false;
            }
        }

        //If cursor is on the PunchElement, record action
        if (Input.GetMouseButtonDown(0) && ActionUI.hitUIElements.Count == 0 && myPlayerCharacter.skill > Mathf.Abs(myCharacter.skillMali))
        {
            myPlayerCharacter.skill += myCharacter.skillMali;
            myPlayerCharacter.skillMali = 0;
            //Add function as string
            myPlayerCharacter.actions.Add(this);
            //Gather parameters for function
            object[] parameters = new object[4];
            parameters[0] = myPlayerCharacter.currentActionTime;
            parameters[1] = myAnimClips[meleeActionNumber].name;
            parameters[2] = tempRotation;
            parameters[3] = ActionUI.instance.myCursor.transform.position;
            myPlayerCharacter.actionValues.Add(parameters);
            myPlayerCharacter.usedActionTime += myPlayerCharacter.currentActionTime;
            myPlayerCharacter.previewModel.SetActive(false);
            myPlayerCharacter.UpdateDummy();
        }
    }

    public override void Execution(object[] parameters)
    {
        //Start Action
        if (myCharacter.startAction)
        {
            myCharacter.rb.MoveRotation((Quaternion)parameters[2]);
            myCharacter.rb.transform.rotation = (Quaternion)parameters[2];
            MeleeProperties.instance.GetProperties((string)parameters[1], myCharacter,false);
            myCharacter.startAction = false;
            myCharacter.applyRootMotion = true;
        }

        myCharacter.myVelocities["GravityVelocity"] = Vector3.zero;
        myCharacter.myAnimator.Play((string)parameters[1]);

        //End Action
        if (myCharacter.ActionTimer((float)parameters[0]))
        {
            DeleteAction();
            myCharacter.finishedAction = true;
            myCharacter.myModel.transform.localPosition = Vector3.zero;
            transform.rotation = Quaternion.Euler(new Vector3(0, transform.eulerAngles.y, 0));
            myCharacter.myVelocities["RootMotionVelocity"] = Vector3.zero;
            myCharacter.myAnimator.Play("Idle");
            myCharacter.applyRootMotion = false;
        }
    }
}

