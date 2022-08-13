using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Random = UnityEngine.Random;

public class GenerateLevel : MonoBehaviour
{
    private GameController _gameController;
    private List<Vector2> _holes = new List<Vector2>();
    private GameObject _destructiblePrefab;
    private GameObject _indestructiblePrefab;
    private GameObject _blocksParent;
    
    public void DrawLevel(ref int depth, ref int height, ref int width, ref int holesQty,
        ref float maxDistanceFromHole, ref int wormsQty)
    {
        _holes = new List<Vector2>();
        //_destructiblePrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Destructible.prefab", typeof(GameObject));
        _destructiblePrefab = Resources.Load<GameObject>("Destructible") as GameObject;
        _indestructiblePrefab = Resources.Load<GameObject>("Indestructible") as GameObject;
        //_indestructiblePrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Indestructible.prefab", typeof(GameObject));
        _gameController = gameObject.GetComponent<GameController>();
        _blocksParent = new GameObject("Blocks");
        
        int wormsCtr = 1;

        for (int h = 0; h < holesQty; ++h)
        {
            Vector2 newHole = new Vector2();
            newHole.x = Random.Range(1, width - 1);
            newHole.y = Random.Range(1, height - 1);
            _holes.Add(newHole);

            float wormsCoef = 1 / ((float)h / (float)wormsQty);
            if (h * wormsCoef >= wormsCtr)
            {
                int teamSize = (int)(wormsQty * 0.5f);
                int teamNumber = wormsCtr > teamSize ? 1 : 0;
                SpawnWorm(ref newHole, ref width, teamNumber, _gameController.GetNameFromTeam(teamNumber, (wormsCtr - (teamNumber * teamSize) - 1)));
                ++wormsCtr;
            }
        }

        for (int h = 0; h < height; ++h)
        {
            for (int w = 0; w < width; ++w)
            {
                for (int d = 0; d < depth; ++d)
                {
                    bool inHole = false;
                    for (int ho = 0; ho < _holes.Count; ++ho)
                    {
                        Vector2 distanceFromHole = new Vector2(w, h) - _holes[ho];
                        inHole = distanceFromHole.magnitude < maxDistanceFromHole ? true : inHole;
                    }

                    if (!inHole)
                    {
                        GameObject newBlock = _indestructiblePrefab;
                        newBlock.transform.position = new Vector3(w, h, d);
                        Instantiate(newBlock, _blocksParent.transform);
                    }
                }
            }
        }
    }

    //test fonction
    public void ExplodeBlocks(ref Vector2 pos, ref float radius)
    {
        Collider[] blocksConverted = Physics.OverlapSphere(new Vector3(pos.x, pos.y, 1), radius);
        for (int c = 0; c < blocksConverted.Length; ++c)
        {
            GameObject newBlock = _destructiblePrefab;
            newBlock.transform.position = blocksConverted[c].transform.position;
            blocksConverted[c].gameObject.SetActive(false);
            Instantiate(newBlock);
        }
    }
    
    void SpawnWorm(ref Vector2 pos, ref int width, int teamNumber, string name)
    {
        GameObject newBlock = _indestructiblePrefab;
        newBlock.transform.position = new Vector3(pos.x, pos.y - 1, 1);
        Instantiate(newBlock, _blocksParent.transform);
        //GameObject newWorm = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Worm.prefab", typeof(GameObject));
        GameObject newWorm = Resources.Load<GameObject>("Worm");
        newWorm.name = name;
        Worm newWormScript = newWorm.GetComponent<Worm>();
        newWormScript.SetName(name);
        newWormScript.SetTeam(teamNumber);
        GameObject team = GameObject.Find("Team_" + teamNumber);
        int facing = pos.x < width * 0.5f ? 1 : -1;
        Instantiate(newWorm, new Vector3(pos.x, pos.y, 1), new Quaternion(0, facing ,0 , 1), team.transform);
    }
}
