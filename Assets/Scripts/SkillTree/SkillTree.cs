using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillTree", menuName = "ScriptableObjects/SkillTreeScriptableObject", order = 1)]

public class SkillTree : ScriptableObject
{
    public List<Skill> skillList;
    public List<SkillConnection> connectionList;   //The key represents a skill that has prerequisites.

    private List<Skill> unlockedSkills;
    public int skillPoints;

    public STNodeLocationData nodeLocData;

    //You shouldn't need to call any of these methods in game.  They are mainly for the construction of the skill tree that should be done with the editor tool
    #region editor_based_methods
    public SkillTree(int startingPoints=0)
    {
        Debug.Log("ST constructor");
        skillList = new List<Skill>();
        connectionList = new List<SkillConnection>();
        skillPoints = startingPoints;
        unlockedSkills = new List<Skill>();
    }
    public void AddSkill(Skill skill)
    {
        if (skillList == null)
        {
            skillList = new List<Skill>();
        }
        if (connectionList == null)
        {
            Debug.Log("ADD SKILL");
            connectionList = new List<SkillConnection>();
        }
        skillList.Add(skill);
        connectionList.Add(new SkillConnection(skill));
    }

    public void RemoveSkill(Skill skill)
    {
        if (skillList == null)
        {
            return;
        }
        if (connectionList == null)
        {
            connectionList = new List<SkillConnection>();
        }
        skillList.Remove(skill);
        connectionList.Remove(GetConnection(skill));
        foreach(SkillConnection sc in connectionList)
        {
            if(sc.prerequisiteSkills.Contains(skill))
            { 
                RemoveConnection(sc.parentSkill, skill);
            }
        }
    }

    public void EditSkill(Skill skill, string name)
    {
        skill.Edit(name);
    }

    public void AddConnection(Skill parent, Skill child)
    {
        if (connectionList == null)
        {
            connectionList = new List<SkillConnection>();
        }
        SkillConnection sc = GetConnection(parent);
        if (sc != null)
        {
            if (!sc.prerequisiteSkills.Contains(child))
            {
                sc.Add(child);
            }
        }
        else
        {
            sc = new SkillConnection(parent, new List<Skill>());
            sc.Add(child);
            connectionList.Add(sc);
        }
    }

    public void RemoveConnection(SkillConnection connection)
    {
        if (connectionList == null)
        {
            return;
        }
        if(connectionList.Contains(connection))
        {
            connectionList.Remove(connection);
        }
    }

    public void RemoveConnection(Skill parent, Skill prereq)
    {
        if (connectionList == null)
        {
            return;
        }
        if (GetConnection(parent)!=null)
        {
            SkillConnection sc = GetConnection(parent);
            sc.Remove(prereq);
            if(sc.IsEmpty())
            {
                connectionList.Remove(sc);
            }
        }
    }

    public void AddLocDataEntry(Skill mySkill, Vector2 myPosition)
    {
        if (nodeLocData == null)
        {
            nodeLocData = new STNodeLocationData(this);
        }
        nodeLocData.AddEntry(mySkill,myPosition);
    }
    public void AddLocDataEntry(STNodeLocationDataEntry entry)
    {
        if(nodeLocData == null)
        {
            nodeLocData = new STNodeLocationData(this);
        }
        nodeLocData.AddEntry(entry);
    }
    #endregion

    //These are the methods you should be using to update your skill tree in game
    #region in_game_methods

    /// <summary>
    /// Unlocks a skill.  Checks if it is already unlocked, if it is in the skill list, if there are enough skill points, and if all prerequisites are unlocked
    /// </summary>
    /// <param name="mySkill">the skill to unlock</param>
    /// <returns>true if skill was unlocked, false otherwise</returns>
    public bool UnlockSkill(Skill mySkill)
    {
        if(unlockedSkills.Contains(mySkill))
        {
            return false;
        }
        if (!skillList.Contains(mySkill)) 
        {
            return false;
        }
        if(skillPoints < mySkill.GetCost())
        {
            return false;
        }
        if(!ArePrequisitesMet(mySkill))
        {
            return false;
        }
        unlockedSkills.Add(mySkill);
        skillPoints -= mySkill.GetCost();
        return true;
    }

    /// <summary>
    /// Unlocks a skill.  Checks if it is already unlocked, if it is in the skill list, if there are enough skill points, and if all prerequisites are unlocked
    /// </summary>
    /// <param name="mySkill">the skill to unlock</param>
    /// <returns>true if skill was unlocked, false otherwise</returns>
    public bool UnlockSkill(string mySkillName)
    {
        Skill mySkill = GetSkill(mySkillName);
        if(mySkill == null)
        {
            return false;
        }
        if (unlockedSkills.Contains(mySkill))
        {
            return false;
        }
        if (!skillList.Contains(mySkill))
        {
            return false;
        }
        if (skillPoints < mySkill.GetCost())
        {
            return false;
        }
        if (!ArePrequisitesMet(mySkill))
        {
            return false;
        }
        unlockedSkills.Add(mySkill);
        skillPoints -= mySkill.GetCost();
        return true;
    }

    /// <summary>
    /// Unlocks a skill without making the proper checks
    /// </summary>
    /// <param name="mySkill">the skill to unlock</param>
    public void ForceUnlock(Skill mySkill)
    {
        unlockedSkills.Add(mySkill);
    }

    /// <summary>
    /// checks if a skill is unlocked
    /// </summary>
    /// <param name="mySkill"></param>
    /// <returns></returns>
    public bool IsUnlocked(Skill mySkill)
    {
        return unlockedSkills.Contains(mySkill);
    }

    /// <summary>
    /// checks if a skill is unlocked
    /// </summary>
    /// <param name="mySkill"></param>
    /// <returns></returns>
    public bool IsUnlocked(string mySkillName)
    {
        foreach(Skill skill in skillList)
        {
            if (skill.GetName() == mySkillName)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Checks if a skill has it's prerequisite skill met
    /// </summary>
    /// <param name="mySkill"></param>
    /// <returns></returns>
    public bool ArePrequisitesMet(Skill mySkill)
    {
        if(connectionList == null)  //there are no prerequisites
        {
            return true; 
        }
        SkillConnection sc = GetConnection(mySkill);
        if (sc != null)
        {
            foreach (Skill prerequisite in sc.prerequisiteSkills)
            {
                if (!unlockedSkills.Contains(prerequisite))  //missing prereq
                {
                    return false;
                }
            }
        }
        return true;
    }

    /// <summary>
    /// Returns a skill in the list that has the appropriate name
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public Skill GetSkill(string name)
    {
        foreach (Skill mySkill in skillList)
        {
            if (mySkill.GetName() == name)
            {
                return mySkill;
            }
        }
        return null;
    }

    /// <summary>
    /// returns the skill connections corresponding to the parent skill
    /// </summary>
    /// <param name="skill">the skill that has prerequisites</param>
    /// <returns></returns>
    public SkillConnection GetConnection(Skill skill)
    {
        foreach(SkillConnection sc in connectionList)
        {
            if(sc.parentSkill == skill)
            {
                return sc;
            }
        }
        return null;
    }

    /// <summary>
    /// Removes all skills from the unlock list and resets skill points
    /// </summary>
    /// <param name="skillPoints">the starting value of skill points that the tree should be reset to</param>
    public void EmptyTree(int mySkillPoints = 0)
    {
        unlockedSkills = new List<Skill>();
        skillPoints = mySkillPoints;
    }
    #endregion
}
