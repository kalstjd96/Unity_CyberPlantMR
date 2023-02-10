using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;
using System;
using System.Linq;
using UnityEngine.EventSystems;

public class GraphEvent : MonoBehaviour
{
    List<Dictionary<string, string>> graphDataRowList = new List<Dictionary<string, string>>();

    [Header("Data Properties")]
    [SerializeField] private Sprite circleSprite; //해당 좌표에 sprite 이미지 점을 찍을 것 
    public Color normalDotColor;
    public Color highlightDotColor;
    public List<Color> lineColorList; //그래프 계열 색깔목록

    [Header("Lable Templte")]
    public RectTransform labelTemplateX;
    public RectTransform labelTemplateY;
    public RectTransform dashTemplateX;

    [Header("Graph Component")]
    public RectTransform graphContainer;
    public Transform lableY;
    public Transform lable;
    public Transform grid;
    public Transform dot;
    public Transform line;
    public Toggle dataTipToggle;
    public Toggle gridToggle;
    public float distanceX = 20f;
    public GameObject lineItemPrefab; //계열 표시 프리팹
    public Transform lineItemParent; //계열 표시 프리팹

    public GameObject dataTipPrefab;
    public GameObject expandView;
    //private RectTransform dashTemplateY;
    //public GameObject Window_Graph;

    //임시... 이미지 교체 방식일 때
    public Image graphImage;

    //DataTable DTs = new DataTable();
    List<string> ColumnList = new List<string>();
    RectTransform ImageChild;
    float yMaximun;
    int aaa = 0;
    string selectItemName;

    List<GameObject> dataTipList;
    GameObject SelectedData;

    void Start()
    {
        dataTipToggle.onValueChanged.AddListener( delegate { OnDataToggle_ValueChanged(); });
        gridToggle.onValueChanged.AddListener( delegate { OnGridToggle_ValueChanged(); });
    }

    void OnDataToggle_ValueChanged()
    {
        if (dataTipList == null) return;

        SelectedData = null;

        if (dataTipToggle.isOn)
        {
            for (int i = 0; i < dataTipList.Count; i++)
            {
                dataTipList[i].transform.parent.GetComponent<RectTransform>().localScale = Vector3.one * 1.4f;
                dataTipList[i].transform.parent.GetComponent<Image>().color = highlightDotColor;
                dataTipList[i].SetActive(true);
            }
        }
        else
        {
            for (int i = 0; i < dataTipList.Count; i++)
            {
                dataTipList[i].transform.parent.GetComponent<RectTransform>().localScale = Vector3.one;
                dataTipList[i].transform.parent.GetComponent<Image>().color = normalDotColor;
                dataTipList[i].SetActive(false);
            }
        }
    }
    void OnGridToggle_ValueChanged()
    {
        if (gridToggle.isOn)
            grid.gameObject.SetActive(true);
        else
            grid.gameObject.SetActive(false);
    }

    //그래프 초기화
    public void InitializeGraph()
    {
        if (graphDataRowList != null)
        {
            dataTipList = new List<GameObject>();
            //grid.gameObject.SetActive(false);
            //dataTipToggle.isOn = false;
            //gridToggle.isOn = false;

            graphDataRowList.Clear();
            ColumnList.Clear();

            for (int i = 0; i < lableY.childCount; i++)
                Destroy(lableY.GetChild(i).gameObject);
            for (int i = 0; i < lable.childCount; i++)
                Destroy(lable.GetChild(i).gameObject);
            for (int i = 0; i < grid.childCount; i++)
                Destroy(grid.GetChild(i).gameObject);
            for (int i = 0; i < dot.childCount; i++)
                Destroy(dot.GetChild(i).gameObject);
            for (int i = 0; i < line.childCount; i++)
                Destroy(line.GetChild(i).gameObject);
            for (int i = 0; i < lineItemParent.childCount; i++)
                Destroy(lineItemParent.GetChild(i).gameObject);
        }
    }

