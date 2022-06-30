using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

public class GenerateLevel : MonoBehaviour
{
    [SerializeField] private int _depth = 3;
    [SerializeField] private int _height = 50;
    [SerializeField] private int _width = 70;
    [SerializeField] private int _holesQty = 30;
    [SerializeField] private float _maxDistanceFromHole = 5.29f;
    private List<Vector2> _holes = new List<Vector2>();
    private GameObject _blockPrefab;
    
    //Load level base (water and land)
    
    //generate level with enough space for the worms
    
    
    void Awake()
    {
        _holes = new List<Vector2>();
        _blockPrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Destructible.prefab", typeof(GameObject));
        DrawLevel();
    }

    void SpawnWorm(ref Vector2 pos)
    {
        GameObject newWorm = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Worm.prefab", typeof(GameObject));
        Instantiate(newWorm, new Vector3(pos.x, pos.y, 1), new Quaternion(0, 1 ,0 , 1));
    }

    void DrawLevel()
    {
        _holes = new List<Vector2>();
        
        int wormsQty = 8;
        int wormsCtr = 0;

        for (int h = 0; h < _holesQty; ++h)
        {
            Vector2 newHole = new Vector2();
            newHole.x = Random.Range(1, _width - 1);
            newHole.y = Random.Range(1, _height - 1);
            _holes.Add(newHole);

            if (h * 0.125f >= wormsCtr)
            {
                SpawnWorm(ref newHole);
                ++wormsCtr;
            }
        }

        for (int h = 0; h < _height; ++h)
        {
            for (int w = 0; w < _width; ++w)
            {
                for (int d = 0; d < _depth; ++d)
                {
                    bool inHole = false;
                    for (int ho = 0; ho < _holes.Count; ++ho)
                    {
                        Vector2 distanceFromHole = new Vector2(w, h) - _holes[ho];
                        inHole = distanceFromHole.magnitude < _maxDistanceFromHole ? true : inHole;
                    }

                    if (!inHole)
                    {
                        GameObject newBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        newBlock.transform.position = new Vector3(w, h, d);
                    }
                }
            }
        }
    }
}
