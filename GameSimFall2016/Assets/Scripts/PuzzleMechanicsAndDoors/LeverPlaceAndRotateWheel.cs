﻿using UnityEngine;
using System.Collections;

//========================================================================================================
//                                   Lever Place and Rotate Wheel  
// This Script puts a lever down using a BASIC TRIGGER and rotates the wheel based on its place.   
//========================================================================================================

public class LeverPlaceAndRotateWheel : MonoBehaviour {
    [Tooltip("the lever")]
    public GameObject lever;
    [Tooltip("tag of the lever")]
    public Tag leverTag;
    
    [Tooltip("the wheel to turn")]    
    public GameObject wheel;   //wheel to turn

    [Tooltip("Number of sides/degrees")]
    public int sides;         // number of sides so as to rotate just one section
    [Tooltip("currently unused")]
    public int tallness;      // height that needs to be pushed in


    private bool turnWheel;   // can the wheel be turned?
    private bool leverDropped; //has the lever been placed?




    // Use this for initialization
    void Start () {

        turnWheel = false;
        leverDropped = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (turnWheel)
        {
            if (Input.GetButtonDown("Action"))
            {
                wheel.transform.Rotate(0, 0, 360 / sides);
            }
        }
    }

    void OnEvent(BasicTrigger trigger)
    {
        //if (trigger.message == "placeLever")
        {
            if (!leverDropped)
            {
                if (Inventory.getInstance().Has(leverTag))
                {
                    lever = Instantiate(lever) as GameObject;
                    lever.transform.position = gameObject.transform.position;
                    Vector3 vec = lever.transform.position;
                    vec.y = lever.transform.position.y + 1.5f;
                    lever.transform.forward = transform.up;
                    lever.transform.position = vec;
                    Debug.Log("leverDropped");
                    Inventory.getInstance().Remove(leverTag);
                    turnWheel = true;
                    leverDropped = true;

                }
            }
            else
                turnWheel = true;
        }
    }


    void OnEventEnd(BasicTrigger trigger)
    {
        turnWheel = false;

    }



}
