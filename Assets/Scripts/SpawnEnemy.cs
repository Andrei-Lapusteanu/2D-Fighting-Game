using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    private const float SPAWN_START_DELAY_MIN = 0.5f;
    private const float SPAWN_START_DELAY_MAX = 5.0f;
    private const float SPAWN_WAIT_MIN = 8.0f;
    private const float SPAWN_WAIT_MAX = 16.5f;
    
    public static string SpawnerName;

    public GameObject enemy;
    private bool isFirstSpawn = true;


    // Start is called before the first frame update
    void Start()
    {
        SpawnerName = this.name;
        StartCoroutine(SpawnEnemyAction());
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator SpawnEnemyAction()
    {
        for (; ; )
            if (isFirstSpawn)
            {
                var randTime = Random.Range(SPAWN_START_DELAY_MIN, SPAWN_START_DELAY_MAX);
                Debug.Log("Start random: " + randTime);
                yield return new WaitForSeconds(randTime);
                isFirstSpawn = false;
                InstantiateEnemy();
            }
            else
            {
                var randTime = Random.Range(SPAWN_WAIT_MIN, SPAWN_WAIT_MAX);
                Debug.Log("Wait random: " + randTime);
                yield return new WaitForSeconds(randTime);
                InstantiateEnemy();
            }
    }

    private void InstantiateEnemy()
    {
        Instantiate(enemy, transform.position, new Quaternion(0, 180, 0, 0));
    }
}
