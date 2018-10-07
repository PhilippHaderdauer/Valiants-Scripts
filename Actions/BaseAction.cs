using UnityEngine;
using System.Collections.Generic;

public abstract class BaseAction : MonoBehaviour {

    public bool menuActive;

    public BaseCharacter myCharacter;
    protected PlayerCharacter myPlayerCharacter;

    //initialize the variables
    virtual public void Awake()
    {
        myCharacter = transform.parent.GetComponent<BaseCharacter>();
        if (transform.parent.GetComponent<PlayerCharacter>())
            myPlayerCharacter = transform.parent.GetComponent<PlayerCharacter>();
    }

    //Controls only the menu
    public void Update()
    {
        if (myPlayerCharacter != null)
            if (myPlayerCharacter.dummyCharControls.gameObject.activeSelf)
            {
                if (myPlayerCharacter.dummyCharControls.rb.isKinematic)
                    if (menuActive)
                        Menu();
            }
            else
            {
                if (!myPlayerCharacter.usedSwitchAction)
                    if (menuActive)
                    Menu();
            }
    }

    //Remove the action from the character after it has been activated
    protected void DeleteAction()
    {
        myCharacter.actions.Remove(this);
        myCharacter.actionValues.RemoveAt(0);
    }

    //Activation of the menu through other classes
    public abstract void ActivateMenu(bool setTrue);

    //The menu for this action
    public abstract void Menu();

    //The actual execution of the action
    public abstract void Execution(object[] parameters);
}