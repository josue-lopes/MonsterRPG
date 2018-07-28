using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager
{
    private static MonsterManager instance = null;
    private Dictionary<int, Monster> m_MonsterData;     //stores the unique base data of every monster

    //singleton implementation
    public static MonsterManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new MonsterManager();
            }

            return instance;
        }
    }

    private MonsterManager() { LoadMonsterData(); }

    public Monster GetMonster(int speciesNumber)
    {
        return m_MonsterData[speciesNumber];
    }

    private void LoadMonsterData()
    {
        m_MonsterData = XMLParser.LoadMonsterData();
        Debug.Log(m_MonsterData.Count + " unique monsters loaded.");
    }
}
