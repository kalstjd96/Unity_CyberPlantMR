using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphViewer : MonoBehaviour
{
    public GameObject graphViewer_Toggle;
    public GameObject graphList;
    public GameObject viewer;
    public Image viewImage;

    public void OnClickGraphBtn(Button button)
    {
        viewer.SetActive(true);
        if (viewImage)
            viewImage.sprite = button.GetComponent<Image>().sprite;
    }

    public void OnClick_ExpandViewerCloseBtn()
    {
        viewer.SetActive(false);
        viewer.GetComponentInChildren<ContentSizeFitter>().GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        graphList.SetActive(true);
        
        GetComponent<GraphEvent>().InitializeGraph();
    }

    public void OnClick_GraphViewerToggle(GameObject target)
    {
        target.SetActive(!target.activeSelf);

        graphList.SetActive(true);
        GetComponent<GraphEvent>().InitializeGraph();
    }

    public void OnClick_GraphListCloseBtn(GameObject target)
    {
        target.SetActive(false);
        graphList.SetActive(true);
        GetComponent<GraphEvent>().InitializeGraph();
        graphViewer_Toggle.GetComponent<Interactable>().IsToggled = false;
    }
}
