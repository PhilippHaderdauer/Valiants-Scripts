using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Cameras;
using UnityEngine.AI;
using System.Linq;
using UnityEngine.SceneManagement;

public class PlayerCharacter : BaseCharacter
{
    //UI
    public Transform charGroundPlane;

    //Track time for actions
    public float currentActionTime;
    public float usedActionTime;

    //Input 
    protected bool rightClick;

    //Menu
    public bool isSelected = false;
    public LineRenderer lnRndrr { get; private set; }

    //Preview of BaseCharacter
    private GameObject dummy;
    public GameObject dummyPrefab;
    public DummyPlayerCharacter dummyCharControls { get; private set; }
    private bool enableSimulationManually;
    public Vector3 commandPosition { get; private set; }
    protected Quaternion commandRotation;

    //PreviewModel for Brawl Actions
    public GameObject previewModel;

    //Dying
    private float dyingTimer = 255;

    // Use this for initialization
    private void Awake()
    {
            Initialize();
            myActionsContainer = transform.Find("MyActionsContainer").gameObject;
            SpawnDummy();
            gameManager.AddCharacter(this);
            lnRndrr = GetComponent<LineRenderer>();
            charGroundPlane = transform.Find("charGroundPlane");
            SkinnedMeshRenderer[] skinnedMeshRenderer = previewModel.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (SkinnedMeshRenderer renderer in skinnedMeshRenderer)
            {
                renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, 0.125f);
            }
    }

    //Update the dummy character when receiving menu inputs
    public void UpdateDummy()
    {
        if (actions.Count > 0 && rb.isKinematic == true && gameManager.gameState == GameManager.gameStates.commandTime && gameManager.setupCommandTime)
        {
            if (actions.Count > dummyCharControls.actions.Count)
            {
                if (actions.Count == 1)
                {
                    dummy.transform.position = transform.position;
                    dummy.transform.rotation = transform.rotation;
                }
                dummy.SetActive(true);
                dummyCharControls.actions.Clear();
                dummyCharControls.actions.Add(actions[actions.Count-1]);
                dummyCharControls.actionValues.Clear();
                dummyCharControls.actionValues.Add(actionValues[actionValues.Count - 1]);
                dummyCharControls.currentAction = currentAction;
                dummyCharControls.currentActionValue = currentActionValue;
                gameManager.ChangeCameraTarget(dummy.transform);
                dummyCharControls.rb.isKinematic = false;
            }
        }
    }

    private new void Update()
    {
        //Input
        rightClick = Input.GetMouseButtonDown(1);

        //If self is moving, set dummy inactive
        if (!rb.isKinematic)
        {
            dummy.SetActive(false);
        }
        else
        {
            //Calculate skill bonus
            if (usedActionTime > 2)
                skillBonus = (float)Math.Round((usedActionTime - 2) * 40, 0);
        }

        //Update reference position for UI etc. As long as dummy is active, use his position
        if (dummy.activeSelf)
        {
            commandPosition = dummy.transform.position;
            commandRotation = dummy.transform.rotation;
        }
        else
        {
            commandPosition = transform.position;
            commandRotation = transform.rotation;
        }

        base.Update();

        //Dying; reload level
        if (health <= 0)
        {
            if (dyingTimer == 255)
            {
                dyingTimer = 3f;
                ActionUI.instance.DyingMessage();
            }
            if (dyingTimer <= 0)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            dyingTimer -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        //Use physics is real time is active
        if (gameManager.gameState == GameManager.gameStates.realTime)
            PhysicsCheck(transform.position);
    }

    private void LateUpdate()
    {
        //Update positions of help elements
        if (charGroundPlane.gameObject.activeSelf)
            charGroundPlane.transform.position = commandPosition;
    }

    //Spawn Dummy when instantiated
    private void SpawnDummy()
    {
            //Instantiate Dummy Object and give him your model
            dummy = Instantiate(dummyPrefab, transform.position, transform.rotation);
            Transform dummyModel = Instantiate(myModel, dummy.transform.position, dummy.transform.rotation, dummy.transform);
            dummyModel.name = "Model";
            dummyModel.gameObject.layer = 10;
            dummyCharControls = dummy.GetComponent<DummyPlayerCharacter>();
            //Copy values of normal collider
            dummy.GetComponent<CapsuleCollider>().center = GetComponent<CapsuleCollider>().center;
            dummy.GetComponent<CapsuleCollider>().radius = GetComponent<CapsuleCollider>().radius;
            dummy.GetComponent<CapsuleCollider>().height = GetComponent<CapsuleCollider>().height;
            //dummyCharControls.StartDummy();
            //Change color of dummy
            SkinnedMeshRenderer[] skinnedMeshRenderer = dummy.GetComponentsInChildren<SkinnedMeshRenderer>();
            MeshRenderer[] MeshRenderer = dummy.GetComponentsInChildren<MeshRenderer>();
            foreach (SkinnedMeshRenderer renderer in skinnedMeshRenderer)
            {
                renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, 0.5f);
            }
            foreach (MeshRenderer renderer in MeshRenderer)
            {
                foreach (Material mat in renderer.materials)
                {
                    mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, 0.5f);
                }    
            }
            dummyCharControls.myCreator = gameObject;
    }

    public void setCharacterSelected(bool setTrue)
    {
            if (setTrue)
            {
                isSelected = true;
            }
            else
            {
                isSelected = false;
                foreach (BaseAction action in myActionsContainer.GetComponents<BaseAction>())
                {
                    action.ActivateMenu(false);
                    dummy.SetActive(false);
                }
            }
    }

    public void ActivatePreviewModel(Quaternion myRotation, AnimationClip animClip)
    {
        previewModel.SetActive(true);
        previewModel.transform.rotation = myRotation;
        previewModel.GetComponent<Animator>().Play(animClip.name);
    }
}
