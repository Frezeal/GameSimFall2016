﻿using UnityEngine;
using System.Collections;
using System.Linq;

public class Item : MonoBehaviour {

    private Collider myCollider;

    [HideInInspector]
    new public Collider collider
    {
        get
        {
            if (myCollider == null)
            {
                myCollider = GetComponent<Collider>();
            }
            return myCollider;
        }
    }

    public void OnCollisionEnter (Collision collision)
    {
        if (collision.transform.tag == "Player")
        {
            Inventory.getInstance().Add(tag);
            Destroy(gameObject);
        }
    }

    public void OnTriggerEnter (Collider other)
    {
        if (other.transform.tag == "Player")
        {
            Inventory.getInstance().Remove(tag);
            Destroy(gameObject);
        }
    }
}
