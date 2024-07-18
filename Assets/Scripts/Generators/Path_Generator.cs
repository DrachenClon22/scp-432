using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Path_Generator : MonoBehaviour {

    //0 == X_Section; 1 == I_Section; 2 == T_Section; 3 == Dead_Section; 4 == Corner_Section; 5 == StartRoom
    public static Dictionary<Vector3, int> positions { get; private set; } = new Dictionary<Vector3, int>();
    public static List<Vector3> poslist { get; private set; } = new List<Vector3>();
    public static int offset = 0;

    public GameObject X_Section;
    public GameObject I_Section;
    public GameObject T_Section;
    public GameObject Dead_Section;
    public GameObject Corner_Section;
    public GameObject StartRoom;

    public GameObject brokenBulb;
    public GameObject bulb;

    private static int prevRand = 0;

    //private static GameObject X_Section_static;
    private static GameObject I_Section_static;
    private static GameObject T_Section_static;
    private static GameObject Dead_Section_static;
    //private static GameObject Corner_Section_static;
    //private static GameObject StartRoom_static;

    private static GameObject brokenBulb_static;
    private static GameObject bulb_static;

    private void Awake()
    {
        positions.Clear();
        offset = 0;
    }

    private void Start()
    {
        I_Section_static = I_Section;
        //X_Section_static = X_Section;
        T_Section_static = T_Section;
        Dead_Section_static = Dead_Section;
        //Corner_Section_static = Corner_Section;

        brokenBulb_static = brokenBulb;
        bulb_static = bulb;

        offset = (int)X_Section.GetComponent<BoxCollider>().size.x;

        positions.Add(Instantiate(StartRoom, transform.position, StartRoom.transform.rotation).transform.position, 5);
        transform.position += Vector3.left * offset;
        positions.Add(Instantiate(I_Section, transform.position, I_Section.transform.rotation).transform.position, 1);
        transform.position += Vector3.left * offset;
        positions.Add(Instantiate(T_Section, transform.position, T_Section.transform.rotation).transform.position, 2);
    }

    public static void GenerateAt(Vector3 targetPosition)
    {

    }

    public static void GenerateAround(Vector3 target, Quaternion rotation)
    {
        int rand = Random.Range(3, 6);
        if (prevRand == rand)
        {
            rand += Random.Range(1, 3);
        }
        prevRand = rand;

        if (rotation.eulerAngles.y == 0 || rotation.eulerAngles.y == 180)
        {
            if (!positions.ContainsKey(target + Vector3.forward * offset))
            {
                if (positions[target] == 2 || positions[target] == 4)
                {
                    for (int i = 1; i < rand; i++)
                    {
                        if (positions.ContainsKey(target + Vector3.forward * offset * (i + 1)))
                        {
                            SpawnSection(Dead_Section_static, target + Vector3.forward * offset * i, Quaternion.Euler(-90, 90, 0), 3);
                            break;
                        }

                        if (i == rand - 1)
                        {
                            SpawnSection(T_Section_static, target + Vector3.forward * offset * i, Quaternion.Euler(-90, 90, 0), 2);
                            break;
                        }

                        SpawnSection(I_Section_static, target + Vector3.forward * offset * i, Quaternion.Euler(-90, 90, 0), 1);
                    }
                }
            }

            if (!positions.ContainsKey(target - Vector3.forward * offset))
            {
                if (positions[target] == 2 || positions[target] == 4)
                {
                    for (int i = 1; i < rand; i++)
                    {
                        if (positions.ContainsKey(target - Vector3.forward * offset * (i + 1)))
                        {
                            SpawnSection(Dead_Section_static, target - Vector3.forward * offset * i, Quaternion.Euler(-90, -90, 0), 3);
                            break;
                        }

                        if (i == rand - 1)
                        {
                            SpawnSection(T_Section_static, target - Vector3.forward * offset * i, Quaternion.Euler(-90, -90, 0), 2);
                            break;
                        }

                        SpawnSection(I_Section_static, target - Vector3.forward * offset * i, Quaternion.Euler(-90, -90, 0), 1);
                    }
                }
            }
        }

        if (rotation.eulerAngles.y == 270 || rotation.eulerAngles.y == 90)
        {
            if (!positions.ContainsKey(target + Vector3.right * offset))
            {
                if (positions[target] == 2 || positions[target] == 4)
                {
                    for (int i = 1; i < rand; i++)
                    {
                        if (positions.ContainsKey(target + Vector3.right * offset * (i + 1)))
                        {
                            SpawnSection(Dead_Section_static, target + Vector3.right * offset * i, Quaternion.Euler(-90, 180, 0), 3);
                            break;
                        }

                        if (i == rand - 1)
                        {
                            SpawnSection(T_Section_static, target + Vector3.right * offset * i, Quaternion.Euler(-90, 180, 0), 2);
                            break;
                        }

                        SpawnSection(I_Section_static, target + Vector3.right * offset * i, Quaternion.Euler(-90, 180, 0), 1);
                    }
                }
            }

            if (!positions.ContainsKey(target - Vector3.right * offset))
            {
                if (positions[target] == 2 || positions[target] == 4)
                {
                    for (int i = 1; i < rand; i++)
                    {
                        if (positions.ContainsKey(target - Vector3.right * offset * (i + 1)))
                        {
                            SpawnSection(Dead_Section_static, target - Vector3.right * offset * i, Quaternion.Euler(-90, 0, 0), 3);
                            break;
                        }

                        if (i == rand - 1)
                        {
                            SpawnSection(T_Section_static, target - Vector3.right * offset * i, Quaternion.Euler(-90, 0, 0), 2);
                            break;
                        }

                        SpawnSection(I_Section_static, target - Vector3.right * offset * i, Quaternion.Euler(-90, 0, 0), 1);
                    }
                }
            }
        }
    }

    private static void SpawnSection(GameObject toSpawn, Vector3 position, Quaternion rotation, int index)
    {
        GameObject go = Instantiate(toSpawn, position, rotation);

        if (Random.Range(0, 100)%2 == 0)
        {
            int rand = Random.Range(0, go.transform.childCount);

            Transform tr = go.transform.GetChild(rand).transform;
            Instantiate(Random.Range(0, 100) % 2 == 0 ? bulb_static : brokenBulb_static, tr.position, tr.rotation, tr.parent);
        }
        positions.Add(go.transform.position, index);
        poslist.Add(go.transform.position);
    }
}
