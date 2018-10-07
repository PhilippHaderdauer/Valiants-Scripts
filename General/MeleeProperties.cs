using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MeleeProperties : MonoBehaviour {

    //Class that saves all the properties of melee attacks

    public static MeleeProperties instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    //Make hitboxes and set velocity according to received string
    public void GetProperties(string attackName, BaseCharacter baseChar, bool getInfo)
    {
        if (attackName == "Tackle")
        {
            if (getInfo)
            {
                baseChar.skillMali = -40;
                ActionUI.instance.skillText.GetComponent<Text>().text = "Damage: 50" + " Knockback Speed: 3" + " Stun Time: 0.5";
            }
            else
                baseChar.MakeHitBox(new Vector3(0.5f, 1f, 0.5f), new Vector3(0.12f, 1.2f, 0.33f),0.5f,0.6f,50,3,0.5f);
        }
        else if (attackName == "Power Smash")
        {
            if (getInfo)
            {
                baseChar.skillMali = -20;
                ActionUI.instance.skillText.GetComponent<Text>().text = "Damage: 75" + " Knockback Speed: 2" + " Stun Time: 1";
            }
            else
                baseChar.MakeHitBox(new Vector3(0.4f, 1f, 0.4f), new Vector3(0f, 0.45f, 0.59f),0.3f,0.5f,75,2,1);
        }
        else if (attackName == "Light Punch")
        {
            if (getInfo)
            {
                baseChar.skillMali = -10;
                ActionUI.instance.skillText.GetComponent<Text>().text = "Damage: 25" + " Knockback Speed: 0" + " Stun Time: 0";
            }
            else
                baseChar.MakeHitBox(new Vector3(0.66f, 1f, 0.66f), new Vector3(0.3f, 1.1f, 0.52f),0.3f,0.2f,25,0,0);
        }
        else if (attackName == "Punch Combination")
        {
            if (getInfo)
            {
                baseChar.skillMali = -30;
                ActionUI.instance.skillText.GetComponent<Text>().text = "Damage: 25" + " Knockback Speed: 1" + " Stun Time: 0.5";
            }
            else
            {
                baseChar.MakeHitBox(new Vector3(1f, 1f, 1f), new Vector3(0.3f, 1.1f, 0.52f),0.1f,0.15f,25,1,0.5f);
                baseChar.MakeHitBox(new Vector3(1f, 1f, 1f), new Vector3(-0.44f, 1.1f, 0.38f),0.1f,0.3f,25,1,0.5f);
                baseChar.MakeHitBox(new Vector3(1f, 1f, 1f), new Vector3(0.59f, 1.1f, 0.06f),0.1f,0.5f,25,1,0.5f);
            }
        }
        else if (attackName == "Right Uppercut")
        {
            if (getInfo)
            {
                baseChar.skillMali = -30;
                ActionUI.instance.skillText.GetComponent<Text>().text = "Damage: 25" + " Knockback Speed: 2" + " Stun Time: 0.5";
            }
            else
                baseChar.MakeHitBox(new Vector3(0.44f, 1f, 0.36f), new Vector3(0.3f, 1.1f, 0.52f),0.3f,0.2f,25,2,0.5f);
        }
        else if (attackName == "Upward Thrust")
        {
            if (getInfo)
            {
                baseChar.skillMali = -20;
                ActionUI.instance.skillText.GetComponent<Text>().text = "Damage: 75" + " Knockback Speed: 0" + " Stun Time: 2";
            }
            else
                baseChar.MakeHitBox(new Vector3(0.44f, 1f, 0.36f), new Vector3(0.74f, 1.1f, 0.51f),0.3f,0.75f,75,0,2);
        }
        else if (attackName == "Flying Kick")
        {
            if (getInfo)
            {
                baseChar.skillMali = -20;
                ActionUI.instance.skillText.GetComponent<Text>().text = "Damage: 50" + " Knockback Speed: 2" + " Stun Time: 1";
            }
            else
                baseChar.MakeHitBox(new Vector3(0.44f, 1f, 0.36f), new Vector3(-1.15f, 0.95f, 0.274f),0.15f,0.4f,50,2,1);
        }
        else if (attackName == "Flying Punch")
        {
            if (getInfo)
            {
                baseChar.skillMali = -40;
                ActionUI.instance.skillText.GetComponent<Text>().text = "Damage: 100" + " Knockback Speed: 0" + " Stun Time: 0";
            }
            else
                baseChar.MakeHitBox(new Vector3(2f, 2f, 2f), new Vector3(-0.25f, 0.45f, 0.73f), 0.25f,1.25f,100,0,0);
        }
        else if (attackName == "Front Thrust Kick")
        {
            if (getInfo)
            {
                baseChar.skillMali = -20;
                ActionUI.instance.skillText.GetComponent<Text>().text = "Damage: 50" + " Knockback Speed: 2" + " Stun Time: 1";
            }
            else
                baseChar.MakeHitBox(new Vector3(0.53f, 0.65f, 0.79f), new Vector3(-0.48f, 0.79f, 0.92f),0.2f,0.2f,50,2,1);
        }
        else if (attackName == "Punch Right")
        {
            if (getInfo)
            {
                baseChar.skillMali = -10;
                ActionUI.instance.skillText.GetComponent<Text>().text = "Damage: 25" + " Knockback Speed: 0" + " Stun Time: 0";
            }
            else
                baseChar.MakeHitBox(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0.23f, 1.35f, 0.65f),0.15f,0.2f,25,0,0);
        }
        else if (attackName == "Kick Right")
        {
            if (getInfo)
            {
                baseChar.skillMali = -20;
                ActionUI.instance.skillText.GetComponent<Text>().text = "Damage: 50" + " Knockback Speed: 1" + " Stun Time: 0.5";
            }
            else
                baseChar.MakeHitBox(new Vector3(0.63f, 0.63f, 0.63f), new Vector3(0.85f, 1.2f, -0.65f),0.1f,0.35f,50,1,0.5f);
        }
        else if (attackName == "Leg Sweep")
        {
            if (getInfo)
            {
                baseChar.skillMali = -30;
                ActionUI.instance.skillText.GetComponent<Text>().text = "Damage: 75" + " Knockback Speed: 0" + " Stun Time: 2";
            }
            else
                baseChar.MakeHitBox(new Vector3(1f, 1f, 1f), new Vector3(0.5f, 0.73f, -0.69f),0.1f,1.5f,75,0,2);
        }
        else if (attackName == "Inside Crescent Kick AI")
        {
            if (getInfo)
            {
                //baseChar.skillMali = 0;
                //ActionUI.instance.skillText.GetComponent<Text>().text = "Damage: 100" + "Knockback Speed: 0" + " Stun Time: 0";
            }
            else
                baseChar.MakeHitBox(new Vector3(0.53f, 0.65f, 0.79f), new Vector3(1.13f, 0.79f, 0.38f),0.3f,1.3f,75,2,1);
        }
        else if (attackName == "Left Jab AI")
        {
            if (getInfo)
            {
                //baseChar.skillMali = 0;
                //ActionUI.instance.skillText.GetComponent<Text>().text = "Damage: 100" + "Knockback Speed: 0" + " Stun Time: 0";
            }
            else
                baseChar.MakeHitBox(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(-0.52f, 1.29f, 0.33f),0.15f,0.2f,25,0,0);
        }
        else if (attackName == "Kicking AI")
        {
            if (getInfo)
            {
                //baseChar.skillMali = 0;
                //ActionUI.instance.skillText.GetComponent<Text>().text = "Damage: 100" + "Knockback Speed: 0" + " Stun Time: 0";
            }
            else
                baseChar.MakeHitBox(new Vector3(0.63f, 0.63f, 0.63f), new Vector3(-0.44f, 0.67f, 1.04f),0.2f,1.25f,50,1,1);
        }
        else if (attackName == "High Right Roundhouse AI")
        {
            if (getInfo)
            {
                //baseChar.skillMali = 0;
                //ActionUI.instance.skillText.GetComponent<Text>().text = "Damage: 100" + "Knockback Speed: 0" + " Stun Time: 0";
            }
            else
                baseChar.MakeHitBox(new Vector3(1f, 1f, 1f), new Vector3(-0.3f, 0.4f, 0.86f),0.1f,0.45f,25,1,1);
        }
    }
}
