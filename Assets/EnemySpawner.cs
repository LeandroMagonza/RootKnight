using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour{

    public float timeBetweenSpawns = 2;
    public float timeTillNextSpawn = 1;
    
    public GameObject enemy;

    public List<GameObject> spawnPositions;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (timeTillNextSpawn > 0) {
            timeTillNextSpawn -= Time.deltaTime;
        }
        else {
            timeTillNextSpawn = timeBetweenSpawns;
            SpawnEnemy();
        }
    }

    public void SpawnEnemy() {
        int nextSpawnPosition = Random.Range(0, spawnPositions.Count);
        Instantiate(enemy,spawnPositions[nextSpawnPosition].transform.position,quaternion.identity);
    }
    
}
