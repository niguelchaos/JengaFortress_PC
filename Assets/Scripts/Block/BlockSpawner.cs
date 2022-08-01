using UnityEngine;
using Unity.Netcode;

public class BlockSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject objectPrefab;
    [SerializeField] private int maxObjectCount = 10;


    public void SpawnObjects()
    {
        if (!IsServer) return;

        for (int i = 0; i < maxObjectCount; i++)
        {
            GameObject obj = Instantiate(objectPrefab, 
                new Vector3(Random.Range(-10,10), 10f, Random.Range(-10,10)), Quaternion.identity);
            
            obj.GetComponent<NetworkObject>().Spawn();
            // instatiate pool
        }
        
    }

    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(EditorConstants.TAG_PLAYER))
        {
            SpawnObjects();
        }
    }

}