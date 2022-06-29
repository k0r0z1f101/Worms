using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GenerateLevel : MonoBehaviour
{
    private GameObject blockPrefab;
    
    //Load level base (water and land)
    
    //generate level with enough space for the worms
    
    
    void Awake()
    {
        blockPrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Destructible.prefab", typeof(GameObject));
        print(blockPrefab);
        SpawnWholeWall();
    }

    void SpawnWholeWall()
    {
        int depth = 1;
        int height = 7;
        int width = 10;
        
        float emptySpace = 0.5f;
        int holes = 4;

        float holeSize = (height * width * emptySpace) / holes;
        List<Vector2> occupiedByHole = new List<Vector2>();

        for (int h = 0; h < height; ++h)
        {
            for (int w = 0; w < width; ++w)
            {
                for (int d = 0; d < depth; ++d)
                {
                    GameObject newBlock = blockPrefab;
                    Instantiate(newBlock, new Vector3(w, h, d), Quaternion.identity);
                }
            }
        }

        for (int h = 0; h < holes; ++h)
        {
            int xMin = -1;
            int yMin = -1;
            int xMax = -1;
            int yMax = -1;

        }
    }
}
