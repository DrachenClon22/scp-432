#pragma warning disable

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Infinite_Ground : MonoBehaviour {

    private TerrainData terrain;
    public Terrain terrainitself;

    public GameObject nightSky;

    private Material material;

    public Transform player;

    private Vector2 textOffset;

    private List<TreeInstance> trees = new List<TreeInstance>();
    private List<Vector3> treesPositions = new List<Vector3>();

    private void Start()
    {
        material = nightSky.GetComponent<Renderer>().material;
        textOffset = material.mainTextureScale;
        terrain = terrainitself.terrainData;

        for (int i = 0; i < terrain.treeInstances.Length; i++)
            treesPositions.Add(terrain.treeInstances[i].position);
    }

    private void Update()
    {
        // terrain texture offset
        terrainitself.transform.position = new Vector3(player.position.x - 50f, terrainitself.transform.position.y, player.position.z - 50f);

        SplatPrototype[] sp = new SplatPrototype[1];
        sp[0] = new SplatPrototype();
        sp[0].texture = terrain.splatPrototypes[0].texture;
        sp[0].tileSize = Vector2.one * 2f;
        sp[0].tileOffset = new Vector3(player.position.x, player.position.z, 0);

        terrain.splatPrototypes = sp;
        // trees on terrain offset
        for (int i = 0; i < terrain.treeInstances.Length; i++)
        {
            TreeInstance ti = terrain.treeInstances[i];
            ti.position = new Vector3(treesPositions[i].x - terrainitself.transform.position.x / terrain.size.x, 0f, treesPositions[i].z - terrainitself.transform.position.z / terrain.size.z);

            if (ti.position.z > 1f)
            {
                treesPositions[i] = new Vector3(treesPositions[i].x, 0f, treesPositions[i].z - 1f);

                ti.position = new Vector3(ti.position.x, 0f, 0f);
            } else if (ti.position.z < 0f)
            {
                treesPositions[i] = new Vector3(treesPositions[i].x, 0f, treesPositions[i].z + 1f);

                ti.position = new Vector3(ti.position.x, 0f, 1f);
            }

            if (ti.position.x > 1f)
            {
                treesPositions[i] = new Vector3(treesPositions[i].x - 1f, 0f, treesPositions[i].z);

                ti.position = new Vector3(0f, 0f, ti.position.z);
            }
            else if (ti.position.x < 0f)
            {
                treesPositions[i] = new Vector3(treesPositions[i].x + 1f, 0f, treesPositions[i].z);

                ti.position = new Vector3(1, 0f, ti.position.z);
            }

            trees.Add(ti);
            terrain.treeInstances[i] = ti;
        }
        terrain.treeInstances = trees.ToArray();
        trees.RemoveRange(0, trees.Count);
        
        // sky texture offset
        material.SetTextureOffset("_MainTex", new Vector3((-player.position.x + Time.time) / (100f / textOffset.x), player.position.z / (100f / textOffset.y), 0));
    }
}
