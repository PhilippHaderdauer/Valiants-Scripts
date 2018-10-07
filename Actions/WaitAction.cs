using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitAction : BaseAction
{

    public override void Awake()
    {
        base.Awake();
    }

    public override void ActivateMenu(bool setTrue)
    {
        menuActive = setTrue;
    }

    //MenuControls
    public override void Menu()
    {
        if (Input.GetMouseButtonDown(0) && ActionUI.hitUIElements.Count == 0)
        {
            myPlayerCharacter.currentActionTime = 0.2f;
            //Add function as string
            myPlayerCharacter.actions.Add(this);
            //Gather parameters for function
            object[] parameters = new object[1];
            parameters[0] = myPlayerCharacter.currentActionTime;
            myPlayerCharacter.actionValues.Add(parameters);
            myPlayerCharacter.usedActionTime += myPlayerCharacter.currentActionTime;
            myPlayerCharacter.UpdateDummy();
        }
    }

    public override void Execution(object[] parameters)
    {
        myCharacter.myAnimator.Play("Idle");
        //End Action
        if (myCharacter.ActionTimer((float)parameters[0]))
        {
            DeleteAction();
            myCharacter.finishedAction = true;
        }
    }
}
