using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameResources : MonoBehaviour
{

    private static GameResources instance;

    [SerializeField]
    private GameObject pVoronoiMeshObject;

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogWarning("Tried to create a second GameResources singleton.");
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static GameResources GET_INSTANCE()
    {
        return instance;
    }

    public GameObject GetVoronoiMeshObject()
    {
        return this.pVoronoiMeshObject;
    }

}
