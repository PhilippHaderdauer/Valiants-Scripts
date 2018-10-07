using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WalkRunAction : BaseAction
{
    private Vector3 targetPosition;
    private Vector3 myCurrentPosition;
    private List<Vector3> pathList = new List<Vector3>();
    [SerializeField]
    protected float movementSpeed;
    protected List<Vector3> actionPathList = new List<Vector3>();
    protected int actionPathListCount;

    public override void Awake()
    {
        base.Awake();
        myCharacter.myVelocities.Add("MoveVelocity", Vector3.zero);
    }

    public override void ActivateMenu(bool setTrue)
    {
        menuActive = setTrue;
        myPlayerCharacter.lnRndrr.enabled = false;
    }

    //MenuControls
    public override void Menu()
    {
        //Change position
        ActionUI.instance.myCursor.transform.position = ActionUI.instance.mousePosition;

        //Calculate navmesh path and draw it with linerenderer
        NavMeshPath path = new NavMeshPath();
        float distance = 0;
        pathList.Clear();
        NavMesh.CalculatePath(ActionUI.instance.myCursor.transform.position, myPlayerCharacter.commandPosition, NavMesh.AllAreas, path);
        for (int i = path.corners.Length - 1; i >= 0; i--)
        {
            if (i == path.corners.Length - 1)
            {
                distance += Vector3.Distance(myPlayerCharacter.commandPosition, path.corners[i]);
            }
            else
            {
                distance += Vector3.Distance(path.corners[i + 1], path.corners[i]);
            }
            pathList.Add(path.corners[i]);
        }
        myPlayerCharacter.lnRndrr.positionCount = pathList.Count;
        myPlayerCharacter.lnRndrr.SetPositions(pathList.ToArray());

        myPlayerCharacter.currentActionTime = (distance / movementSpeed);
        myPlayerCharacter.currentActionTime = (float)System.Math.Round(myPlayerCharacter.currentActionTime, 2);

        List<Vector3> tempPathList = new List<Vector3>();
        //Make deep copy
        foreach (Vector3 pos in pathList)
        {
            tempPathList.Add(new Vector3(pos.x, pos.y, pos.z));
        }

        myPlayerCharacter.lnRndrr.enabled = false;
        //If cursor is on the WalkRunElement, record action
        if (ActionUI.hitUIElements.Count == 0 && path.status == NavMeshPathStatus.PathComplete)
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
                object[] parameters = new object[3];
                parameters[0] = myPlayerCharacter.currentActionTime;
                parameters[1] = tempPathList;
                myPlayerCharacter.actionValues.Add(parameters);
                myPlayerCharacter.usedActionTime += myPlayerCharacter.currentActionTime;
                myPlayerCharacter.UpdateDummy();
            }
        }
        else
        {
            myPlayerCharacter.currentActionTime = 0f;
        }
    }

    public override void Execution(object[] parameters)
    {
        //Is the current character the playercharacter or the dummy?
        if (myPlayerCharacter)
        {
            myCurrentPosition = myPlayerCharacter.commandPosition;
        }
        else
        {

            myCurrentPosition = myCharacter.transform.position;
        }

            //Start Action / Do Action
            if (myCharacter.startAction)
            {
                actionPathListCount = 0;
                List<Vector3> tempPathList = (List<Vector3>)parameters[1];
                actionPathList.Clear();
                foreach (Vector3 pos in tempPathList)
                {
                    actionPathList.Add(new Vector3(pos.x, pos.y, pos.z));
                }
                actionPathList.RemoveAt(0);
                myCharacter.myVelocities["MoveVelocity"] = (actionPathList[actionPathListCount] - myCurrentPosition).normalized * movementSpeed;
                myCharacter.startAction = false;
            }

            myCharacter.myAnimator.Play("Walk");
            myCharacter.myVelocities["MoveVelocity"] = (actionPathList[actionPathListCount] - myCurrentPosition).normalized * movementSpeed;

            //Change waypoint
            if (actionPathListCount < actionPathList.Count - 1)
                if (Vector3.Distance(actionPathList[actionPathListCount], myCurrentPosition) < 0.1f)
                {
                    actionPathListCount += 1;
                    myCharacter.myVelocities["MoveVelocity"] = (actionPathList[actionPathListCount] - myCurrentPosition).normalized * movementSpeed;
                }

            //Rotate character if there is velocity in a x,z-direction
            if (new Vector3(myCharacter.myVelocities["MoveVelocity"].x, 0, myCharacter.myVelocities["MoveVelocity"].z) != Vector3.zero)
                myCharacter.rb.MoveRotation(Quaternion.LookRotation(new Vector3(myCharacter.myVelocities["MoveVelocity"].x, 0, myCharacter.myVelocities["MoveVelocity"].z), transform.up));

            //End Action
            if (myCharacter.ActionTimer((float)parameters[0]))
            {
                DeleteAction();
                myCharacter.finishedAction = true;
                myCharacter.myVelocities["MoveVelocity"] = Vector3.zero;
            }
    }
}

