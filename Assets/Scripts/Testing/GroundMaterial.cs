using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundMaterial : MonoBehaviour {

    public Color color;
    public string ignoreTag = "Terrain";

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != ignoreTag)
        {
            collision.gameObject.GetComponent<Renderer>().material.color = color;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag != ignoreTag)
        {
            collision.gameObject.GetComponent<Renderer>().material.color = Color.white;
        }
    }
}
