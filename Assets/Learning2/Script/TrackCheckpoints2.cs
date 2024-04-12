using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackCheckpoints2 : MonoBehaviour
{
    public static TrackCheckpoints2 instance;

    public List<Transform> checkpointSingleList;
    private int nextCheckpointSingleIndex;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        Transform checkpointsTransform = transform.Find("CheckPoints");

        checkpointSingleList = new List<Transform>();
        foreach (Transform checkpointSingleTransform in checkpointsTransform)
        {
            checkpointSingleList.Add(checkpointSingleTransform);
        }

        nextCheckpointSingleIndex = 0;
    }

    public int GetCheckPointIndex(Transform checkpointsTransform)
    {
        return checkpointSingleList.IndexOf(checkpointsTransform);
    }

    public bool PlayerThroughCheckpoint(Transform checkPointSingle)
    {
        if(checkpointSingleList.IndexOf(checkPointSingle)== nextCheckpointSingleIndex)
        {
            // 올바른 체크포인트
            Debug.Log("OK");
            nextCheckpointSingleIndex++;
            return true;    
        } else
        {
            Debug.Log("Nope");
            return false;  
        }
    }
}