    public void ClickItem(string selectItemName)
    {
        graphImage.sprite = Resources.Load<Sprite>("Graph/" + selectItemName);
        expandView.SetActive(true);
        GetComponent<GraphViewer>().graphList.SetActive(false);

        //InitializeGraph();
        //GetComponent<GraphViewer>().graphList.SetActive(false);
        //this.selectItemName = selectItemName;
        ////string query = "Select* from GraphData where " + '"' + "증강현실 배관위치" + '"' + " = '" + EventSystem.current.currentSelectedGameObject.name + "'";
        //graphDataRowList = CSV.Read("GraphData");
        ////graphDataRowList.Sort((x, y) => int.Parse(x["No"]).CompareTo(int.Parse(y["No"])));
        //var temp = graphDataRowList.Where(list => list["증강현실 배관위치"].Equals(selectItemName));
        //List<Dictionary<string, string>> selectList = temp.ToList();

        //for (int i = 0; i < selectList.Count; i++)
        //{
        //    int index = 0;
        //    List<double> valueList = new List<double>();
        //    ColumnList = new List<string>();

        //    foreach (var columnName in selectList[i].Keys)
        //    {
        //        if (index > 2)
        //        {
        //            ColumnList.Add(columnName);
        //            valueList.Add(double.Parse(selectList[i][columnName].ToString()));
        //        }
        //        index++;
        //    }
        //    ShowGraph(i, valueList, selectItemName);
        //    //계열 표시
        //    GameObject lineItem = Instantiate(lineItemPrefab, lineItemParent);
        //    lineItem.GetComponent<Image>().color = lineColorList[i];
        //    lineItem.GetComponentInChildren<Text>().text = selectList[i]["기동일"].ToString();
        //}
        //if (selectList.Count > 1)
        //{
        //    for (int i = 0; i <= (dataTipList.Count / 2) - 1; i++)
        //    {
        //        double data1 = double.Parse(dataTipList[i].GetComponentInChildren<Text>().text);
        //        double data2 = double.Parse(dataTipList[i + (dataTipList.Count / 2)].GetComponentInChildren<Text>().text);
        //        int result = data1.CompareTo(data2);

        //        if (result > 0) // data1이 더 큼
        //        {
        //            Vector3 pos = dataTipList[i].transform.localPosition;
        //            pos.y += 20f;
        //            dataTipList[i].transform.localPosition = pos;
        //            Vector3 pos1 = dataTipList[i + (dataTipList.Count / 2)].transform.localPosition;
        //            pos1.y -= 20f;
        //            dataTipList[i + (dataTipList.Count / 2)].transform.localPosition = pos1;
        //        }
        //        else if (result < 0)// data2이 더 큼
        //        {
        //            Vector3 pos = dataTipList[i].transform.localPosition;
        //            pos.y -= 20f;
        //            dataTipList[i].transform.localPosition = pos;
        //            Vector3 pos1 = dataTipList[i + (dataTipList.Count / 2)].transform.localPosition;
        //            pos1.y += 20f;
        //            dataTipList[i + (dataTipList.Count / 2)].transform.localPosition = pos1;
        //        }
        //        else
        //        {
        //            Vector3 pos = dataTipList[i].transform.localPosition;
        //            pos.y += 20f;
        //            dataTipList[i].transform.localPosition = pos;
        //            Vector3 pos1 = dataTipList[i + (dataTipList.Count / 2)].transform.localPosition;
        //            pos1.y += 20f;
        //            dataTipList[i + (dataTipList.Count / 2)].transform.localPosition = pos1;
        //        }
        //    }
        //}
        //else
        //{
        //    for (int i = 0; i < dataTipList.Count; i++)
        //    {
        //        Vector3 pos = dataTipList[i].transform.localPosition;
        //        pos.y += 30f;
        //        dataTipList[i].transform.localPosition = pos;
        //    }
        //}
        //expandView.SetActive(true);
        //OnDataToggle_ValueChanged();
        //OnGridToggle_ValueChanged();
    }


    //data 점 표시
    private GameObject CreateCircle(int lineIndex, float data, Vector2 anchoredPosition)
    {
        Debug.Log(lineIndex + " / " + data);
        GameObject dotObj = new GameObject("circle", typeof(Button));
        dotObj.transform.SetParent(dot, false);
        dotObj.AddComponent<Image>().sprite = circleSprite;
        dotObj.GetComponent<Image>().color = normalDotColor;

        RectTransform rectTransform = dotObj.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(12f, 12f);
        rectTransform.anchorMin = new Vector2(0f, 0f);
        rectTransform.anchorMax = new Vector2(0f, 0f);

        GameObject dataTip = Instantiate(dataTipPrefab);
        dataTip.transform.SetParent(dotObj.transform);
        dataTip.transform.localPosition = Vector3.zero;

        GameObject dotClickArea = new GameObject("ClickArea", typeof(Image));
        dotClickArea.transform.SetParent(dotObj.transform);
        dotClickArea.GetComponent<RectTransform>().sizeDelta = new Vector2(50f, 50f);
        dotClickArea.transform.localPosition = Vector3.zero;
        dotClickArea.GetComponent<Image>().CrossFadeAlpha(0f, 1f, true); //투명하게 

        Vector3 position = dataTip.transform.localPosition;
        //position.y += 30f;
        dataTip.transform.localPosition = position;
        dataTip.transform.localScale = Vector3.one;
        dataTip.GetComponentInChildren<Text>().text = data.ToString();
        dataTipList.Add(dataTip);
        //점 눌렀을 때 data 보여주기
        dotObj.GetComponent<Button>().onClick.AddListener( 
            delegate 
            {
                if (!dataTipToggle.isOn)
                {
                    if (SelectedData && SelectedData != dotObj)
                    {
                        SelectedData.transform.GetChild(0).gameObject.SetActive(false);
                        SelectedData.transform.GetComponent<Image>().color = normalDotColor;
                        SelectedData.transform.GetComponent<RectTransform>().localScale = Vector3.one;
                    }

                    dataTip.SetActive(!dataTip.activeSelf);

                    if (dataTip.activeSelf)
                    {
                        dotObj.transform.GetComponent<RectTransform>().localScale = Vector3.one * 1.4f;
                        dotObj.GetComponent<Image>().color = highlightDotColor;
                    }
                    else
                    {
                        dotObj.transform.GetComponent<RectTransform>().localScale = Vector3.one;
                        dotObj.GetComponent<Image>().color = normalDotColor;
                    }

                    SelectedData = dotObj;
                }
            });
        
        return dotObj;
    }
    
