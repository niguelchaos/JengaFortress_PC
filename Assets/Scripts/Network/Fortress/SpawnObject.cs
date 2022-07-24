using Unity;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;


public class SpawnObject : NetworkBehaviour
{
    [SerializeField] private GameObject objPrefab;
    [SerializeField] private List<Vector3> objPositions;
    [SerializeField] private int objIndex;

    private void Awake()
    {

    }
    
    public void Spawn()
    {
        if (!IsServer) return;

        print("Spawning");


        GameObject obj = Instantiate(objPrefab, objPositions[objIndex], Quaternion.identity);
        if (obj.GetComponent<NetworkObject>() != null)
        {
            obj.GetComponent<NetworkObject>().Spawn();
        }
        else {
            print("ded");
        }

        if (objIndex < objPositions.Count - 1)
        {
            objIndex++;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(EditorConstants.TAG_PLAYER))
        {
            if (!IsServer) return;
            else {
                Spawn();
            }
        }
    }

}
