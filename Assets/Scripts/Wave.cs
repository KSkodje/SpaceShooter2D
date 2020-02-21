using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// to have it show in Unity editor
[System.Serializable]
public class Wave
{
    public int enemiesInWave;
    public float enemySpeed;
    public float enemyFireRate;

    public bool canFireAtPickups = false;
    public bool canSpawnFromSide = false;
    public bool canHaveShield = false;
    public bool canBeAgressive = false;
    public bool canFireBackwards = false;
    public bool canEvade = false;
    public bool addHarderEnemyType = false;
    //public bool isLastWave = false;

    // Pickup settings

    // constructor - named same as the class
    public Wave(int enemiesInWave, float enemySpeed, float enemyFireRate,  bool canFireAtPickups, bool canSpawnFromSide, bool canHaveShield, bool canBeAgressive, bool canFireBackwards, bool canEvade, bool addHarderEnemyType)
    {
        this.enemiesInWave = enemiesInWave;
        this.enemySpeed = enemySpeed;
        this.enemyFireRate = enemyFireRate;

        this.canFireAtPickups = canFireAtPickups;
        this.canSpawnFromSide = canSpawnFromSide;
        this.canHaveShield = canHaveShield;
        this.canBeAgressive = canBeAgressive;
        this.canFireBackwards = canFireBackwards;
        this.canEvade = canEvade;
        this.addHarderEnemyType = addHarderEnemyType;
        //this.isLastWave = isLastWave;

        // COULD add choice between randomizing or set to all spawned enemies
        // bool applyToAllEnemies; or bool randomizeAbilities;
    }
}