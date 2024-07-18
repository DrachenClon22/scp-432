using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpawnEnemy : MonoBehaviour {

    public bool godMode = false;

    public static bool isSpawned = false;
    public static bool doSpawn = true;

    public static Vector3 lastSpawnedPosition;

    [Space]
    public float spawnSecondsMin = 30f;
    public float spawnSecondsMax = 50f;
    public float maxDistance = 10f;
    public int requiredSections = 10;
    public int offset = 2;
    [Space]
    public GameObject prefab;

    private static GameObject spawned;

    IEnumerator Start()
    {
        doSpawn = true;
        isSpawned = false;

        if (godMode)
        {
            print("DO NOT FORGET TO REMOVE GODMODE");
        }

        if (!godMode)
        {
            while (true)
            {
                if (doSpawn)
                {
                    yield return new WaitForSeconds(Random.Range(spawnSecondsMin, spawnSecondsMax));

                    if (!PlayerController.groundTag.Equals("Concrete") && !isSpawned && Path_Generator.positions.Count > requiredSections)
                    {
                        int random = Random.Range(-3, 4);
                        random = (random == 0) ? 1 : random;

                        for (; ; )
                        {
                            if (Path_Generator.positions.ContainsKey(PlayerController.currentIntersect + Vector3.right * Path_Generator.offset * random))
                            {
                                if (Path_Generator.positions[PlayerController.currentIntersect + Vector3.right * Path_Generator.offset * random] == 5)
                                {
                                    random = Random.Range(-3, 4);
                                    random = (random == 0) ? 2 : random;
                                    continue;
                                }
                                else
                                    break;
                            }
                            else
                            {
                                random = Random.Range(-3, 4);
                                random = (random == 0) ? 2 : random;
                            }
                            yield return null;
                        }

                        if (Path_Generator.positions.ContainsKey(PlayerController.currentIntersect + Vector3.right * Path_Generator.offset * random))
                        {
                            spawned = Instantiate(prefab, PlayerController.currentIntersect + Vector3.right * Path_Generator.offset * random, Quaternion.identity);
                            lastSpawnedPosition = PlayerController.currentIntersect + Vector3.right * Path_Generator.offset * random;
                            isSpawned = true;
                        }
                    }
                }

                if (isSpawned)
                {
                    if (EnemyController.DistanceToPlayer > maxDistance)
                    {
                        Destroy(spawned);
                        spawned = null;
                        isSpawned = false;
                    }
                }

                yield return null;
            }
        } else
            yield return null;
    }

    public static void DespawnEnemy()
    {
        Destroy(spawned);
        spawned = null;
        isSpawned = false;
    }
}
