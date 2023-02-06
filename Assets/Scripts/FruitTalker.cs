using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitTalker : MonoBehaviour
{
    [SerializeField]
    private SkillTree fruitTree;

    [SerializeField]
    private List<string> potentialFruits;

    private void Start()
    {
        fruitTree.EmptyTree(0);
        foreach(Skill fruitSkill in fruitTree.skillList)
        {
            potentialFruits.Add(fruitSkill.GetName());
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log($"I eat my fruit containing: {FruitSupply()}");
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log($"I throw my fruit containing: {FruitSupply()}");
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            Debug.Log("I make some money to pay for more fruits to add to my tree...");
            Debug.Log($"I now have {++fruitTree.skillPoints} dollars");
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log($"I try to add apples to my fruit tree for ${fruitTree.GetSkill("apple").GetCost()}...");
            if (fruitTree.UnlockSkill("apple"))
            {
                Debug.Log("My tree now has apples");
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log($"I try to add bananas to my fruit tree for ${fruitTree.GetSkill("banana").GetCost()}...");
            if (fruitTree.UnlockSkill("banana"))
            {
                Debug.Log("My tree now has bananas");
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log($"I try to add berries to my fruit tree for ${fruitTree.GetSkill("berry").GetCost()}...");
            if (fruitTree.UnlockSkill("berry"))
            {
                Debug.Log("My tree now has berries");
            }
        }
    }

    public string FruitSupply()
    {
        string myFruit = "";
        foreach(string fruit in potentialFruits)
        {
            if(fruitTree.IsUnlocked(fruit))
            {
                myFruit += fruit + ", ";
            }
        }
        return myFruit;
    }
}
