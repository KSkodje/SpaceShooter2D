using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField]
    private Wave[] waves = null;
    // [SerializeField]
    // private bool pickups;
    [SerializeField]
    private SpawnManager spawnManager = null;

    public void StartWaves()
    {
        StartCoroutine(SendWave());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            Debug.Log("Starting waves");
            StartWaves();
        }
    }

    IEnumerator SendWave()
    {
        int i = 0;
        foreach(Wave wave in waves)
        {
            bool isLastWave = false;
            i++;
            if (i == waves.Length)
            {
                // && CheckIfWaveDone();
                Debug.Log("Last wave");
                isLastWave = true;
            }
            //enemiesInWave, enemySpeed, enemyFireRate, canFireAtPickups, canSpawnFromSide, canHaveShield, canBeAgressive, canFireBackwards, canEvade, addHarderEnemyType, isLastWave
            if (wave.enemySpeed <= 0)
            {
                wave.enemySpeed = 3f;
            }
            if (wave.enemyFireRate <= 0)
            {
                wave.enemyFireRate = 3f;
            }
            spawnManager.StartWave(wave.enemiesInWave, wave.enemySpeed, wave.enemyFireRate, wave.canFireAtPickups, wave.canSpawnFromSide, wave.canHaveShield, wave.canBeAgressive, wave.canFireBackwards, wave.canEvade, wave.addHarderEnemyType, isLastWave);
            yield return new WaitUntil(CheckIfWaveDone);
            // Could have a "popup" with Wave number?
            if (isLastWave && CheckIfWaveDone() == true)
            {
                StopCoroutine(SendWave());
            }
            
        }
    }

    bool CheckIfWaveDone()
    {
        return spawnManager.AllEnemiesDead();
    }
}
