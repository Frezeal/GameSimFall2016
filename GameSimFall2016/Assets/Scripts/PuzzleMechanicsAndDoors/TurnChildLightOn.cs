﻿using UnityEngine;
using System.Collections;

//========================================================================================================
//                             Turn Child Light On
// Guess what folks. You wanna know what this script does?
// Are you ready for this?
// It turns on a light that is a component of the object.
// Wow. well. I guess the name is wrong, since it's not technically a child, it's a component. But yeah.                          
//========================================================================================================

public class TurnChildLightOn : MonoBehaviour {
	public AudioSource lightSound;

	// Use this for initialization
	void Start () {
		
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public void OnEvent(BasicTrigger trigger)
    {
        //if (trigger.message == "turnOn")
            GetComponent<Light>().intensity = 8f;
		this.lightSound.Play ();
			
    }
}