    private void ShowGraph(int lineIndex, List<double> valueList , string chartName)
    {
        if (chartName  == "F1" || chartName == "LT1")
        {
            yMaximun = 10f;
        }
        else if (chartName == "L1")
        {
            yMaximun = 70f;
        }
        else if (chartName == "HT1")
        {
            yMaximun = 80f;
        }
        else if (chartName == "LT12")
        {
            yMaximun = 50f;
        }
        else if (chartName == "HT2")
        {
            yMaximun = 100f;
        }
        float graphHeight = graphContainer.sizeDelta.y;
        //float yMaximun = 100f; //y축 값 

        GameObject lastCircleGameObject = null; //이전 좌표 선을 잇기 위해서 
        float startPosX = 70f;
        float currentPosX = 0f;
        for (int i = 0; i < valueList.Count; i++)
        {
            currentPosX = startPosX + distanceX;

            float yPosition = (float)((valueList[i] / yMaximun) * graphHeight);
            GameObject circleGameObject = CreateCircle(lineIndex, (float)valueList[i], new Vector2(currentPosX, yPosition));
            if (lastCircleGameObject != null)
            {
                CreateDotConnection(lineIndex, lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition, circleGameObject.GetComponent<RectTransform>().anchoredPosition);
            }
            lastCircleGameObject = circleGameObject;
            
            //X축 아래 숫자 나타내기 
            RectTransform labelX = Instantiate(labelTemplateX);
            labelX.SetParent(lable, false);
            labelX.gameObject.SetActive(true);
            //labelX.anchoredPosition = new Vector2(currentPosX, (i+1) % 2 == 0 ? -40f : -20f); //날짜값과 그래프 사이 넓이
            labelX.anchoredPosition = new Vector2(currentPosX, (i+1) % 2 == 0 ? -30f : -10f); //날짜값과 그래프 사이 넓이
            labelX.GetComponent<Text>().text = ColumnList[i].ToString(); //숫자 
            //X축 격자 위치 조정
            Vector3 girdX_pos = labelX.GetChild(0).localPosition;
            girdX_pos.y -= (i + 1) % 2 == 0 ? 10f : 30f;
            labelX.GetChild(0).localPosition = girdX_pos;

            startPosX = currentPosX;

            //격자 생성.
            GameObject verLine = new GameObject("verLine", typeof(Image));
            verLine.transform.SetParent(grid);
            verLine.GetComponent<Image>().color = new Color32(160, 200, 200, 255);
            verLine.GetComponent<RectTransform>().anchoredPosition = new Vector2(currentPosX - 51f, 320f);
            verLine.GetComponent<RectTransform>().sizeDelta = new Vector2(3f, 10f);
            verLine.GetComponent<RectTransform>().localScale = new Vector3(1f, 73f, 1f);

            //content 크기 확장
            if (i == valueList.Count - 1)
            {
                Vector2 size = graphContainer.GetComponent<RectTransform>().sizeDelta;
                size.x = labelX.anchoredPosition.x + 100f;
                graphContainer.GetComponent<RectTransform>().sizeDelta = size;
            }
        }
        
        int separatorCount = 10; //y축을 10개 나눈다.
        for (int i= 0; i < separatorCount; i++)
        {
            RectTransform labelY = Instantiate(labelTemplateY);
            labelY.SetParent(lable, false);
            labelY.gameObject.SetActive(true);
            float normalizedValue = i * 1f / separatorCount; //y축을 나타내는 코드 
            labelY.anchoredPosition = new Vector2(-7f, normalizedValue * graphHeight);
            //labelY.GetComponent<Text>().text = Mathf.RoundToInt(normalizedValue).ToString();
            labelY.GetComponent<Text>().text = Mathf.RoundToInt(normalizedValue * yMaximun).ToString();
            labelY.transform.SetParent(this.lableY);
            //위치 조정
            Vector3 pos = labelY.anchoredPosition;
            pos.x = -10f;
            labelY.anchoredPosition = pos;
        }
    }

    private void CreateDotConnection(int lineIndex, Vector2 dotPositionA, Vector2 dotPositionB)
    {
        GameObject gameObject = new GameObject("dotConnection", typeof(Image));
        gameObject.transform.SetParent(line, false);
        gameObject.transform.SetAsFirstSibling();
        //gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
        gameObject.GetComponent<Image>().color = lineColorList[lineIndex]; //선 색깔 

        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        Vector2 dir = (dotPositionB - dotPositionA).normalized;
        float distance = Vector2.Distance(dotPositionA, dotPositionB);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.sizeDelta = new Vector2(distance, 5f);
        rectTransform.anchoredPosition = dotPositionA + dir * distance * 0.5f;
        rectTransform.localEulerAngles = new Vector3(0, 0, UtilsClass.GetAngleFromVectorFloat(dir));
    }
}