using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

struct Team
{
    static private string[] NAMES = {"Daphnée", "FrançOis", "Tatiana", "Dominick", "Olivier", "Philippe", "Samuel", "FraVçois", "Sébastien", "Tyron", "Guillaume", "Christine", "FRançois", "David", "Alexis", "Antoine", "Augustin", "Haytem", "FranÇois", "Julie", "Yoan", "Marc-André"};

    private int teamID;
    private int score;
    private List<string> names;
    private GameObject teamObj;

    public Team(int teamNumber, int wormsQty)
    {
        teamID = teamNumber;
        teamObj = new GameObject("Team_" + teamNumber);
        names = new List<string>();
        for (int w = 0; w < wormsQty; ++w)
        {
            string newName = NAMES[Random.Range(0, NAMES.Length)];
            while (names.Contains(newName))
            {
                newName = NAMES[Random.Range(0, NAMES.Length)];
            }
            names.Add(newName);
        }
        score = wormsQty * 100;
    }

    public string GetName(int number)
    {
        return names[number];
    }
}