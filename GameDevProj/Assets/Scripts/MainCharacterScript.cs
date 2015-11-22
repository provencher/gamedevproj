﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Assets.Scripts;

public class MainCharacterScript : MonoBehaviour {

    Flashlight spotLight;
    GameObject faceUiLarge; //Used for notes or hints
    GameObject faceUiSmall; //Used for "Press E to interact" text

    Text smallUiText;
    Text largeUiText;

    

    GameObject currentReadable = null;

    GameObject[] ghouls;

    Light[] allLights;
    int sceneLightCount = 0;

    bool buttonOneDown = false;
    bool buttonOneWasDown = false;
    bool buttonTwoDown = false;
    bool buttonTwoWasDown = false;
    bool buttonThreeDown = false;
    bool buttonThreeWasDown = false;
    bool buttonFourDown = false;
    bool buttonFourWasDown = false;

    const string SMALL_UI_TEXT = "(X) ";

    bool faceUiSmallActive = false;
    bool faceUiLargeActive = false;

	// Use this for initialization
	void Start () {
        sceneLightCount = 0;
        allLights = new Light[50];
        spotLight = GetComponentInChildren<Flashlight>();
        faceUiLarge = GameObject.Find("FaceUILarge");
        largeUiText = faceUiLarge.GetComponentInChildren<Text>();
        faceUiLarge.SetActive(false);
        
        faceUiSmall = GameObject.Find("FaceUISmall");
        smallUiText = faceUiSmall.GetComponentInChildren<Text>();
        faceUiSmall.SetActive(false);

        SanitySystem.init();

        ghouls = GameObject.FindGameObjectsWithTag("ghoul");
        foreach (GameObject ghoul in ghouls)
        {
            ghoul.SetActive(false);
        }

        GameObject[] allItems = GameObject.FindGameObjectsWithTag("item");

        foreach(GameObject go in allItems)
        {
            if(go.name.Equals("Torch"))
            {
                allLights[sceneLightCount++] = go.GetComponentInChildren<Light>();
            }
        }
        Debug.Log("Found a total of " + sceneLightCount + " lights in the scene.");

    }
	
	// Update is called once per frame
	void Update () {

        UpdateGamepadButtons();
        faceUiSmallActive = false;
        faceUiLargeActive = false;
        if (Input.GetKeyUp(KeyCode.F) || buttonFourClicked())
        {
            spotLight.toggleLight();
            if(spotLight.lightEnabled)
            {
                deactivateGhouls();
                
            }
            else
            {
                activateGhouls();
            }
        }
        if (!spotLight.lightEnabled)
        {
            bool nearLight = false;
            //Check if in light or in the dark to increase sanity
            foreach(Light light in allLights)
            {
                if(light == null)
                {
                    break;
                }
                if(light.enabled && (light.transform.position - transform.position).magnitude < light.GetComponentInChildren<Light>().range)
                {
                    nearLight = true;
                }
            }
            if(!nearLight)
            {
                Debug.Log("Not near light!");
                SanitySystem.increaseInsanity(1);
            }
            else
            {
                Debug.Log("Near light!!!");
                //In light, decrease insanity
                SanitySystem.decreaseInsanity(1);
            }
            
        }
        else
        {
            //In light, decrease insanity
            SanitySystem.decreaseInsanity(1);
        }



        //Cast some rays to see if you're pointing at any creatures
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 10f))
        {
            if(hit.collider.gameObject != null)
            {
                GameObject hitObject = hit.collider.gameObject;
                if(hit.distance < 2.0f && hitObject.tag.Equals("item"))
                {
                    smallUiText.text = SMALL_UI_TEXT + hitObject.name;
                    faceUiSmallActive = true;
                    if ((Input.GetKeyUp(KeyCode.X) || buttonThreeClicked()) && hitObject.GetComponent<Interactable>() != null)
                    {
                        hitObject.GetComponent<Interactable>().interact(this.gameObject);
                    }

                }
                else if (hit.distance < 1.5f && hitObject.tag.Equals("readable"))
                {
                    if(currentReadable == null || !currentReadable.Equals(hitObject))
                    {
                        currentReadable = hitObject;
                        largeUiText.text = hitObject.GetComponent<Readable>().GetText();
                    }
                    faceUiLargeActive = true;

                }
                else if(hitObject.name.StartsWith("ghoul") && spotLight.lightEnabled)
                {
                    GhoulAI ghoul = hitObject.GetComponent<GhoulAI>();
                    ghoul.disappear();
                }

            }
        }
            
        faceUiSmall.SetActive(faceUiSmallActive);
        faceUiLarge.SetActive(faceUiLargeActive);
        
	}

    private void UpdateGamepadButtons()
    {
        buttonOneWasDown = buttonOneDown;
        buttonTwoWasDown = buttonTwoDown;
        buttonThreeWasDown = buttonThreeDown;
        buttonFourWasDown = buttonFourDown;

        buttonOneDown = OVRInput.Get(OVRInput.Button.One);
        buttonTwoDown = OVRInput.Get(OVRInput.Button.Two);
        buttonThreeDown = OVRInput.Get(OVRInput.Button.Three);
        buttonFourDown = OVRInput.Get(OVRInput.Button.Four);
    }

    private bool buttonOneClicked() //A button
    {
        return buttonOneDown && !buttonOneWasDown;
    }
    private bool buttonTwoClicked() //B
    {
        return buttonTwoDown && !buttonTwoWasDown;
    }
    private bool buttonThreeClicked()//X
    {
        return buttonThreeDown && !buttonThreeWasDown;
    }
    private bool buttonFourClicked()//Y
    {
        return buttonFourDown && !buttonFourWasDown;
    }

    public void activateGhouls()
    {
        foreach (GameObject ghoul in ghouls)
        {
            ghoul.SetActive(true);
        }
    }

    public void deactivateGhouls()
    {
        foreach (GameObject ghoul in ghouls)
        {
            ghoul.SetActive(false);
        }
    }
}
