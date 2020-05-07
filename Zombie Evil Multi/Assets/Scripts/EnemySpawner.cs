using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemy;
    public GameObject spawnPoint;
    public int enemyMultiplier;
    [HideInInspector]
    public List<SpawnPoint> enemySpawnPoints;

    int[] enemyCount = new int[3];
    int totalEnemies = 0;

    public int enemiesLeft;

    void Start()
    {
        enemyCount[0] = enemyMultiplier * 5;
        enemyCount[1] = enemyMultiplier * 3;
        enemyCount[2] = enemyMultiplier;
        totalEnemies = enemyMultiplier * 9;
        enemiesLeft = totalEnemies;
        for (int i = 0; i <= totalEnemies; i++)
        {
            var spawnPosition = new Vector2(Random.Range(-10f, 10f), Random.Range(-10f, 10f));
            SpawnPoint enemySpawnPoint = (Instantiate(spawnPoint, spawnPosition, Quaternion.identity) as GameObject).GetComponent<SpawnPoint>();
            enemySpawnPoints.Add(enemySpawnPoint);
        }
    }

    public void SpawnEnemies(ConnectionManager.EnemiesJSON enemiesJSON)
    {
        int i = 0;
        int j = 0;
        foreach (ConnectionManager.UserJSON enemyJSON in enemiesJSON.enemies)
        {
            Vector2 position = new Vector2(enemyJSON.position[0], enemyJSON.position[1]);
            GameObject newEnemy;
            if (i < enemyCount[0])
            {
                newEnemy = Instantiate(enemy[0], position, Quaternion.identity) as GameObject;
                i++;
            }
            else if (j < enemyCount[1])
            {
                newEnemy = Instantiate(enemy[1], position, Quaternion.identity) as GameObject;
                j++;
            }
            else
            {
                newEnemy = Instantiate(enemy[2], position, Quaternion.identity) as GameObject;
            }
            
            newEnemy.name = enemyJSON.name;
        }
    }
}
