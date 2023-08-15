using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using System.Reflection;
using Unity.VisualScripting;

public struct TouchHistory
{
    public TouchHistory(int id)
    {
        fingerId = id;

        began = false;
        cancelled = false;
        ended = false;
        completed = false;
        stationaryFrames = 0;
        movedFrames = 0;
        startPos = Vector2.zero;
        currentPos = Vector2.zero;
        endPos = Vector2.zero;
        startTime = Time.time;
        duration = 0;
    }
    public bool began, cancelled, ended, completed;
    public int fingerId, stationaryFrames, movedFrames;
    public Vector2 startPos, currentPos, endPos;
    public float startTime, duration;
};
public class TouchManager : MonoBehaviour
{
    //touchHistories[index]is will have public data for taps, moves, and swipes
    const int touchHistorySize = 50;
    public TouchHistory[] touchHistories = new TouchHistory[touchHistorySize];
    private void Start()
    {
        for(int i = 0; i < touchHistorySize; i++)
        {
            touchHistories[i] = new TouchHistory(i);
        }
    }
    public void ClearTouchHistory(int index)
    {
        touchHistories[index].began = false;
        touchHistories[index].cancelled = false;
        touchHistories[index].ended = false;
        touchHistories[index].completed = false;
        touchHistories[index].stationaryFrames = 0;
        touchHistories[index].movedFrames = 0;
        touchHistories[index].startPos = Vector2.zero;
        touchHistories[index].currentPos = Vector2.zero;
        touchHistories[index].endPos = Vector2.zero;
        touchHistories[index].startTime = 0;
        touchHistories[index].duration = 0;
    }

    void Update()
    {
        foreach(Touch touch in Input.touches)
        {
            //if touch ended || cancelled but not completed
            if(touchHistories[touch.fingerId].completed == false && (touchHistories[touch.fingerId].ended == false || touchHistories[touch.fingerId].cancelled))
            {
                //complete it
                touchHistories[touch.fingerId].completed = true;
                //print its touch history
                PrintTouchHistory(touch.fingerId);
            }
            if (touch.phase == TouchPhase.Began)
            {
                if (touchHistories[touch.fingerId].completed)
                    ClearTouchHistory(touch.fingerId);
                touchHistories[touch.fingerId].began = true;
                //Time
                touchHistories[touch.fingerId].startTime = Time.time;
                //Position
                touchHistories[touch.fingerId].startPos = touch.position;
                touchHistories[touch.fingerId].currentPos = touch.position;
                print("Id: " + touchHistories[touch.fingerId].fingerId);
            }
            else if(touch.phase == TouchPhase.Stationary)
            {
                //Count frames here
                touchHistories[touch.fingerId].stationaryFrames++;
                print("Stationary Frames: " + touchHistories[touch.fingerId].stationaryFrames.ToString());
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                //Count frames here
                touchHistories[touch.fingerId].movedFrames++;
                //Position
                touchHistories[touch.fingerId].currentPos = touch.position;
                print("Moving Frames: " + touchHistories[touch.fingerId].movedFrames.ToString());
                print("Current Position: " + touchHistories[touch.fingerId].currentPos.x.ToString() + " " + touchHistories[touch.fingerId].currentPos.y.ToString());
            }
            else if (touch.phase == TouchPhase.Canceled)
            {
                touchHistories[touch.fingerId].cancelled = true;
                print("Cancelled");
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                touchHistories[touch.fingerId].ended = true;
                touchHistories[touch.fingerId].endPos = touch.position;
                touchHistories[touch.fingerId].duration = Time.time - touchHistories[touch.fingerId].startTime;
                print("Ended");
                print("End Position: " + touchHistories[touch.fingerId].endPos.x.ToString() + " " + touchHistories[touch.fingerId].endPos.y.ToString());

            }
        }
    }

    public void PrintTouchHistory(int index)
    {
            print("Id: " + touchHistories[index].fingerId);
            print("Began");
            print("Start Position: " + touchHistories[index].startPos.x.ToString() + " " + touchHistories[index].startPos.y.ToString());
            if(touchHistories[index].stationaryFrames > 0)
                print("Stationary Frames: " + touchHistories[index].stationaryFrames);
            if(touchHistories[index].movedFrames > 0)
                print("Moved Frames: " + touchHistories[index].movedFrames);
            if(touchHistories[index].currentPos != touchHistories[index].startPos)
                print("Current Position: " + touchHistories[index].currentPos.x.ToString() + " " + touchHistories[index].currentPos.y.ToString());
            if(touchHistories[index].ended)
            {
                print("End Position: " + touchHistories[index].endPos.x.ToString() + " " + touchHistories[index].endPos.y.ToString());
            }
            if(touchHistories[index].cancelled)
                print("Cancelled");
    }
}

/*
            //Tap
            Ray raycast = Camera.main.ScreenPointToRay(touch.position);
            RaycastouchHistories[index]it raycastouchHistories[index]it;
            if (Physics.Raycast(raycast, out raycastouchHistories[index]it))
            {
                if (raycastouchHistories[index]it.collider.CompareTag("Object"))
                {
                    raycastouchHistories[index]it.collider.GetComponent<ObjectController>().Tap();
                }
            }
*/