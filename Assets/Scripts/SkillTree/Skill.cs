using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Skill", menuName = "ScriptableObjects/SkillScriptableObject", order = 1)]
public class Skill :ScriptableObject
{
    public string skillName;

    private bool learned = false;

    private bool locked = false;

    private int cost;

    public Skill prereqSkill;

    public List<Skill> childSkills;

    public Skill(string myName, int myCost = 1, bool startLearned = false)
    {
        skillName = myName;
        cost = myCost;
        learned = startLearned;
        prereqSkill = null;
        childSkills = new List<Skill>();
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

    public bool isLearned()
    {
        return learned;
    }

    public bool isLocked(bool otherReqs = true)
    {
        if (!otherReqs)
            return false;
        if (prereqSkill.isLearned())
        {
            return false;
        }

        return true;

    }

    public Skill GetPrereqSkill()
    {
        return prereqSkill;
    }
    public List<Skill> GetChildSkills()
    {
        return childSkills;
    }

    #endregion
    public bool Learn()
    {
        if(!isLocked())
            learned = true;
        return learned;            
    }

    public void Forget()
    {
        learned = false;
    }

    public void AddChild(Skill child)
    {
        childSkills.Add(child);
        child.prereqSkill = this;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="newName"></param>
    /// <param name="newCost"></param>
    /// <param name="newLearned">This should be true if this skill starts out learned</param>
    public void Edit(string newName, int newCost, bool newLearned)
    {
        skillName = newName;
        cost = newCost;
        learned = newLearned;
    }
}
