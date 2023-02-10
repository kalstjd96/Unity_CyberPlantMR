using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GuideMessage : MonoBehaviour
{
    public static GuideMessage instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void SetData(int rowIndex)
    {
        GetComponentInChildren<TMP_Text>().text = ProjectManager.instance.GetData(rowIndex, "Guide");
    }
}
