using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    public GameObject prefab;
    public float spawnInterval = 10f;
    private GameObject Stopper;
    public List<GameObject> spawnedObjects = new List<GameObject>();
    public GameObject canvas;
    public bool Check_interval = false;

    
    private const float MIN_INTERVAL = 1.5f;

    void Start()
    {
        Stopper = GameObject.FindWithTag("Use");
        StartCoroutine(SpawnRoutine());
    }

    private void Update()
    {
     
        if (canvas.GetComponent<Points>().score % 2 == 0 && Check_interval == true && spawnInterval > MIN_INTERVAL)
        {
            spawnInterval = Mathf.Max(spawnInterval - 0.5f, MIN_INTERVAL);
            Check_interval = false;
        }
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            Spawner();

           
            yield return new WaitForSeconds(Mathf.Max(spawnInterval, MIN_INTERVAL));
        }
    }

    void Spawner()
    {
       
        spawnedObjects.RemoveAll(obj => obj == null);
        foreach (GameObject obj in spawnedObjects)
        {
            if (obj != null && obj.transform.position.x < -20f)
            {
                Destroy(obj);
            }
        }
        spawnedObjects.RemoveAll(obj => obj == null);

        GameObject newObj = Instantiate(prefab, this.transform.position, Quaternion.identity);
        spawnedObjects.Add(newObj);
    }
}
