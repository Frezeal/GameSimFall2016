﻿using UnityEngine;
using System.Collections;
using System.Linq;
using System;

public class Girl : Player {

    new public enum State
    {
        FALL,
        ATTACK,
    };

    [HideInInspector]
    public Animator animator;
    
    public GameObject rockPrefab;
    public Transform rockSpawnNode;
    public float shootingForce = 30.0f;
       
    [HideInInspector]
    public Transform target
    {
        get { return myTarget; }
    }

    private int myTargetableLayerMask = 1 << 9;
    private bool isTargeting = false;
    private Collider[] myTargets;
    private Transform myTarget;
    private int myTargetIndex;



    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
        addRunnable(Player.State.DEFAULT, runMoveState);
        addRunnable(State.FALL, runFallingState);
        addRunnable(State.ATTACK, runAttackState);
        //myTargetableLayerMask = 1 << LayerMask.NameToLayer("Targetable"); 
    }

    virtual protected void runMoveState()
    {
        RaycastHit hit;
        if (rigidbody.useGravity)
        {
            if (isGrounded(out hit))
            {
                animator.applyRootMotion = true;
                setParent(hit);
                movePlayer();

                if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
                {
                    animator.SetInteger("state", 1);
                }
                else
                {
                    animator.SetInteger("state", 0);
                }
            }
            else
            {
                animator.applyRootMotion = false;
                animator.SetInteger("state", 2);
                playerState = State.FALL;
            }
        }

        if (!playerState.Equals(Player.State.DEFAULT))
        {
            return;
        }

        if (Input.GetButtonDown("Attack") && myStates.ContainsKey(State.ATTACK))
        {
            animator.SetInteger("state", 3);
            animator.SetInteger("substate", 2);
            playerState = State.ATTACK;
            //this.slingShot.volume = .05f;
			   //this.slingShot.Play ();
        }
        targetSetup();
    }

    protected void runFallingState()
    {
        if (isGrounded())
        {
            playerState = Player.State.DEFAULT;
        }
        else
        {
            clearParent();
        }
    }

    void runAttackState()
    {

        if (!isGrounded())
        {
            animator.SetInteger("state", 2);
            playerState = State.FALL;
            return;
        }

        if (Input.GetButton("Vertical") && animator.GetCurrentAnimatorStateInfo(0).IsName("Posing"))
        {
            animator.SetInteger("state", 0);
            playerState = Player.State.DEFAULT;
        }

        strafe();



        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Shoot") && animator.GetInteger("substate") != 0)
        {
            animator.SetInteger("substate", 0);
            fire();
        }
        else if (Input.GetButtonDown("Attack") && animator.GetCurrentAnimatorStateInfo(0).IsName("Posing"))//
        {
            animator.SetInteger("substate", 1);
            //animator.SetInteger("state", 1);
            //animator.SetInteger("substate", 0);
            //playerState = Player.State.DEFAULT;
        }
        targetSetup();

        ////toggle our shooting target
        //if (Input.GetButtonDown("Action") && myTargets.Length != 0)
        //{
        //    int index = (Array.IndexOf(myTargets, myTarget.GetComponent<Collider>()) + 1) % myTargets.Length;
        //    myTarget = myTargets[index].transform;
        //}
    }

    void fire ()
    {
        GameObject rock = Instantiate(rockPrefab, rockSpawnNode.position, transform.rotation) as GameObject;
        Rigidbody rockBody = rock.GetComponent<Rigidbody>();
        //Physics.IgnoreCollision(rockBody.GetComponent<Collider>(), rigidbody.GetComponent<Collider>());
        Vector3 force = (target != null ? (target.position - rigidbody.position).normalized : transform.forward) * shootingForce;
        rockBody.AddForce(force, ForceMode.Impulse);  // should use velocity
    }

    void strafe ()
    {
        Vector3 cameraFoward = camera.transform.forward;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 direction = camera.transform.forward;
        direction.y = 0;//aligns facing direction with the ground.

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothSpeed * Time.deltaTime);

        animator.SetFloat("Horizontal", h);

        //if (myTarget != null)
        //{
        //    //rotate towards the shooting target
        //    rigidbody.position += ((camera.transform.forward * v) +
        //                           (camera.transform.right * h)) *
        //                            movementSpeed * Time.deltaTime;
        //}
    }

    bool findTargets()
    {
        myTargets = Physics.OverlapSphere(rigidbody.position, targetRange, myTargetableLayerMask);

        if (myTarget == null || Vector3.Distance(rigidbody.position, myTarget.position) > targetRange)
        {
            myTarget = (myTargets.Length != 0 ? myTargets[0].transform : null);

            for (int index = 0; index < myTargets.Length; index++)
            {
                float currentAngle = Vector3.Angle(camera.transform.forward, myTarget.position - rigidbody.position);
                float angle = Vector3.Angle(camera.transform.forward, myTargets[index].transform.position - rigidbody.position);
                if (angle < currentAngle)
                {
                    myTarget = myTargets[index].transform;
                    myTargetIndex = index;
                }
            }
        }
        return (myTarget != null);
    }

    void targetSetup ()
    {
       if (Input.GetButtonDown("Center"))
       {
          isTargeting = !isTargeting;
       }
       if (isTargeting)
       {
          if (!findTargets())
          {
             isTargeting = false;
          }
          else
          {
             if (Input.GetButtonDown("Action"))
             {
                 myTargetIndex = ++myTargetIndex % myTargets.Length;
                 myTarget = myTargets[myTargetIndex].transform;
             }
          }
       }
       else
       {
          myTarget = null;
       }
    }

    void LateUpdate()
    {
        if (PlayerManager.getInstance().currentPlayer != this)
        {
            animator.SetInteger("state", 0);
            playerState = Player.State.DEFAULT;
        }
        animator.SetFloat("Fear", StatusManager.getInstance().fear);
        //posisonEffect.SetActive((status & Status.POISON) == Status.POISON);
        //fireEffect.SetActive((status & Status.FIRE) == Status.FIRE);
        //darkEffect.SetActive((status & Status.DARK) == Status.DARK);
        //if (darkEffect.activeSelf && GameObject.FindObjectOfType<FearManager>().fearCurrent >= 100 )
        //{
        //    fearEffect.SetActive(true);
        //}
        //else
        //{
        //    fearEffect.SetActive(false);
        //}
    }
}
