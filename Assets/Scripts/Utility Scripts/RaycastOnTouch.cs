using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RaycastOnTouch : MonoBehaviour
{
    GameManager gameManager;
    public LayerMask IgnoreMe;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    Ray ray;
    RaycastHit raycastHit;
    void Update()
    {
        if(Time.timeScale >= 1)
        {
            Vector2 tempPos;
            foreach(Touch touch in Input.touches)
            {
                tempPos = touch.position;
                if(touch.phase == TouchPhase.Began)
                {
                    ray = Camera.main.ScreenPointToRay(touch.position);
                    if(Physics.Raycast(ray, out raycastHit, 100.0f, ~IgnoreMe))
                    {
                        if (raycastHit.collider.CompareTag("Object") || raycastHit.collider.CompareTag("CorrectObject"))
                        {
                            ObjectGroupController temp = raycastHit.transform.GetComponent<ObjectGroupController>();
                            temp.Guess(raycastHit.collider.gameObject, tempPos);
                        }
                        else if(raycastHit.collider.CompareTag("Bomb") || (raycastHit.transform.CompareTag("ObjectGroup") && gameManager.bombActive))
                        {
                            //Debug.Log("Hitbomb");
                            gameManager.activeBomb.transform.GetComponent<BombScript>().Explode();
                        }
                    }
                }
            } 
        }
    }
}

//hit.transform returns the parent's transform of the hit, yet
//hit.collider returns the specific collider of the hit.