using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class STNodeLocationDict
{
    public List<STNodeLocationData> dict;

    public STNodeLocationData Tree(SkillTree tree)
    {
        foreach(STNodeLocationData stnld in dict)
        {
            if (tree == stnld.skillTree)
                return stnld;
        }
        return null;
    }

    public void Add(STNodeLocationData data)
    {
        if (dict==null)
        {
            dict = new List<STNodeLocationData>();
        }
        foreach(STNodeLocationData stnld in dict)
        {
            if(stnld.skillTree == data.skillTree)
            {
                stnld.dataList = data.dataList;
                return;
            }
        }
        dict.Add(data);
    }
}
