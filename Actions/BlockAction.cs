using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BlockAction : BaseAction
{
    public AnimationClip myAnimClip;

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

        myPlayerCharacter.currentActionTime = myAnimClip.length;

        Quaternion tempRotation = Quaternion.LookRotation(direction, transform.up);

        myPlayerCharacter.ActivatePreviewModel(tempRotation, myAnimClip);
        myPlayerCharacter.previewModel.transform.position = myPlayerCharacter.commandPosition;

        myPlayerCharacter.skillMali = -20;
        //If cursor is on the PunchElement, record action
        if (Input.GetMouseButtonDown(0) && ActionUI.hitUIElements.Count == 0 && myPlayerCharacter.skill > 20)
        {
            myPlayerCharacter.skill += myCharacter.skillMali;
            myPlayerCharacter.skillMali = 0;
            //Add function as string
            myPlayerCharacter.actions.Add(this);
            //Gather parameters for function
            object[] parameters = new object[4];
            parameters[0] = myPlayerCharacter.currentActionTime;
            parameters[1] = "Block";
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
            myCharacter.startAction = false;
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
        }
    }
}

