﻿using UnityEngine;
using System.Collections;

public class ThirdPersonCamera : MonoBehaviour {

    public float speed = 10f;

    public Vector3 offset = new Vector3( 0, 1, -5 );

    public LayerMask layerMask = 0x1; //default layer

    [HideInInspector]
    public new Transform transform;

    [HideInInspector]
    public new Camera camera;

    private Vector3 pivotPoint;
    private Player myPlayer;
    private Vector3 myAngle = Vector3.zero;

	// Use this for initialization
	void Awake () {
        transform = GetComponent<Transform>();
        camera = GetComponent<Camera>();
	}

    void Start()
    {
        myAngle = PlayerManager.getInstance().currentPlayer.transform.eulerAngles;
    }
	
	// Update is called once per frame
	void Update () {
        if (myPlayer != PlayerManager.getInstance().currentPlayer)
        {
            myPlayer = PlayerManager.getInstance().currentPlayer;
        }
	}

    public void LateUpdate()
    {
        if (myPlayer == null)
            return;

        Vector3 target = Vector3.zero;
        Vector3 view = Vector3.zero;

        Girl kira = (myPlayer as Girl);
        if (kira != null && kira.target != null)
        {
            targeting(out view, out target);
        }
        else
        {
            thirdPerson(out view, out target);
        }
        //smooth erratic camera movement
        Vector3 velocity = Vector3.zero;

        if (Vector3.Distance(transform.position, myPlayer.transform.position) > 10.0f)
        {
           transform.position = Vector3.SmoothDamp(transform.position, view, ref velocity, 0.15f);
        }
        else
        {
           transform.position = Vector3.SmoothDamp(transform.position, view, ref velocity, 0.015f);
        }
               
      transform.LookAt(target, Vector3.up);
    }

    void thirdPerson ( out Vector3 view, out Vector3 target )
    {
        float horizontal = Input.GetAxis("Alt_Horizontal");
        float vertical = Input.GetAxis("Alt_Vertical");
        Quaternion rotation = Quaternion.identity;

        if (Input.GetButtonDown("Center"))
        {
            myAngle = new Vector3(0, myPlayer.transform.eulerAngles.y, 0);
        }
        else if (horizontal != 0 || vertical != 0)
        {
            myAngle += new Vector3(-vertical, horizontal, 0) * (speed * Time.deltaTime);
        }
        //lock camera angles
        myAngle.x = Mathf.Clamp(myAngle.x, -15, 45);

        pivotPoint = myPlayer.collider.bounds.center;
        pivotPoint.y = myPlayer.collider.bounds.min.y + 0.1f;

        rotation = Quaternion.Euler(myAngle);
        view = pivotPoint + rotation * offset;

        RaycastHit hit;
        if (Physics.Linecast(pivotPoint, view, out hit, layerMask))
        {
         view = hit.point + (pivotPoint - hit.point).normalized * 0.1f;
        }

        target = pivotPoint;
        Debug.DrawLine(view, target);

    }

    void targeting (out Vector3 view, out Vector3 target)
    {
        Girl kira = (myPlayer as Girl);
        target = kira.target.position;

        Quaternion rotation = Quaternion.LookRotation(kira.transform.position - target);
        view = kira.transform.position + rotation * (Vector3.forward - Vector3.right * 0.4f) + Vector3.up;
   }
}
