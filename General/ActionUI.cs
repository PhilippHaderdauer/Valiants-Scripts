using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class ActionUI : MonoBehaviour {

    public static ActionUI instance;

    //MainButtons
    [SerializeField]
    private List<GameObject> mainButtons;

    //ScreenSpaceUI
    private Slider healthBar;
    private Text healthPoints;

    //Camera
    public Camera myCam;

    //Movement
    [SerializeField]
    private GameObject movementMenu;
    public Vector3 mousePosition { get; set; }
    public GameObject myCursor;

    //Brawl
    [SerializeField]
    private GameObject brawlMenu;
    [SerializeField]
    private GameObject meleeMenu;
    [SerializeField]
    private Text[] meleeTexts;

    //Special
    [SerializeField]
    private GameObject specialMenu;

    //Switch
    [SerializeField]
    private Text[] switchTexts;
    [SerializeField]
    private GameObject switchMenu;
    private int[] switchIndices = new int[2];

    //Character UI
    [SerializeField]
    private Text[] timers;
    [SerializeField]
    private Text[] healthTexts;
    [SerializeField]
    private Text[] skillTexts;
    [SerializeField]
    private Slider[] healthSliders;
    [SerializeField]
    private Slider[] skillSliders;
    [SerializeField]
    public GameObject skillText;

    //Helper Text
    [SerializeField]
    private Text youDied;
    [SerializeField]
    private GameObject instructions;
       
    //Character
    public PlayerCharacter myCharacter;

    //UIRaycast
    GraphicRaycaster graphicRay;
    PointerEventData pointerEventData = new PointerEventData(null);
    private List<RaycastResult> raycastResults = new List<RaycastResult>();
    public static List<string> hitUIElements { get; } = new List<string>();

    //Raycast
    private RaycastHit sceneHit;
    private Ray sceneRay;

    //Button Handling
    public void ButtonHandling(string function)
    {
        foreach (BaseAction goapAction in myCharacter.myActionsContainer.GetComponents<BaseAction>())
        {
            goapAction.menuActive = false;
        }
        if (!myCharacter.usedSwitchAction)
        {
        if (myCharacter.myActionsContainer.GetComponent<JumpAction>())
            if (function == "JumpAction")
            {
                myCharacter.myActionsContainer.GetComponent<JumpAction>().menuActive = true;
            }
        if (myCharacter.myActionsContainer.GetComponent<WaitAction>())
            if (function == "WaitAction")
            {
                myCharacter.myActionsContainer.GetComponent<WaitAction>().menuActive = true;
            }
        if (myCharacter.myActionsContainer.GetComponent<WalkRunAction>())
            if (function == "WalkRunAction")
            {
                myCharacter.myActionsContainer.GetComponent<WalkRunAction>().menuActive = true;
            }
        if (myCharacter.myActionsContainer.GetComponent<BlockAction>())
            if (function == "BlockAction")
            {
                myCharacter.myActionsContainer.GetComponent<BlockAction>().menuActive = true;
            }
        if (myCharacter.myActionsContainer.GetComponent<SwitchAction>())
        {
            if ((function == "Switch0"))
            {
                myCharacter.myActionsContainer.GetComponent<SwitchAction>().menuActive = true;
                myCharacter.myActionsContainer.GetComponent<SwitchAction>().switchNumber = switchIndices[0];
            }
            if ((function == "Switch1"))
            {
                myCharacter.myActionsContainer.GetComponent<SwitchAction>().menuActive = true;
                myCharacter.myActionsContainer.GetComponent<SwitchAction>().switchNumber = switchIndices[1];
            }
        }
        if (myCharacter.myActionsContainer.GetComponent<MeleeAction>())
        {
            if (function == "Melee0")
            {
                myCharacter.myActionsContainer.GetComponent<MeleeAction>().menuActive = true;
                myCharacter.myActionsContainer.GetComponent<MeleeAction>().meleeActionNumber = 0;
            }
            if (function == "Melee1")
            {
                myCharacter.myActionsContainer.GetComponent<MeleeAction>().menuActive = true;
                myCharacter.myActionsContainer.GetComponent<MeleeAction>().meleeActionNumber = 1;
            }
            if (function == "Melee2")
            {
                myCharacter.myActionsContainer.GetComponent<MeleeAction>().menuActive = true;
                myCharacter.myActionsContainer.GetComponent<MeleeAction>().meleeActionNumber = 2;
            }
            if (function == "Melee3")
            {
                myCharacter.myActionsContainer.GetComponent<MeleeAction>().menuActive = true;
                myCharacter.myActionsContainer.GetComponent<MeleeAction>().meleeActionNumber = 3;
            }
        }
        }
    }

    public void UpdateButtons()
    {
        if (myCharacter.myActionsContainer.GetComponent<MeleeAction>())
        {
            for (int i = 0; i < 4; i++)
            {
                meleeTexts[i].text = myCharacter.myActionsContainer.GetComponent<MeleeAction>().myAnimClips[i].name;
            }
        }
        if (myCharacter.myActionsContainer.GetComponent<SwitchAction>())
        {
            int i = 0;
            int k = 0;
            foreach(PlayerCharacter chara in GameManager.instance.characters)
            {
                if (chara != myCharacter)
                {
                    switchTexts[i].text = chara.gameObject.name.Replace("(Clone)", "");
                    switchIndices[i] = k;
                    i++;
                }
                k++;
            }
        }
    }

    // Use this for initialization
    private void Awake () {
        if (instance == null) {
            instance = this;
        }
        myCam = Camera.main;
        graphicRay = GetComponent<GraphicRaycaster>();
	}

    private void ChangeCam(Camera cam)
    {
        myCam = cam;
    }

    // Update is called once per frame
    private void Update () {
        if (myCharacter != null)
            RayCasting();
        UpdateCharacterUI();
        if (Input.GetKeyDown(KeyCode.F1))
        {
            instructions.SetActive(!instructions.activeSelf);
        }
        if (myCursor.activeSelf)
        {
            if (GameManager.instance.gameState != GameManager.gameStates.commandTime)
            {
                myCursor.SetActive(false);
            }
        }
        else
        {
            if (GameManager.instance.gameState == GameManager.gameStates.commandTime)
            {
                myCursor.SetActive(true);
            }
        }
    }

    public void DyingMessage()
    {
        youDied.gameObject.SetActive(true);
    }

    private void UpdateCharacterUI()
    {
        int i = 0;
        foreach (PlayerCharacter chara in GameManager.instance.myCharacters)
        {
            timers[i].text = Math.Round(chara.usedActionTime, 1).ToString("0.0") + "\n" + "<color=red>+</color>" + Math.Round(chara.currentActionTime, 1).ToString("<color=red>0.0</color>");
            healthSliders[i].value = chara.health / chara.healthMax * 100;
            skillSliders[i].value = (chara.skill+chara.skillBonus+chara.skillMali) / chara.skillMax * 100;
            healthTexts[i].text = chara.health + " / " + chara.healthMax;
            skillTexts[i].text = (chara.skill + chara.skillMali) + " / " + chara.skillMax + " + " + chara.skillBonus;
            i++;
        }
    }

    private void RayCasting()
    {
        //Mouse input to ray
        if (myCam != null)
            sceneRay = myCam.ScreenPointToRay(Input.mousePosition);

        //Collision with scene
        if (Physics.Raycast(sceneRay, out sceneHit, Mathf.Infinity, 1 << 8))
        {
            mousePosition = sceneHit.point;
        }

        //Collision with character plane instead of normal level to aim
        if (myCharacter.myActionsContainer.GetComponent<MeleeAction>())
        {
            if (myCharacter.myActionsContainer.GetComponent<MeleeAction>().menuActive)
            {
                if (Physics.Raycast(sceneRay, out sceneHit, Mathf.Infinity, 1 << 9))
                {
                    mousePosition = sceneHit.point;
                }
            }
        }

        //Collision with character plane instead of normal level to aim
        if (myCharacter.myActionsContainer.GetComponent<BlockAction>())
        {
            if (myCharacter.myActionsContainer.GetComponent<BlockAction>().menuActive)
            {
                if (Physics.Raycast(sceneRay, out sceneHit, Mathf.Infinity, 1 << 9))
                {
                    mousePosition = sceneHit.point;
                }
            }
        }

        //UI Raycast
        raycastResults.Clear();
        hitUIElements.Clear();
        pointerEventData.position = Input.mousePosition;
        graphicRay.Raycast(pointerEventData, raycastResults);
        foreach (RaycastResult result in raycastResults)
        {
            hitUIElements.Add(result.gameObject.name);
        }
    }

    public void ActivateMainButtons(bool setTrue)
    {
        foreach (GameObject button in mainButtons)
        {
            button.SetActive(setTrue);
        }
    }

    public void ActivateMovementMenu(bool setTrue)
    {
        movementMenu.SetActive(setTrue);
    }

    public void ActivateBrawlMenu(bool setTrue)
    {
        brawlMenu.SetActive(setTrue);
    }

    public void ActivateSwitchMenu(bool setTrue)
    {
        switchMenu.SetActive(setTrue);
    }

    public void ActivatePunchMenu(bool setTrue)
    {
        meleeMenu.SetActive(setTrue);
    }

    public void ActivateSpecialMenu(bool setTrue)
    {
        specialMenu.SetActive(setTrue);
    }

    public void ResetUI()
    {
        ActivateMainButtons(false);
        ActivateMovementMenu(false);
        ActivateSwitchMenu(false);
        ActivateBrawlMenu(false);
        ActivatePunchMenu(false);
        ActivateSpecialMenu(false);
    }
}
