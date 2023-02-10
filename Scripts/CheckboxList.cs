using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CheckboxList : MonoBehaviour
{
    public static CheckboxList instance;
    public GameObject listPrefab;
    public Transform content;
    public GameObject completeBtn;
    List<GameObject> checkList;
    int dataCount;
    int checkCount;
    bool isOn;
    bool isDone;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void OnClick_CheckButton(GameObject target)
    {
        checkCount++;
        target.GetComponentInChildren<Interactable>().enabled = false;
        target.transform.GetChild(0).Find("CheckMark").gameObject.SetActive(true);
        
        if (checkCount == dataCount)
        {
            ProjectManager.instance.isDone = true;
            completeBtn.SetActive(true);
            checkCount = 0;
        }
    }

    public void Init()
    {
        if (checkList != null)
            for (int i = 0; i < checkList.Count; i++)
            {
                Destroy(checkList[i]);
            }
        completeBtn.SetActive(false);
        transform.GetChild(0).gameObject.SetActive(false);
        StopAllCoroutines();
        isDone = false;
        isOn = false;
    }

    public IEnumerator LoadData(string str)
    {
        isOn = true;

        checkList = new List<GameObject>();

        string[] data = str.Split('&');
        dataCount = data.Length;
        for (int i = 0; i < data.Length; i++)
        {
            GameObject listObj = Instantiate(listPrefab, content);
            listObj.GetComponent<TMP_Text>().text = data[i].Trim();
            listObj.GetComponentInChildren<Interactable>().OnClick.AddListener(delegate { OnClick_CheckButton(listObj); });
            checkList.Add(listObj);
        }

        yield return null;
    }
}
