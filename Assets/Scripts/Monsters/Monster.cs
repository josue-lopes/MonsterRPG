using System;
using System.Collections.Generic;
using UnityEngine;

public class Monster
{
    //stats
    private Dictionary<MonsterStat, double> m_BaseStats;    //stores base stat affiliated with specific monster and affects stat calculation
    private Dictionary<MonsterStat, int> m_EValues;         //how many effort values acculumuated for each stat
    private Dictionary<MonsterStat, int> m_IValues;         //unique random underlying values which affect stats
    private Dictionary<MonsterStat, double> m_FinalStats;   //final stats with base + evs + ivs

    private Dictionary<int, AttackMove> m_Moveset;          //currently learned moves
    private Dictionary<int, int> m_PPValues;                //how many uses each move still has
    private int m_Level = 1;                                //current level
    private double m_Experience = 0.0;                      //current amount of experience, level-up depends on experience type and level
    private MonsterType m_Type1;                            //monster types
    private MonsterType m_Type2;
    private ExperienceGroup m_ExperienceGroup;              //which experience group is the monster part of

    //battle stats
    private double m_EvasionPercentage = 1.0;               //percentage to avoid enemy attack
    private double m_AccuracyPercentage = 1.0;              //percentage to hit with attacks
    private StatusEffect m_CurrentStatus;                   //status currently 
    private double m_CurrentHP;

    //dex info
    private int m_SpeciesNumber;                            //species number index
    private string m_SpeciesName;                           //unique species name

    //Properties
    public int SpeciesNumber { get { return m_SpeciesNumber; } set { m_SpeciesNumber = value; } }
    public string SpeciesName { get { return m_SpeciesName; } set { m_SpeciesName = value; } }
    public Dictionary<MonsterStat, double> BaseStats { get { return m_BaseStats; } set { m_BaseStats = value; } }
    public MonsterType Type1 { get { return m_Type1; } set { m_Type1 = value; } }
    public MonsterType Type2 { get { return m_Type2; } set { m_Type2 = value; } }
    public ExperienceGroup ExperienceGroup { get { return m_ExperienceGroup; } set { m_ExperienceGroup = value; } }

    public Monster() { Init(); }

    //Copy constructor
    public Monster(Monster original)
    {
        Init();

        double[] originalStats = new double[6];

        foreach (MonsterStat type in Enum.GetValues(typeof(MonsterStat)))
        {
            if (type == MonsterStat.STAT_COUNT)
                break;

            originalStats[(int)type] = original.BaseStats[type];
        }

        SetUpInitialValues(original.SpeciesNumber, original.SpeciesName, originalStats, original.Type1, original.Type2, original.ExperienceGroup);
    }

    //function called during initial set up of monster in data base
    public void SetUpInitialValues(int id, string name, double[] baseStats, MonsterType type1, MonsterType type2, ExperienceGroup eGroup)
    {
        m_SpeciesNumber = id;
        m_SpeciesName = name;
        m_BaseStats[MonsterStat.HP] = baseStats[0];
        m_BaseStats[MonsterStat.ATTACK] = baseStats[1]; 
        m_BaseStats[MonsterStat.DEFENCE] = baseStats[2];
        m_BaseStats[MonsterStat.SPATTACK] = baseStats[3];
        m_BaseStats[MonsterStat.SPDEFENCE] = baseStats[4];
        m_BaseStats[MonsterStat.SPEED] = baseStats[5];
        m_Type1 = type1;
        m_Type2 = type2;
        m_ExperienceGroup = eGroup;
    }
    
    //Function called to distribute experience to monster after battle
    public void IncreaseExperience(int expAmount)
    {
        //if already max level return
        if (m_Level == 100)
            return;

        //add gained experience to experience
        m_Experience += expAmount;
        double currentLevelMax = 0.0;

        //determine which type monster is to determine if they've hit level max
        switch (m_ExperienceGroup)
        {
            case ExperienceGroup.FAST:
                currentLevelMax = 0.8 * (m_Level ^ 3);
                break;
            case ExperienceGroup.MEDIUM_FAST:
                currentLevelMax = m_Level ^ 3;
                break;
            case ExperienceGroup.MEDIUM_SLOW:
                currentLevelMax = 1.2 * (m_Level ^ 3) - 15 * (m_Level ^ 2) + 100 * m_Level - 140;
                break;
            case ExperienceGroup.SLOW:
                currentLevelMax = 1.25 * (m_Level ^ 3);
                break;
        }

        //if you've monster enough experience, move on to next level
        if (m_Experience >= currentLevelMax)
            m_Level++;
    }

    //Init any collections/variables on creation of class
    private void Init()
    {
        m_BaseStats = new Dictionary<MonsterStat, double>();
        m_EValues = new Dictionary<MonsterStat, int>();
        m_IValues = new Dictionary<MonsterStat, int>();
        m_FinalStats = new Dictionary<MonsterStat, double>();
        m_Moveset = new Dictionary<int, AttackMove>();
        m_PPValues = new Dictionary<int, int>();
    }

    //Function called to update stats after level has changed
    private void UpdateStats()
    {
        //Calculate HP first as it uses a different formula
        m_FinalStats[MonsterStat.HP] =
            ((m_BaseStats[MonsterStat.HP] + m_IValues[MonsterStat.HP]) * 2 + (Mathf.Sqrt(m_EValues[MonsterStat.HP]) / 4)) * m_Level / 100 + m_Level + 10;

        //Iterate through other stats and update them with slightly differnt formula
        for (MonsterStat it = MonsterStat.ATTACK; it != MonsterStat.STAT_COUNT; ++it)
        {
            m_FinalStats[it] =
                ((m_BaseStats[it] + m_IValues[it]) * 2 + (Mathf.Sqrt(m_EValues[it]) / 4)) * m_Level / 100 + 5;
        }
    }
}

public enum MonsterStat
{
    HP,
    ATTACK,
    DEFENCE,
    SPATTACK,
    SPDEFENCE,
    SPEED,
    STAT_COUNT
}

public enum ExperienceGroup
{
    FAST,
    MEDIUM_FAST,
    MEDIUM_SLOW,
    SLOW
}