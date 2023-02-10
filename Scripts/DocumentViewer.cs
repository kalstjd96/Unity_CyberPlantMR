using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;

public class DocumentViewer : MonoBehaviour
{
    public static DocumentViewer instance;
    List<Dictionary<string, string>> drawingDataRowList;

    DataTable drawingTable;
    public GameObject PnID_Toggle;

    public GameObject drawingPrefab;
    public Transform layoutGroup;

    public GameObject viewer;
    public GameObject collectionList;
    public Image viewImage;

    RectTransform viewImageRect;
    Vector3 OriginalPos;
    Vector3 OriginalScale;
    float scroll_Sensitivity = 1f;
    float zoomOutMin = 1;
    float zoomOutMax = 5;
    float zoomSpeed = 0.5f;

    void Awake()
    {
        if (instance == null)
            instance = this;

        viewImageRect = viewImage.transform.parent.GetComponent<RectTransform>();
        OriginalPos = viewImageRect.position;
        OriginalScale = viewImageRect.localScale;
    }

    void Update()
    {
        if (viewer.activeSelf)
        {
#if UNITY_EDITOR
            float mouseScrollValue = Input.GetAxis("Mouse ScrollWheel");

            if (mouseScrollValue > 0)
            {
                if (viewImageRect.localScale.x < 5f)
                    viewImageRect.localScale += viewImageRect.localScale * mouseScrollValue * scroll_Sensitivity;
                else viewImageRect.localScale = Vector3.one * 5f;
            }
            else
            {
                if (viewImageRect.localScale.x > 1f)
                    viewImageRect.localScale += viewImageRect.localScale * Input.GetAxis("Mouse ScrollWheel") * scroll_Sensitivity;
                else viewImageRect.localScale = Vector3.one;
            }
#elif UNITY_ANDROID
            if (Input.touchCount == 2)
            {
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);
                
                Vector2 touchZeroPrev = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrev = touchOne.position - touchOne.deltaPosition;
                
                float prevMagnitude = (touchZeroPrev - touchOnePrev).magnitude;
                float curMagnitude = (touchZero.position - touchOne.position).magnitude;
                
                float deltaDiff = prevMagnitude - curMagnitude;
                viewImageRect.localScale += viewImageRect.localScale * deltaDiff * Time.deltaTime * zoomSpeed;
            }
#endif
        }
    }

    public void LoadData()
    {
        drawingDataRowList = CSV.Read("Drawing");
        drawingDataRowList.Sort((x, y) => int.Parse(x["Index"]).CompareTo(int.Parse(y["Index"])));
        
        for (int i = 0; i < drawingDataRowList.Count; i++)
        {
            string drawingNo = drawingDataRowList[i]["DrawingNo"];
            GameObject drawingObj = Instantiate(drawingPrefab, layoutGroup);
            drawingObj.name = drawingNo;
            drawingObj.GetComponentInChildren<SpriteRenderer>().sprite = Resources.Load<Sprite>("P&ID/" + drawingNo);
            drawingObj.GetComponent<Interactable>().OnClick.AddListener(delegate { OnClickDrawingBtn(drawingObj.GetComponentInChildren<SpriteRenderer>()); });
        }
        layoutGroup.GetComponent<GridObjectCollection>().UpdateCollection();
    }

    public void SetActive(bool value)
    {
        if (value)
        {
            gameObject.transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            InitializeViewer();
            gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    public void OnClickDrawingBtn(SpriteRenderer spriteRenderer)
    {
        collectionList.SetActive(false);
        viewer.SetActive(true);
        viewImage.sprite = spriteRenderer.sprite;
    }

    public void OnClickDownBtn()
    {
        viewImageRect = viewImage.transform.parent.GetComponent<RectTransform>();

        if (viewImageRect.localScale.x > 1f)
        {
            viewImageRect.localScale /= 1.2f;
            if (viewImageRect.localScale.x < 0.8f)
                viewImageRect.localScale = Vector3.one;
        }
        else viewImageRect.localScale = Vector3.one;
    }

    public void OnClickUpBtn()
    {
        viewImageRect = viewImage.transform.parent.GetComponent<RectTransform>();

        if (viewImageRect.localScale.x < 5f)
        {
            viewImageRect.localScale *= 1.2f;
            if (viewImageRect.localScale.x > 5f - 1.2f)
                viewImageRect.localScale = Vector3.one * 5f;
        }
        else viewImageRect.localScale = Vector3.one * 5f;
    }

    public void OnClickReturnSizeBtn()
    {
        RectTransform rect = viewImage.transform.parent.GetComponent<RectTransform>();
        rect.localScale = Vector3.one;
    }

    public void InitializeViewer()
    {
        collectionList.SetActive(true);
        viewer.SetActive(false);
        OnClickReturnSizeBtn();
    }

    public void Init()
    {
        for (int i = 0; i < layoutGroup.childCount; i++)
        {
            Destroy(layoutGroup.GetChild(i).gameObject);
        }
        InitializeViewer();
    }

    public void OnClick_PnIDToggle(GameObject target)
    {
        InitializeViewer();
        target.SetActive(!target.activeSelf);
    }

    public void OnClick_PnIDList_CloseBtn(GameObject target)
    {
        PnID_Toggle.GetComponent<Interactable>().IsToggled = false;
        InitializeViewer();
        target.SetActive(false);
    }
}