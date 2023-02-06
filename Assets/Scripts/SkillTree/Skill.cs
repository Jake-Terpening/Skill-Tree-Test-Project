using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


[CreateAssetMenu(fileName = "Skill", menuName = "ScriptableObjects/SkillScriptableObject", order = 1)]
public class Skill :ScriptableObject
{
    public string skillName;

    public static string defaultFilePath = "Assets/NewSkill.asset";

    [SerializeField]
    private int cost;

    public Skill(string myName, int myCost =1)
    {
        skillName = myName;
        cost = myCost;
    }


    #region getters
    public string GetName()
    {
        return skillName;
    }

    public int GetCost()
    {
        return cost;
    }
    public void Edit(string newName)
    {
        skillName = newName;
    }

    #endregion
}
