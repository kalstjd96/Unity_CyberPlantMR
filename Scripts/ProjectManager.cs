using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Microsoft.MixedReality.Toolkit.UI;

public class ProjectManager : MonoBehaviour
{
    public List<Dictionary<string, string>> seqRowData { get; private set; }
    public static ProjectManager instance;

    public GameObject DebugPanel;
    public Transform EDG_Control_Panel;
    
    Animation[] TagAni;
    GameObject prevTarget;
    string animName;

    [NonSerialized] public Coroutine mainCor;
    [NonSerialized] public string tableName;
    [NonSerialized] public int rowIndex;
    [NonSerialized] public string seqType;
    [NonSerialized] public bool isDone;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    //Debug MR 패널에 띄우기
    public void Debugging(string debugText)
    {
        if (DebugPanel)
            DebugPanel.GetComponentInChildren<TMP_Text>().text += "/ " + debugText;
            
        Debug.Log(debugText);
    }

    public string GetData(int rowIndex, string columnName)
    {
        return seqRowData[rowIndex][columnName].Replace("$", "\n");
    }

    //SLOW or FAST 버튼 클릭 이벤트
    public void OnClickModeButton(string tableName)
    {
        seqRowData = CSV.Read(tableName); //CSV reader
        seqRowData.Sort((x, y) => int.Parse(x["No"]).CompareTo(int.Parse(y["No"]))); //No 기준 오름차순 Sorting
        this.tableName = tableName;
        SceneManager.LoadScene("Main");
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "TitleScene":
            {
                GameObject.Find("SlowModeBtn").GetComponent<Interactable>().OnClick.AddListener (
                delegate { OnClickModeButton("SLOW_SEQ"); });
                GameObject.Find("FastModeBtn").GetComponent<Interactable>().OnClick.AddListener(
                delegate { OnClickModeButton("FAST_SEQ"); });
                StartCoroutine(Init_ModelState());
                break;
            }
            case "Main":
            {
                StartSenario(); break;
            }
        }
    }

    //시나리오 시작
    public void StartSenario()
    {
        mainCor = null;
        rowIndex = 0;
        
        ProcedureList.instance.Set();
        DocumentViewer.instance.LoadData();
        //subSenarioPanel.SetActive(true);
        //seqDocument.Create();

        isDone = true;
        GuideMessage.instance.gameObject.SetActive(true);
        mainCor = StartCoroutine(MainProcess());
    }

    bool temp;
    public IEnumerator MainProcess()
    {
        WaitUntil nextTrigger = new WaitUntil(() => isDone);
        for (int i = rowIndex; i < seqRowData.Count; i++)
        {
            yield return nextTrigger;
            rowIndex = i;
            isDone = false;

            if (prevTarget)
                foreach (var anim in prevTarget.GetComponentsInChildren<Animation>())
                {
                    foreach (AnimationState clip in anim)
                    {
                        anim[clip.name].time = 0;
                        anim.Play();
                        yield return null;
                        anim.Stop();
                    }
                }

            ProcedureList.instance.ShowCurrent(GetData(rowIndex, "Procedure").ToString());
            //GuideMessage 메세지 설정
            GuideMessage.instance.SetData(rowIndex);
            //상세 절차내용 설정
            SequenceDetail.instance.SetData(rowIndex);
            
            //Sub 시퀀스목록창 띄우기
            if (GetData(rowIndex, "Type").Equals("0") || GetData(rowIndex, "SubList").ToString().Trim() == "TRUE")
            {
                ProcedureList.instance.subListPanel.SetActive(true);
            }
            //현재 절차 목록 버튼 하이라이트표기
            //ProcedureList.instance.ShowCurrent(seqRowData[rowIndex]["Procedure"]);
            //??
            if (GetData(rowIndex, "Type").Equals("0"))
                yield return new WaitUntil(() => temp);
            yield return new WaitForSeconds(0.3f);

            TypeCheck(GetData(rowIndex, "Type"));
            yield return nextTrigger; //절차 수행완료할 때까지 기다림
            isDone = false;

            //완료체크 팝업
            ProcedureList.instance.checkMark.SetActive(true);

            if (rowIndex == seqRowData.Count - 1)
            {
                ProcedureList.instance.completePanel.SetActive(true);
                yield break;
            }
        }
    }

    void TypeCheck(string seqType)
    {
        this.seqType = seqType;

        switch (seqType)
        {
            case "0": //SubSeq
                {
                    break;
                }
            case "00": //SubSeq - CheckMark만 띄움
                {
                    isDone = true;
                    break;
                }
            case "1": //EDG_Panel 애니메이션
                {
                    string targetName = GetData(rowIndex, "TagNo").Trim();
                    GameObject target = EDG_Control_Panel.Find(targetName.ToString()).gameObject;
                    TagAni = target.GetComponentsInChildren<Animation>();
                    //for (int j = 0; j < target.transform.childCount; j++)
                    //{
                    //    if (target.transform.GetChild(j).name.Contains("Canvas"))
                    //        target.transform.GetChild(j).gameObject.SetActive(true);
                    //}
                    for (int i = 0; i < TagAni.Length; i++)
                    {
                        animName = TagAni[i].name.Equals("NavPointer") ? "NavPointer" : GetData(rowIndex, "Animation").Trim();
                        TagAni[i].Play(animName);
                    }
                     prevTarget = target;

                    isDone = true;
                    break;
                }
            case "2": //체크박스
                {
                    CheckboxList.instance.transform.GetChild(0).gameObject.SetActive(true);
                    CheckboxList.instance.StartCoroutine(CheckboxList.instance.LoadData(GetData(rowIndex, "CheckList").Trim()));
                    break;
                }
            case "3": //설명(Description) 패널 확인
                {
                    Description.instance.transform.GetChild(0).gameObject.SetActive(true);
                    Description.instance.SetData(rowIndex);
                    isDone = true;
                    break;
                }
            case "4": //기록지 점검 => 3번 타입으로 통일
                {
                    //if (Navigator.instance.isOn)
                    //    Navigator.instance.On(dataRow);

                    //isDone = true;
                    Description.instance.transform.GetChild(0).gameObject.SetActive(true);
                    Description.instance.SetData(rowIndex);
                    isDone = true;
                    break;
                }
            case "5": // Timer
                {
                    //if (Navigator.instance.isOn)
                    //    Navigator.instance.On(dataRow);

                    //isDone = true;
                    Description.instance.transform.GetChild(0).gameObject.SetActive(true);
                    Description.instance.SetData(rowIndex);
                    isDone = true;
                    break;
                }
        }
    }

    public void Next_Btn()
    {
        Init();
        isDone = true;
    }

    public void Init()
    {
        Description.instance.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        ProcedureList.instance.checkMark.SetActive(false);
        CheckboxList.instance.Init();
    }

    
    IEnumerator Init_ModelState()
    {
        EDG_Control_Panel.parent.GetComponent<Collider>().enabled = true;

        Animation[] anims = EDG_Control_Panel.GetComponentsInChildren<Animation>();
        for (int i = 0; i < anims.Length; i++)
        {
            if (anims[i][anims[i].name])
            {
                anims[i][anims[i].name].time = 0;
                anims[i].Play();
                yield return null;
                anims[i].Stop();
            }
        }
    }

    public void Set_BlendMode(Transform parent, Set_RendererMode.BlendMode mode, float alpha)
    {
        foreach (var renderer in parent.GetComponentsInChildren<MeshRenderer>())
        {
            if (!renderer.gameObject.tag.Equals("None_Transparent"))
            {
                Material[] mat = renderer.gameObject.GetComponent<MeshRenderer>().materials;
                for (int g = 0; g < mat.Length; g++)
                {
                    Set_RendererMode.ChangeRenderMode(mat[g], mode);

                    mat[g].color = new Color(mat[g].color.r,
                                    mat[g].color.g,
                                    mat[g].color.b,
                                    alpha);
                }
            }
        }
    }
}
