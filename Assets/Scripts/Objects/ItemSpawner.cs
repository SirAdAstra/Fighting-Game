using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public bool isEmpty;

    [SerializeField] private GameObject sword;
    [SerializeField] private GameObject pistol;
    [SerializeField] private GameObject rifle;
    [SerializeField] private GameObject spawnPoint;

    private float spawnTimerStart = 30f;
    private float spawnTimer;

    private void Start()
    {
        spawnTimer = spawnTimerStart;
        Spawn();
    }

    private void Update()
    {
        if (isEmpty)
        {
            if (spawnTimer <= 0)
            {
                Spawn();
                spawnTimer = spawnTimerStart;
            }
            else
            {
                spawnTimer -= Time.deltaTime;
            }
        }
    }

    private void Spawn()
    {
        int rand = Random.Range(0, 3);

        switch(rand)
        {
            case 0:
                Instantiate(sword, spawnPoint.transform.position, Quaternion.identity);
                break;
            
            case 1:
                Instantiate(pistol, spawnPoint.transform.position, Quaternion.identity);
                break;

            case 2:
                Instantiate(rifle, spawnPoint.transform.position, Quaternion.identity);
                break;
        }

        isEmpty = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Sword" || other.tag == "Pistol" || other.tag == "Rifle")
            isEmpty = true;
    }
}
