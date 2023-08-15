using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantlyRotate : MonoBehaviour
{
    public float rotationSpeed;
    public bool rotating = true;
    Rigidbody rb;

    //material
    public List<MeshRenderer> myMaterial;
    public List<Color> myColors; 

    void Start()
    {
        rb = GetComponent<Rigidbody>();
            StartCoroutine("Rotate");
    }

    public IEnumerator Rotate()
    {
        rb.angularVelocity = Vector3.up * rotationSpeed;
        yield return new WaitForSeconds(5);
        if (rotating)
            StartCoroutine("Rotate");
    }
    public void ChangeColor(int colorIndex)
    {
        foreach(MeshRenderer mr in myMaterial)
            mr.material.color = myColors[colorIndex];
    }
}
