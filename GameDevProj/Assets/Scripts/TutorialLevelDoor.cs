﻿using UnityEngine;
using System.Collections;
using Assets.Scripts;
using System;

public class TutorialLevelDoor : MonoBehaviour, Interactable {

    public static bool locked = true;

    public string getLabel()
    {
        if(locked)
        {
            return "Locked. (Need key)";
        }
        else
        {
            return "Exit tutorial";
        }
    }

    public void interact(GameObject player)
    {
        if(!locked)
        {
            PlayerPrefs.SetString(GameConstants.GET_SCENE_TO_LOAD, "Level2");
            Application.LoadLevel(GameConstants.LOADING);
        }
    }

    public bool isInteractable()
    {
        return true;
    }

}
