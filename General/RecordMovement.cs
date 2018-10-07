using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordMovement : MonoBehaviour {

    public List<object> recordedProperties = new List<object>();
    private BaseCharacter myCharacter;
    public int timeStepOffset;
    private Animator myAnimator;
    [SerializeField]
    private int timeSteps;


    public void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        if (GetComponent<BaseCharacter>()) {
            myCharacter = GetComponent<BaseCharacter>();
            myAnimator = myCharacter.myModel.GetComponent<Animator>();
        }
        else
        {
            //Add other objects as well. With offset if they have been spawned at a later state (e.g. bullets)
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().otherObjects.Add(GetComponent<Rigidbody>());
            timeStepOffset = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().timeStepsMax;
        }
    }

    public void RecordMovementFunction ()
    {
        if (myCharacter) {
            object[] parameters = new object[5];
            parameters[0] = transform.position;
            parameters[1] = transform.rotation;
            if (gameObject.activeSelf) {
                parameters[2] = myAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash;
                parameters[3] = myAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            }
            parameters[4] = gameObject.activeSelf;
            recordedProperties.Add(parameters);
            timeSteps++;
        }
        else
        {
            object[] parameters = new object[3];
            parameters[0] = transform.position;
            parameters[1] = transform.rotation;
            parameters[2] = gameObject.activeSelf;
            recordedProperties.Add(parameters);
        }
    }

    public void PlayMovementFunction(int i)
    {
        if (i - timeStepOffset >= 0)
        {
            i -= timeStepOffset;
        }
        else
        {
            i = 0;
        }
        object[] tempObject = (object[]) recordedProperties[i];
        transform.position = (Vector3)tempObject[0];
        transform.rotation = (Quaternion)tempObject[1];
        if (myCharacter)
        {
            gameObject.SetActive((bool)tempObject[4]);
            if (gameObject.activeSelf)
            {
                myAnimator.Play((int)tempObject[2], 0, (float)tempObject[3]);
            }
        }
        else
        {
            gameObject.SetActive((bool)tempObject[2]);
        }
        if (i == 0)
        {
            gameObject.SetActive(false);
        }
    }
   
    public void RecordMovementFunctionDummy()
    {
        object[] parameters = new object[6];
        parameters[0] = transform.position;
        parameters[1] = transform.rotation;
        parameters[3] = myCharacter.remainingCurrentActionTime;
        parameters[4] = myCharacter.finishedAction;
        parameters[5] = myCharacter.startAction;
        recordedProperties.Add(parameters);
    }   
}
