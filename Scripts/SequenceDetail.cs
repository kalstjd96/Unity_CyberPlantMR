using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SequenceDetail : MonoBehaviour
{
    public static SequenceDetail instance;
    public GameObject seqDetail_Toggle;
    public TMP_Text content;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void SetData(int rowIndex)
    {
        content.text = ProjectManager.instance.GetData(rowIndex, "SEQ_DETAIL");
    }

    public void OnClick_SeqDetailToggle(GameObject target)
    {
        target.SetActive(!target.activeSelf);
    }

    public void OnClick_CloseBtn(GameObject target)
    {
        seqDetail_Toggle.GetComponent<Interactable>().IsToggled = false;
        target.SetActive(false);
    }
}
