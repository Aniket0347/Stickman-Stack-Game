using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    public GameObject prefab;
    public float spawnInterval = 10f;
    private GameObject Stopper;
    private Stack stackComp;
    public List<GameObject> spawnedObjects = new List<GameObject>();
    public GameObject canvas;
    public bool Check_interval = false;

    private const float MIN_INTERVAL = 1.5f;

    // Set these in Inspector to exact world X where prefab appears
    public float spawnRightX = 5f;
    public float spawnLeftX = -5f;

    // Set these in Inspector to exact world X where stopper should sit for each side
    public float stopperXFromRight = 2f;
    public float stopperXFromLeft = -2f;

    void Start()
    {
        Stopper = GameObject.FindWithTag("Use");
        stackComp = Stopper.GetComponent<Stack>();
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

    void Start_Spawn()
    {
        StartCoroutine(SpawnRoutine());
    }

    void Awake()
    {
        Invoke("Start_Spawn", 0.1f); // slight delay so Start() finishes first
    }

    void Spawner()
    {
        // Clean up objects that went off either side
        spawnedObjects.RemoveAll(obj => obj == null);
        foreach (GameObject obj in spawnedObjects)
        {
            if (obj != null && (obj.transform.position.x < spawnLeftX - 5f || obj.transform.position.x > spawnRightX + 5f))
                Destroy(obj);
        }
        spawnedObjects.RemoveAll(obj => obj == null);

        // Randomly pick a side
        bool spawnFromRight = Random.value > 0.5f;

        float spawnX = spawnFromRight ? spawnRightX : spawnLeftX;
        float direction = spawnFromRight ? 1f : -1f;

        if (spawnFromRight)
        {
            Debug.Log("Coming from Right");

            // Move stopper to right position
            if (Stopper != null)
            {
                Vector3 pos = Stopper.transform.position;
                Stopper.transform.position = new Vector3(stopperXFromRight, pos.y, pos.z);
            }

            // Tell Stack to reset to this X after stacking
            if (stackComp != null)
            {
                stackComp.currentStopperX0 = stopperXFromRight;
                stackComp.currentStopperX1 = stopperXFromRight;
            }
        }
        else
        {
            Debug.Log("Coming from Left");

            // Move stopper to left position
            if (Stopper != null)
            {
                Vector3 pos = Stopper.transform.position;
                Stopper.transform.position = new Vector3(stopperXFromLeft, pos.y, pos.z);
            }

            // Tell Stack to reset to this X after stacking
            if (stackComp != null)
            {
                stackComp.currentStopperX0 = stopperXFromLeft;
                stackComp.currentStopperX1 = stopperXFromLeft;
            }
        }

        Vector3 spawnPos = new Vector3(spawnX, transform.position.y, transform.position.z);
        GameObject newObj = Instantiate(prefab, spawnPos, Quaternion.identity);

        Move moveComp = newObj.GetComponent<Move>();
        if (moveComp != null)
            moveComp.direction = direction;

        PrefabDirection dirComp = newObj.GetComponent<PrefabDirection>();
        if (dirComp == null)
            dirComp = newObj.AddComponent<PrefabDirection>();
        dirComp.comingFromRight = spawnFromRight;

        spawnedObjects.Add(newObj);
    }
}