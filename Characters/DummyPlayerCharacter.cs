using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyPlayerCharacter : BaseCharacter {

    //MyCreator
    public GameObject myCreator;

    // Use this for initialization
    private void Awake () {
        Initialize();
    }
	
	// Update is called once per frame
	new private void Update () {
        /*
         * //Delete command
        rightClick = Input.GetMouseButtonDown(1);
        if (rightClick && myCreator.GetComponent<PlayerCharacter>().isSelected && rb.isKinematic)
        {
            if (Commands.Count > 0)
            {
                GetComponent<RecordMovement>().PlayMovementFunctionDummy(0, true);
                gameManager.ActivateManualDummySimulationFunction(0, gameObject);
                rightClick = false;
                object[] lastActionTime = (object[])CommandParameters[Commands.Count - 1];
                if (Commands.Count == 1)
                    myCreator.GetComponent<PlayerCharacter>().RemainingActionTime = 4;
                else
                    myCreator.GetComponent<PlayerCharacter>().RemainingActionTime += Mathf.Abs((float)lastActionTime[0]);
                remainingActionTime += Mathf.Abs((float)lastActionTime[0]);
                Commands.RemoveAt(Commands.Count - 1);
                CommandParameters.RemoveAt(CommandParameters.Count - 1);
                myCreator.GetComponent<PlayerCharacter>().Commands.RemoveAt(Commands.Count);
                myCreator.GetComponent<PlayerCharacter>().CommandParameters.RemoveAt(CommandParameters.Count);
                if (Commands.Count == 0)
                    rb.isKinematic = true;
            }
        }*/
        base.Update();
        //If you have no more actions, make yourself kinematic
        if (actions.Count == 0 && finishedAction)
        {
            rb.isKinematic = true;
        }
    }
}
