using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Currently not in use

public class CharacterMotionAndIK : MonoBehaviour
{
    public bool isPreviewModel = false;
    private BaseCharacter myBaseCharacter;
    private Animator myAnimator;
    [SerializeField]
    private float lastTime = 255;

    void Start()
    {
        myBaseCharacter = transform.root.GetComponent<BaseCharacter>();
        myAnimator = myBaseCharacter.myAnimator;
    }

    void OnAnimatorMove()
    {
        if ((isPreviewModel) || (myBaseCharacter.applyRootMotion))
        {
                if (isPreviewModel)
                {
                    GetComponent<Rigidbody>().MovePosition(transform.position += new Vector3(-GetComponent<Animator>().deltaPosition.x,0, -GetComponent<Animator>().deltaPosition.z) / (lastTime - Time.time) / 50);
                    GetComponent<Rigidbody>().MoveRotation(transform.rotation *= Quaternion.Euler((GetComponent<Animator>().deltaRotation.eulerAngles / (lastTime - Time.time) / 30000)));
                }
                else
                {
                    myBaseCharacter.rb.MovePosition(myBaseCharacter.transform.position += new Vector3(-GetComponent<Animator>().deltaPosition.x, 0, -GetComponent<Animator>().deltaPosition.z) / (lastTime - Time.time) / 50);
                    myBaseCharacter.rb.MoveRotation(myBaseCharacter.transform.rotation *= Quaternion.Euler((myAnimator.deltaRotation.eulerAngles / (lastTime - Time.time) / 30000)));
                }
                lastTime = Time.time;
        }
    }
}
