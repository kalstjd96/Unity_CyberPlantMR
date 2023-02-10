using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Highlight : MonoBehaviour
{
    public static Highlight instance { get; private set; }
    [SerializeField] Material highlightMaterial;
    MeshRenderer[] targets;
    Material[] defaultMaterials;
    List<Color[]> colorList;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void OnDestroy()
    {
        if (instance != null)
            instance = null;
    }

    //컨트롤러를 댓을 때 테두리 지정하기 위해서는 HighlighterController.cs 참고
    public IEnumerator HighlightOn()
    {
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
                    ChangeRenderMode(targets[i].materials[j], BlendMode.Transparent);
                    targets[i].materials[j] = targeMat;
                    targets[i].materials[j].SetColor("_Color",
                        Color.Lerp(targeMat.color, targetColor, Mathf.PingPong(Time.time, 1.5f)));
                }
               
            }
            yield return null;
        }
    }

    //해당 모델의 색상을 저장하고 하이라이트 동작을 호출
    public void On(GameObject[] models , bool state)
    {
        targets = new MeshRenderer[models.Length];
        defaultMaterials = new Material[models.Length];
        colorList = new List<Color[]>();

        for (int i = 0; i < models.Length; i++)
        {
            //하이라이트 해줘야 하는 모델들의 렌더를 넣는 부분
            targets[i] = models[i].GetComponent<MeshRenderer>();

            Color[] oriColor = new Color[targets[i].materials.Length];
            for (int j = 0; j < oriColor.Length; j++)
            {
                if (targets[i].materials[j].HasProperty("_Color"))
                {
                    Color color = targets[i].materials[j].color;
                    color.a = 1;
                    oriColor.SetValue(color, j);
                }
            }
            colorList.Add(oriColor);
            defaultMaterials[i] = targets[i].material;
        }

        if (state)
        {
            StartCoroutine(HighlightOn());
        }
    }

    public void Off()
    {
        StopAllCoroutines();

        if (targets != null && targets.Length > 0)
        {
            for (int i = 0; i < targets.Length; i++)
            {
                Material[] materials = targets[i].materials;
                
                for (int j = 0; j < materials.Length; j++)
                {
                    Color color = colorList[i][j];
                    color.a = 1;
                    targets[i].materials[j].color = color;
                    //defaultColors.Add(color);
                    //defaultMaterials[i] = targets[i].materials[j];
                    //렌더러 모드 되돌리기
                    ChangeRenderMode(targets[i].materials[j], BlendMode.Opaque);

                }
            }
        }
    }
}
