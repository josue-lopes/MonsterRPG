using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager
{
    private static BattleManager instance = null;

    private Dictionary<MonsterType, MonsterType> m_StrengthMasks;   //stores which types the specific key type is strong against in bitmasks
    private Dictionary<MonsterType, MonsterType> m_WeaknessMasks;   //stores which types the specific key type is weak against in bitmasks

    //singleton implementation
    public static BattleManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new BattleManager();
            }

            return instance;
        }
    }

    private BattleManager() { Init(); }

    private void Init()
    {
        m_StrengthMasks = new Dictionary<MonsterType, MonsterType>();
        m_WeaknessMasks = new Dictionary<MonsterType, MonsterType>();

        //initialize each type with their strengths / weaknesses
        #region Init Type Strengths / Weaknesses
        
        //Normal
        m_StrengthMasks.Add(MonsterType.NORMAL, MonsterType.NONE);
        m_WeaknessMasks.Add(MonsterType.NORMAL, MonsterType.ROCK | MonsterType.STEEL);

        //Fire
        m_StrengthMasks.Add(MonsterType.FIRE, MonsterType.GRASS | MonsterType.ICE | MonsterType.BUG | MonsterType.STEEL);
        m_WeaknessMasks.Add(MonsterType.FIRE, MonsterType.FIRE | MonsterType.WATER | MonsterType.ROCK | MonsterType.DRAGON);

        //Water
        m_StrengthMasks.Add(MonsterType.WATER, MonsterType.FIRE | MonsterType.GROUND | MonsterType.ROCK);
        m_WeaknessMasks.Add(MonsterType.WATER, MonsterType.WATER | MonsterType.GRASS | MonsterType.DRAGON);

        //Electric
        m_StrengthMasks.Add(MonsterType.ELECTRIC, MonsterType.WATER | MonsterType.FLYING);
        m_WeaknessMasks.Add(MonsterType.ELECTRIC, MonsterType.ELECTRIC | MonsterType.GRASS | MonsterType.DRAGON);

        //Grass
        m_StrengthMasks.Add(MonsterType.GRASS, MonsterType.WATER | MonsterType.GROUND | MonsterType.ROCK);
        m_WeaknessMasks.Add(MonsterType.GRASS, MonsterType.FIRE | MonsterType.GRASS | MonsterType.POISON | MonsterType.FLYING | MonsterType.BUG | MonsterType.DRAGON | MonsterType.STEEL);

        //Ice
        m_StrengthMasks.Add(MonsterType.ICE, MonsterType.GRASS | MonsterType.GROUND | MonsterType.FLYING | MonsterType.DRAGON);
        m_WeaknessMasks.Add(MonsterType.ICE, MonsterType.FIRE | MonsterType.WATER | MonsterType.ICE | MonsterType.STEEL);

        //Fighting
        m_StrengthMasks.Add(MonsterType.FIGHTING, MonsterType.NORMAL | MonsterType.ICE | MonsterType.ROCK | MonsterType.DARK | MonsterType.STEEL);
        m_WeaknessMasks.Add(MonsterType.FIGHTING, MonsterType.POISON | MonsterType.FLYING | MonsterType.PSYCHIC | MonsterType.BUG);

        //Poison
        m_StrengthMasks.Add(MonsterType.POISON, MonsterType.GRASS);
        m_WeaknessMasks.Add(MonsterType.POISON, MonsterType.POISON | MonsterType.GROUND | MonsterType.ROCK | MonsterType.GHOST);

        //Ground
        m_StrengthMasks.Add(MonsterType.GROUND, MonsterType.FIRE | MonsterType.ELECTRIC | MonsterType.POISON | MonsterType.ROCK | MonsterType.STEEL);
        m_WeaknessMasks.Add(MonsterType.GROUND, MonsterType.GRASS | MonsterType.BUG);

        //Flying
        m_StrengthMasks.Add(MonsterType.FLYING, MonsterType.GRASS | MonsterType.FIGHTING | MonsterType.BUG);
        m_WeaknessMasks.Add(MonsterType.FLYING, MonsterType.ELECTRIC | MonsterType.ROCK | MonsterType.STEEL);

        //Psychic
        m_StrengthMasks.Add(MonsterType.PSYCHIC, MonsterType.FIGHTING | MonsterType.POISON);
        m_WeaknessMasks.Add(MonsterType.PSYCHIC, MonsterType.PSYCHIC | MonsterType.STEEL);

        //Bug
        m_StrengthMasks.Add(MonsterType.BUG, MonsterType.GRASS | MonsterType.PSYCHIC | MonsterType.DARK);
        m_WeaknessMasks.Add(MonsterType.BUG, MonsterType.FIRE | MonsterType.FIGHTING | MonsterType.POISON | MonsterType.FLYING | MonsterType.POISON | MonsterType.FLYING | MonsterType.GHOST | MonsterType.STEEL);

        //Rock
        m_StrengthMasks.Add(MonsterType.ROCK, MonsterType.FIRE | MonsterType.ICE | MonsterType.FLYING | MonsterType.BUG);
        m_WeaknessMasks.Add(MonsterType.ROCK, MonsterType.FIGHTING | MonsterType.GROUND | MonsterType.STEEL);

        //Ghost
        m_StrengthMasks.Add(MonsterType.GHOST, MonsterType.PSYCHIC | MonsterType.GHOST);
        m_WeaknessMasks.Add(MonsterType.GHOST, MonsterType.DARK);

        //Dragon
        m_StrengthMasks.Add(MonsterType.DRAGON, MonsterType.DRAGON);
        m_WeaknessMasks.Add(MonsterType.DRAGON, MonsterType.STEEL);

        //Dark
        m_StrengthMasks.Add(MonsterType.DARK, MonsterType.PSYCHIC | MonsterType.GHOST);
        m_WeaknessMasks.Add(MonsterType.DARK, MonsterType.FIGHTING | MonsterType.DARK);

        //Steel
        m_StrengthMasks.Add(MonsterType.STEEL, MonsterType.ICE | MonsterType.ROCK);
        m_WeaknessMasks.Add(MonsterType.STEEL, MonsterType.FIRE | MonsterType.WATER | MonsterType.ELECTRIC | MonsterType.STEEL);
        
        #endregion
    }

    private double CalculateDamageMultiplier(MonsterType attackType, MonsterType defendType)
    {
        //lookup strength and weakness bitmasks for attacking type
        MonsterType strengthMask = m_StrengthMasks[attackType];
        MonsterType weaknessMask = m_WeaknessMasks[attackType];

        //check whether the defending type matches any of the masks, otherwise damage stays the same
        if ((strengthMask & defendType) != 0) 
            return 2.0;
        else if ((weaknessMask & defendType) != 0)
            return 0.5;
        else
            return 1.0;
    }
}

[Flags]
public enum MonsterType
{
    NONE =     0x0,
    NORMAL =   0x01,
    FIRE =     0x02,
    WATER =    0x04,
    ELECTRIC = 0x08,
    GRASS =    0x10,
    ICE =      0x20,
    FIGHTING = 0x40,
    POISON =   0x80,
    GROUND =   0x100,
    FLYING =   0x200,
    PSYCHIC =  0x400,
    BUG =      0x800,
    ROCK =     0x1000,
    GHOST =    0x2000,
    DRAGON =   0x4000,
    DARK =     0x8000,
    STEEL =    0x10000
}
