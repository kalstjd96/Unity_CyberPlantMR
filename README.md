<img src="https://capsule-render.vercel.app/api?type=waving&color=auto&height=200&section=header&text=CyberPlant-MR&fontSize=80" /> 

# MR 기반 EDG Local Control Panel 기동 절차 시스템

## Features (담당 기능)

-   [Panel 진행 절차 DB 구성](#panel-procedure-db)
-   [Panel 리스트 뷰어 기능](#panel-list-viewer)
-   [절차별 Target에 대한 상태](#target-state)
-   [그래프로 표현된 Data 정보 열람 기능](#graph-data-viewer)
-   [HoloLens2 Motion 적용(MRTK)](#mrtk)
    
## Panel Procedure DB

>사용된 스크립트<br/>
> DB_Manager.cs

Control Panel 진행에 대한 절차를 SQLite 기반으로 DB 구성하였습니다.

## Panel List Viewer

>사용된 스크립트<br/>
> ProcedureList.cs

진행해야할 절차를 리스트 형태로 확인하는 기능을 구현하였습니다.

```c#

using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class ProcedureList : MonoBehaviour
{
    public static ProcedureList instance;

    #region PROPERTIES
    DataTable DT;
    Dictionary<int, string> procedureRowDic;
    List<GameObject> procedureList;
    List<GameObject> buttonList;

    public ScrollingObjectCollection scrollObjectCollection;
    public GameObject modelLockBtn;
    public GameObject checkMark;
    public GameObject listPrefab;
    public GameObject listParent;
    public GameObject subListPanel;
    public GameObject subListPrefab;
    public GameObject subListParent;
    public TMP_Text modeTitleText;
    public GameObject completePanel;

    public Material originalMaterial;
    public Material highlightMaterial;
    public Color highlightTextColor;
    public Color doneColor;
    #endregion

    Transform scrollContainer;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void ToggleActive()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    //초기 설정들 (데이터 로드 및 리스트 생성)
    public void Set()
    {
        //타이틀 모드텍스트 입력 (SLOW/FAST)
        scrollContainer = scrollObjectCollection.transform.GetChild(0);
        string ScenarioName = ProjectManager.instance.tableName;
        string[] resultSc = ScenarioName.Split('_');
        modeTitleText.text = resultSc[0].ToString();

        //DB 데이터 로드
        LoadData();

        //리스트 생성
        Create();
        SetLockBtnEvent();
        //하위 리스트 생성
        SetSubSequenceList();
    }

    void SetSubSequenceList()
    {
        for (int i = 0; i < ProjectManager.instance.seqRowData.Count; i++)
        {
            int rowIndex = 0;
            if (!string.IsNullOrEmpty(ProjectManager.instance.GetData(i, "SubList")))
            {
                rowIndex = i;
                GameObject subListObj = Instantiate(subListPrefab, subListParent.transform);
                subListObj.GetComponentInChildren<Interactable>().OnClick.AddListener(delegate
                {
                    SkipTo(rowIndex);
                });
                subListObj.GetComponentInChildren<TMP_Text>().text = ProjectManager.instance.GetData(i, "Guide");
            }
        }

        subListParent.GetComponent<GridObjectCollection>().UpdateCollection(); //Grid 정렬 갱신
    }

    //절차(Procedure) 중복제거 저장
    void LoadData()
    {
        procedureRowDic = new Dictionary<int, string>();

        string procedureText = "0";
        string temp = "";

        for (int i = 0; i < ProjectManager.instance.seqRowData.Count; i++)
        {
            temp = ProjectManager.instance.GetData(i, "Procedure");
            if (string.IsNullOrEmpty(temp)) continue;

            if (!procedureText.Equals(temp))
            {
                procedureText = temp;
                procedureRowDic.Add(i, procedureText);
            }
        }
    }

    public void Create()
    {
        buttonList = new List<GameObject>();
        
        for (int i = 0; i < procedureRowDic.Count; i++)
        {
            int rowIndex = procedureRowDic.ElementAt(i).Key;
            if (!ProjectManager.instance.GetData(rowIndex, "SubList").Equals("TRUE"))
            {
                GameObject listObj = Instantiate(listPrefab, listParent.transform);
                listObj.GetComponent<Interactable>().OnClick.AddListener(delegate
                {
                    SkipTo(rowIndex);
                });

                //리스트 속성 설정
                listObj.transform.Find("Number").GetComponent<TMP_Text>().text = (i + 1).ToString("000") + ". ";
                listObj.transform.Find("Local").GetComponent<TMP_Text>().text = "[ " + ProjectManager.instance.GetData(rowIndex, "Location") + " ]";
                listObj.transform.Find("Main_Text").GetComponent<TMP_Text>().text = procedureRowDic.ElementAt(i).Value;

                buttonList.Add(listObj);
            }
        }
        listParent.GetComponent<GridObjectCollection>().UpdateCollection(); //Grid 정렬 갱신
    }

    [System.NonSerialized] public GameObject prevList;
    public void ShowCurrent(string compareData)
    {
        if (prevList != null)
        {
            prevList.transform.Find("BackPlate").Find("Quad").GetComponent<MeshRenderer>().material = originalMaterial;
            prevList.transform.Find("Main_Text").GetComponent<TMP_Text>().color = Color.white;
            prevList.transform.Find("Number").GetComponent<TMP_Text>().color = Color.white;
        }
        GameObject currentList = buttonList.Find(x => x.transform.Find("Main_Text").GetComponent<TMP_Text>().text.Equals(compareData));
        currentList.transform.Find("BackPlate").Find("Quad").GetComponent<MeshRenderer>().material = highlightMaterial;
        currentList.transform.Find("Main_Text").GetComponent<TMP_Text>().color = highlightTextColor;
        currentList.transform.Find("Number").GetComponent<TMP_Text>().color = highlightTextColor;

        prevList = currentList;

        AutoScroll(prevList.transform.GetSiblingIndex());
        //scrollObjectCollection.MoveByTiers(prevList.transform.GetSiblingIndex());
    }

    void AutoScroll(int index)
    {
        scrollContainer.DOKill();
        scrollContainer.localPosition = Vector3.zero;

        if (index >= 2)
        {
            Vector3 position = scrollContainer.localPosition;
            position.y += listParent.GetComponent<GridObjectCollection>().CellHeight * index - scrollObjectCollection.CellHeight;

            scrollContainer.DOLocalMove(position, 0.2f);
        }

        if (scrollContainer.localPosition.y > 1.4f)
        {
            scrollContainer.DOKill();
            scrollContainer.localPosition = new Vector3(scrollContainer.localPosition.x, 1.4f, scrollContainer.localPosition.z);
        }
        if (scrollContainer.localPosition.y < 0f)
        {
            scrollContainer.DOKill();
            scrollContainer.localPosition = Vector3.zero;
        }
    }

    //행(rowIndex) 간 점프기능 (Description 점프)
    public void OnClick_DescriptionSkip_Btn(GameObject btnObj)
    {
        //첫번째 시퀀스 혹은 마지막 시퀀스가 아닐 때.
        int resultRowIndex = 0;

        if (btnObj.name.Contains("Prev") && ProjectManager.instance.rowIndex != 0)
            resultRowIndex = ProjectManager.instance.rowIndex - 1;
        else if (btnObj.name.Contains("Next") && ProjectManager.instance.rowIndex < ProjectManager.instance.seqRowData.Count - 1)
            resultRowIndex = ProjectManager.instance.rowIndex + 1;

        SkipTo(resultRowIndex);
    }

    //해당 rowIndex 절차로 점프
    public void SkipTo(int rowIndex)
    {
        ProjectManager.instance.StopCoroutine(ProjectManager.instance.mainCor); // 진행중인 메인코루틴 중지
        ProjectManager.instance.rowIndex = rowIndex; //선택한 절차의 열번호 설정
        ProjectManager.instance.Next_Btn(); //초기화 및 다음 시퀀스 준비
        ProjectManager.instance.mainCor = StartCoroutine(ProjectManager.instance.MainProcess()); //메인코루틴 시작
    }

    //사용자 완료 체크
    public void CheckMarking()
    {
        if (!ProjectManager.instance.isDone) //중복 방지
        {
            if (prevList != null)
                prevList.transform.Find("Done").GetComponent<SpriteRenderer>().color = doneColor;

            if (ProjectManager.instance.rowIndex == ProjectManager.instance.seqRowData.Count - 1)
            {
                ProjectManager.instance.Init();
            }
            else
                Invoke("Next", 1f);
        }
    }

    void Next()
    {
        ProjectManager.instance.Next_Btn();
    }

    #region 완료패널 버튼이벤트
    public void OnClick_HomeBtn()
    {
        SceneManager.LoadScene("TitleScene");
    }
    public void OnClick_AppQuitBtn()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    #endregion
    
    public void SetLockBtnEvent()
    {
        Interactable toggleModule = modelLockBtn.GetComponent<Interactable>();
        toggleModule.OnClick.RemoveAllListeners();
        toggleModule.OnClick.AddListener(delegate
        {
            if (toggleModule.IsToggled)
            {
                //모델 Lock
                modelLockBtn.transform.Find("Lock").gameObject.SetActive(true);
                modelLockBtn.transform.Find("UnLock").gameObject.SetActive(false);
                ProjectManager.instance.EDG_Control_Panel.parent.GetComponent<Collider>().enabled = false;
            }
            else
            {
                //모델 Unlock
                ProjectManager.instance.EDG_Control_Panel.parent.GetComponent<Collider>().enabled = true;
                modelLockBtn.transform.Find("Lock").gameObject.SetActive(false);
                modelLockBtn.transform.Find("UnLock").gameObject.SetActive(true);
            }
        });
    }
}
```
<img src="https://user-images.githubusercontent.com/47016363/217998078-331fba74-9df0-4c51-ac18-9ff4d9780b5e.png"  width="400" height="250"/>

## Target State

>사용된 스크립트<br/>
> Highlight.cs , GuideMessage.cs 

해당 절차에 대한 Target 상태를 표현하였습니다. ex. 애니메이션, 가이드 등 

```c#

//하이라이트 색상으로 깜빡임 
while (true)
{
    for (int i = 0; i < targets.Length; i++)
    {
        Material targeMat = highlightMaterial;
        Color targetColor = targeMat.color;
        //Color targetColor = new Color(0f, 30f, 255f, 255f) * 10f : Color.white * 200f;
        targetColor.a = 1.5f;

        for (int j = 0; j < targets[i].materials.Length; j++)
        {
            // Target의 RenderMode 변경 
            ChangeRenderMode(targets[i].materials[j], BlendMode.Transparent);
            targets[i].materials[j] = targeMat;
            targets[i].materials[j].SetColor("_Color",
                Color.Lerp(targeMat.color, targetColor, Mathf.PingPong(Time.time, 1.5f)));
        }

    }
    yield return null;
}


```
<img src="https://user-images.githubusercontent.com/47016363/217998187-0a5727b9-833d-4189-af01-abc630d038c0.png"  width="400" height="250"/>

## Graph Data Viewer

DB에 저장된 데이터를 기반으로 그래프 뷰어를 구현하였습니다. (오픈소스 )

<img src="https://user-images.githubusercontent.com/47016363/217997541-07d916e2-a315-4baa-97a3-63c46751ec48.png"  width="400" height="250"/>

## MRTK

MRTK를 통해 가상 물체의 크기, 위치, 회전 등 조작 기능을 적용하였습니다.

<img src="https://user-images.githubusercontent.com/47016363/217986555-00894438-ebaa-4e50-9ef7-49df1b70e041.png"  width="400" height="250"/>
<img src="https://user-images.githubusercontent.com/47016363/217989203-7a7d481d-4426-46e0-8399-3153e20877ce.png"  width="400" height="250"/>

