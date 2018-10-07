using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Cameras;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public List<PlayerCharacter> characters;
    public List<NPCControls> NPCs;
    public List<Rigidbody> otherObjects;

    public List<GameObject> characterPrefabs;
    public List<PlayerCharacter> myCharacters { get; private set; } = new List<PlayerCharacter>();

    public gameStates gameState;

    public enum gameStates
    {
        realTime,
        commandTime,
        observeTime,
    }

    private List<Transform> cameraTargets = new List<Transform>();

    //Input
    public bool spacePressed;

    //Camera
    public Camera myCam;
    private int cameraNumber;

    //CharacterSelect
    public PlayerCharacter selectedCharacter;

    //realTime
    public bool setupRealTime;
    //commandTime
    public bool setupCommandTime;
    //Recording
    public int timeSteps;
    public int timeStepsMax;
    //ObserveTime
    [HideInInspector]
    public bool setupObserveTime;

    //realTime
    private float passedTime;

    //UI
    public Transform screenSpaceUI;

    // Use this for initialization
    private void Awake()
    {
        //Singleton
        if (instance == null)
        {
            instance = this;
        }
        gameState = gameStates.realTime;
        myCam = Camera.main;
        int i = 1;
        foreach (GameObject chara in characterPrefabs)
        {
            GameObject newChara = Instantiate(chara, GameObject.FindWithTag("PlayerSpawnPoint" + i.ToString()).transform.position, GameObject.FindWithTag("PlayerSpawnPoint" + i.ToString()).transform.rotation);
            myCharacters.Add(newChara.GetComponent<PlayerCharacter>());
            i++;
        }
        MakeCharacterActive(myCharacters[0]);
    }

    public void AddCharacter(PlayerCharacter character)
    {
        characters.Add(character);
    }

    public void ChangeCameraTarget(Transform target)
    {
        myCam.transform.parent.parent.GetComponent<FreeLookCam>().SetTarget(target);
    }

    //Make an character active and controllable
    public void MakeCharacterActive(PlayerCharacter charControls)
    {
        if (selectedCharacter != null)
        {
            selectedCharacter.setCharacterSelected(false);
            ActionUI.instance.ResetUI();
            ActionUI.instance.ActivateMainButtons(true);
        }
        ChangeCameraTarget(charControls.gameObject.transform);
        gameState = gameStates.commandTime;
        setupCommandTime = false;
        ActionUI.instance.myCharacter = charControls;
        ActionUI.instance.UpdateButtons();
        charControls.setCharacterSelected(true);
        selectedCharacter = charControls;
        foreach (NPCControls npc in NPCs)
        {
            npc.target = selectedCharacter.gameObject;
        }
        selectedCharacter.usedActionTime = 0;
    }

    private void pollInput()
    {
        spacePressed = Input.GetButtonDown("Space");
    }

    private void Reset()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene(0);
        }
    }

    //Check if you have to make an character active and controllable
    private bool CheckCharacter()
    {
        if (selectedCharacter.actions.Count == 0 && selectedCharacter.finishedAction)
            {
                MakeCharacterActive(selectedCharacter);
                return true;
            }
        return false;
    }

    private void CheckLevelVictory()
    {
        bool NPCsAlive = false;
        foreach (BaseCharacter npc in NPCs)
        {
            if (npc.health > 0){
                NPCsAlive = true;
                break;
            }
        }
        if (!NPCsAlive)
        {
            gameState = gameStates.observeTime;
        }
    }

    private void Update()
    {
        CheckLevelVictory();
        pollInput();
        Reset();

        //State-Switches
        if (gameState == gameStates.commandTime)
        {
            if (!setupCommandTime)
            {
                ActionUI.instance.ActivateMainButtons(true);
                setupCommandTime = true;
                //Reset characters for commandTime
                foreach (PlayerCharacter character in characters)
                {
                    character.rb.isKinematic = true;
                }
                foreach (NPCControls npc in NPCs)
                {
                    npc.rb.isKinematic = true;
                }
                foreach (Rigidbody otherObject in otherObjects)
                {
                    otherObject.isKinematic = true;
                    otherObject.GetComponent<Collider>().enabled = false;
                }
            }

            //Change state
            if (spacePressed)
            {
                if (selectedCharacter.actions.Count != 0 && (selectedCharacter.usedActionTime > 1.99f || selectedCharacter.usedSwitchAction) && selectedCharacter.dummyCharControls.rb.isKinematic)
                {
                        setupRealTime = false;
                        gameState = gameStates.realTime;
                        spacePressed = false;
                }
            }
        }

        if (gameState == gameStates.realTime)
        {
            if (!setupRealTime)
            {
                ActionUI.instance.ResetUI();
                ChangeCameraTarget(selectedCharacter.transform);
                if (selectedCharacter != null)
                    selectedCharacter.GetComponent<PlayerCharacter>().setCharacterSelected(false);
                setupRealTime = true;
                selectedCharacter.skill += selectedCharacter.skillBonus;
                selectedCharacter.skillBonus = 0;
                selectedCharacter.usedActionTime -= Time.deltaTime / 2;
                foreach (PlayerCharacter character in characters)
                {
                    character.rb.isKinematic = false;
                    character.setCharacterSelected(false);
                    character.currentActionTime = 0;
                }
                foreach (NPCControls npc in NPCs)
                {
                    npc.rb.isKinematic = false;
                }
                foreach (Rigidbody otherObject in otherObjects)
                {
                    otherObject.isKinematic = false;
                    otherObject.GetComponent<Collider>().enabled = true;
                }
                passedTime += Time.deltaTime / 2;
            }
            else
            {
                cameraTargets.Add(myCam.transform.parent.parent.GetComponent<FreeLookCam>().Target);
                CheckCharacter();
                //Record position and time each frame
                selectedCharacter.usedActionTime -= Time.deltaTime;
                foreach (PlayerCharacter character in characters)
                {
                    character.gameObject.GetComponent<RecordMovement>().RecordMovementFunction();
                    
                }
                foreach (NPCControls npc in NPCs)
                {
                    npc.gameObject.GetComponent<RecordMovement>().RecordMovementFunction();
                }
                foreach (Rigidbody otherObject in otherObjects)
                {
                    otherObject.gameObject.GetComponent<RecordMovement>().RecordMovementFunction();
                }
                timeStepsMax++;
                passedTime += Time.deltaTime;
            }
        }

        if (gameState == gameStates.observeTime)
        {
            if (!setupObserveTime)
            {
                timeSteps = 0;
                setupObserveTime = true;
                foreach (NPCControls npc in NPCs)
                {
                    npc.rb.isKinematic = true;
                }
                foreach (PlayerCharacter character in characters)
                {
                    character.rb.isKinematic = true;
                }
                foreach (Rigidbody otherObject in otherObjects)
                {
                    otherObject.isKinematic = true;
                }
            }
            else
            {
                ChangeCameraTarget(cameraTargets[timeSteps]);
                foreach (BaseCharacter character in characters)
                {
                    character.gameObject.GetComponent<RecordMovement>().PlayMovementFunction(timeSteps);
                }
                foreach (BaseCharacter npc in NPCs)
                {
                    npc.gameObject.GetComponent<RecordMovement>().PlayMovementFunction(timeSteps);
                }
                foreach (Rigidbody otherObject in otherObjects)
                {
                    otherObject.gameObject.GetComponent<RecordMovement>().PlayMovementFunction(timeSteps);
                    otherObject.GetComponent<Collider>().enabled = false;
                }

                timeSteps++;
                if (timeSteps == timeStepsMax - 1)
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                }
            }
        }
    }
}


