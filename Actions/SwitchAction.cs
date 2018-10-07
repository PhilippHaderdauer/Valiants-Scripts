using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchAction : BaseAction
{
    public int switchNumber;

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
        myPlayerCharacter.skillMali = -20;
        if (Input.GetMouseButtonDown(0) && ActionUI.hitUIElements.Count == 0 && myPlayerCharacter.skill > 20)
        {
            myPlayerCharacter.skillMali = 0;
            myPlayerCharacter.skill -= 20;
            myPlayerCharacter.currentActionTime = 0.2f;
            //Add function as string
            myPlayerCharacter.actions.Add(this);
            //Gather parameters for function
            object[] parameters = new object[1];
            parameters[0] = myPlayerCharacter.currentActionTime;
            myPlayerCharacter.actionValues.Add(parameters);
            myPlayerCharacter.usedActionTime += myPlayerCharacter.currentActionTime;
            myPlayerCharacter.UpdateDummy();
            menuActive = false;
        }
    }

    public override void Execution(object[] parameters)
    {
        myCharacter.myAnimator.Play("Idle");
        if (myCharacter.GetComponent<DummyPlayerCharacter>())
        {
            myCharacter.GetComponent<DummyPlayerCharacter>().myCreator.GetComponent<PlayerCharacter>().usedSwitchAction = true;
        }
        //End Action
        if (myCharacter.ActionTimer((float)parameters[0]))
        {
            if (myCharacter.GetComponent<PlayerCharacter>())
            {
                myPlayerCharacter.setCharacterSelected(false);
                myPlayerCharacter.gameObject.SetActive(false);
                myPlayerCharacter.dummyCharControls.actions.Clear();
                myPlayerCharacter.dummyCharControls.currentAction = null;
                GameManager.instance.MakeCharacterActive(GameManager.instance.characters[switchNumber]);
                GameManager.instance.selectedCharacter.transform.position = transform.position;
                GameManager.instance.selectedCharacter.gameObject.SetActive(true);
                myCharacter.transform.position = new Vector3(0, -1000, 0);
                myCharacter.usedSwitchAction = false;
            }
            DeleteAction();
            myCharacter.finishedAction = true;
        }
    }
}
