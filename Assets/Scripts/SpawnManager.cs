using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab = null;
    [SerializeField]
    private GameObject _enemyContainer = null;
    [SerializeField]
    private GameObject[] powerups = null;
    [SerializeField]
    private GameObject[] _playerAids= null;
    [SerializeField]
    private bool _stopSpawning = false;
    

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine("SpawnPowerupRoutine");
        StartCoroutine(SpawnPlayerAidRoutine());
    }
    private IEnumerator SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(3.0f);
        while (!_stopSpawning)
        {
            Vector3 posToSpawn = new Vector3(Random.Range(-9f, 9f), 7, 0);
            GameObject newEnemy = Instantiate(_enemyPrefab, posToSpawn, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
                
            float waitTime = Random.Range(1.0f, 3.0f);
            yield return new WaitForSeconds(waitTime);
        }
    }

    private IEnumerator SpawnPowerupRoutine()
    {
        int _powerUpCounter = 0;
        // Unlimited ammo and constant firerate powerup? and explode random enemy on exit
        yield return new WaitForSeconds(4.0f);
        while (!_stopSpawning)
        {
            float waitForTime = Random.Range(3.0f, 8.0f);
            yield return new WaitForSeconds(waitForTime);
            Vector3 posToSpawn = new Vector3(Random.Range(-9f, 9f), 7, 0);
            int randomPowerup = Random.Range(0, 3);
            if(_powerUpCounter >= 15)
            {
                randomPowerup = Random.Range(0, 4);
            }
            if (randomPowerup == 3)
            {
                _powerUpCounter = 0;
            }
            Instantiate(powerups[randomPowerup], posToSpawn, Quaternion.identity);
        }
    }

    private IEnumerator SpawnPlayerAidRoutine()
    {
        while (!_stopSpawning)
        {
            float timeToWait = Random.Range(3.0f, 5.0f);
            yield return new WaitForSeconds(timeToWait);
            Vector3 posToSpawn = new Vector3(Random.Range(-9f, 9f), 7, 0);
            int randomPlayerAid = Random.Range(0, 2);
            Instantiate(_playerAids[randomPlayerAid], posToSpawn, Quaternion.identity);
        }
    }


    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }

    public void RandomEnemySelfDestruct()
    {
        // sprite and adding it to powerup routine remaining and ensuring it always picks a valid enemy from the list 

        // Falls outside of the scope of the framework assignment if used as a signle powerup.
        GameObject selected_enemy = RandomEnemySelect();
        if (selected_enemy)
        {
            selected_enemy.GetComponent<Enemy>().OnEnemyDeath(); // triggers death sequence
        }
    }

    public GameObject RandomEnemySelect()
    {
        int enemyCount = _enemyContainer.transform.childCount;
        if (enemyCount>0)
        {
            int pickRandomEnemy = Random.Range(0, enemyCount);
            GameObject selected_enemy = _enemyContainer.transform.GetChild(pickRandomEnemy).gameObject;
            return selected_enemy;
        }
        else
        {
            Debug.Log("No enemies");
            return null;
        }
    }
}
