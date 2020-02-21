using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab = null;
    [SerializeField]
    private GameObject _newEnemyPrefab = null;
    [SerializeField]
    private GameObject _boss = null;
    [SerializeField]
    private GameObject _enemyContainer = null;
    [SerializeField]
    private GameObject[] powerups = null;
    [SerializeField]
    private GameObject[] _playerAids= null;
    [SerializeField]
    private float _PctChanceAmmo = 66.6f;
    [SerializeField]
    private bool _stopSpawning = false;
    private bool _waveFinished = false;
    

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine("SpawnPowerupRoutine");
        StartCoroutine(SpawnPlayerAidRoutine());
    }

    /*
     * Check for collision before instantiating
     * Is there a good way to check for collision before instantiating an object? I am creating a spawn, but I don't want to create an object at a certain point if one is already occupying the space.
     * 
     * There's a bunch of ways to do this, you could:
     * Shoot a raycast up from the spawn point, and see if it hits anything
     * Use Physics.OverlapSphere at the spawn point to see if there are any objects in a sphere around the spawn point
     * Attach a collider to the spawn point, make it a trigger, and see if anything is tripping the trigger
     */
    private IEnumerator SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(3.0f);
        while (!_stopSpawning)
        {
            // sideways < -11 or 11 >
            // float direction = l/r
            //Vector3 posYtoSpawn = new Vector3(0, Random.Range(-5f, 5f), 0);
            Vector3 posToSpawn = new Vector3(Random.Range(-9f, 9f), 7, 0);
            GameObject newEnemy = Instantiate(_enemyPrefab, posToSpawn, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
                
            float waitTime = Random.Range(1.0f, 3.0f);
            yield return new WaitForSeconds(waitTime);
        }
    }


    public void StartWave(int enemiesInWave, float enemySpeed, float enemyFireRate, bool canFireAtPickups, bool canSpawnFromSide, bool canHaveShield, bool canBeAgressive, bool canFireBackwards, bool canEvade, bool addHarderEnemyType, bool isLastWave)
    {
        // Start powerups and playeraids
        StartCoroutine(SpawnPowerupRoutine());
        StartCoroutine(SpawnPlayerAidRoutine());
        StartCoroutine(EnemyWave(enemiesInWave, enemySpeed, enemyFireRate, canFireAtPickups, canSpawnFromSide, canHaveShield, canBeAgressive, canFireBackwards, canEvade, addHarderEnemyType, isLastWave));
    }

    private IEnumerator EnemyWave(int enemiesInWave, float enemySpeed, float enemyFireRate, bool canFireAtPickups, bool canSpawnFromSide, bool canHaveShield, bool canBeAgressive, bool canFireBackwards, bool canEvade, bool addHarderEnemyType, bool isLastWave)
    {
        // Wait 3 seconds from wave start to spawning first enemy
        yield return new WaitForSeconds(3.0f);
        int enemyCount = 0;
        _waveFinished = false;
        while(enemyCount < enemiesInWave)
        {
            char spawnDir;
            Vector3 posToSpawn;
            if (canSpawnFromSide)
            {
                spawnDir = CheckIfSpawnSideways();
                if (spawnDir == 'l')
                {
                    posToSpawn = new Vector3(-11.5f, Random.Range(-9f, 9f), 0);
                }
                else if (spawnDir == 'r')
                {
                    posToSpawn = new Vector3(11.5f, Random.Range(-9f, 9f), 0);
                }
                else
                {
                    spawnDir = 't';
                    posToSpawn = new Vector3(Random.Range(-9f, 9f), 7, 0);
                }
            }
            else
            {
                spawnDir = 't';
                posToSpawn = new Vector3(Random.Range(-9f, 9f), 7, 0);
            }

            if (addHarderEnemyType && FlipCoin() == true)
            {

                GameObject newEnemy = Instantiate(_newEnemyPrefab, posToSpawn, Quaternion.identity);
                newEnemy.transform.parent = _enemyContainer.transform;
            }
            else
            {
                GameObject newEnemy = Instantiate(_enemyPrefab, posToSpawn, Quaternion.identity);
                newEnemy.transform.parent = _enemyContainer.transform;
                //newEnemy.GetComponent<Enemy>().SetAbilities(enemySpeed, enemyFireRate, canFireAtPickups, canSpawnFromSide, canHaveShield, canBeAgressive, canFireBackwards, canEvade);
                newEnemy.GetComponent<Enemy>().SetAbilities(enemySpeed, enemyFireRate, canFireAtPickups, spawnDir, canHaveShield, canBeAgressive, canFireBackwards, canEvade);
            }
            
            enemyCount++;
            float waitTime = Random.Range(1.0f, 3.0f);
            yield return new WaitForSeconds(waitTime);
        }
        _waveFinished = true;

        if (enemyCount == enemiesInWave && isLastWave)
        {
            yield return new WaitUntil(AllEnemiesDead);

            // For some reason UI manager will not be loaded when instantiated as prefab all of a sudden
            _boss.SetActive(true);
        }
        else
        {
            yield return new WaitUntil(AllEnemiesDead);
            StopCoroutine("EnemyWave");
        }
    }

    private bool FlipCoin()
    {
        int flip = Random.Range(0, 10);
        if (flip < 5)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private char CheckIfSpawnSideways()
    {
        // 40% chance to spawn sideways
        int randDir = Random.Range(0, 10);
        if (randDir < 7)
        {
            // top
            return 't';
        }
        else
        {
            if (FlipCoin())
            {
                return 'l';
            }
            else
            {
                return 'r';
            }
        }
    }

    public bool AllEnemiesDead()
    {
        if (_enemyContainer.transform.childCount == 0 && _waveFinished == true)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private IEnumerator SpawnPowerupRoutine()
    {
        int _powerUpCounter = 0;
        yield return new WaitForSeconds(4.0f);
        while (!_stopSpawning)
        {
            float waitForTime = Random.Range(5.0f, 11.0f);
            yield return new WaitForSeconds(waitForTime);
            Vector3 posToSpawn = new Vector3(Random.Range(-9f, 9f), 7, 0);
            int randomPowerup = Random.Range(0, 4);
            if(_powerUpCounter > 15)
            {
                // can spawn One of Two different homing 
                randomPowerup = Random.Range(0, 6);
            }
            if (randomPowerup >= 4)
            {
                _powerUpCounter = 0;
            }
            else
            {
                _powerUpCounter++;
            }
            Instantiate(powerups[randomPowerup], posToSpawn, Quaternion.identity);
        }
    }

    private IEnumerator SpawnPlayerAidRoutine()
    {
        while (!_stopSpawning)
        {
            // PlayerAids: 0=Health, 1=Ammo
            float timeToWait = Random.Range(5.0f, 8.0f);
            yield return new WaitForSeconds(timeToWait);
            Vector3 posToSpawn = new Vector3(Random.Range(-9f, 9f), 7, 0);
            float randomPlayerAid = Random.Range(0, 100f);

            if (randomPlayerAid < _PctChanceAmmo)
            {
                Instantiate(_playerAids[1], posToSpawn, Quaternion.identity);
            }
            else
            {
                Instantiate(_playerAids[0], posToSpawn, Quaternion.identity);
            }
            //int randomPlayerAid = Random.Range(0, 2);
            //Instantiate(_playerAids[randomPlayerAid], posToSpawn, Quaternion.identity);
        }
    }


    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }

    public void RandomEnemySelfDestruct()
    {
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

    void WaveRoutine()
    {
        // X number of Waves
        // X number of enemies per wave
        // Tweak speed and fireRate and increase with each wave
        // When all enemies in last wave defeated - Spawn Boss
        // When Boss spawns - have healthbar appear on top of the scene
        // add chance of more advanced enemies after the first wave has been defeated

    }
}
