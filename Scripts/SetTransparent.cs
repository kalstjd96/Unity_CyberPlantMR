using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTransparent : MonoBehaviour
{
    public GameObject sliderObj;
    PinchSlider pinchSlider;

    private void Awake()
    {
        GetComponent<Interactable>().OnClick.AddListener(delegate { OnClickSliderToggleBtn(); });
        pinchSlider = sliderObj.GetComponent<PinchSlider>();
        pinchSlider.OnValueUpdated.AddListener(delegate { OnValueChanged(); });
    }
    public void OnClickSliderToggleBtn()
    {
        sliderObj.SetActive(!sliderObj.activeSelf);
    }

    public void OnValueChanged()
    {
        if (pinchSlider.SliderValue == 1f)
        {
            ProjectManager.instance.Set_BlendMode(
            ProjectManager.instance.EDG_Control_Panel,
            Set_RendererMode.BlendMode.Opaque,
            1f);
        }
        else
        {
            ProjectManager.instance.Set_BlendMode(
            ProjectManager.instance.EDG_Control_Panel,
            Set_RendererMode.BlendMode.Transparent,
            pinchSlider.SliderValue);
        }
    }
}
