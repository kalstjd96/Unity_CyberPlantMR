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

    #region Rendering Mode를 바꾸기 위해 사용한 코드
    public enum BlendMode
    {
        Opaque,
        Cutout,
        Fade,
        Transparent
    }

    public static void ChangeRenderMode(Material standardShaderMaterial, BlendMode blendMode)
    {
        switch (blendMode)
        {
            case BlendMode.Opaque:
                standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                standardShaderMaterial.SetInt("_ZWrite", 1);
                standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
                standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
                standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                standardShaderMaterial.renderQueue = -1;
                break;
            case BlendMode.Cutout:
                standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                standardShaderMaterial.SetInt("_ZWrite", 1);
                standardShaderMaterial.EnableKeyword("_ALPHATEST_ON");
                standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
                standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                standardShaderMaterial.renderQueue = 2450;
                break;
            case BlendMode.Fade:
                standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                standardShaderMaterial.SetInt("_ZWrite", 0);
                standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
                standardShaderMaterial.EnableKeyword("_ALPHABLEND_ON");
                standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                standardShaderMaterial.renderQueue = 3000;
                break;
            case BlendMode.Transparent:
                standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                standardShaderMaterial.SetInt("_ZWrite", 0);
                standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
                standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
                standardShaderMaterial.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                standardShaderMaterial.renderQueue = 3000;
                break;
        }

    }
    #endregion


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

                //수정 : 하나의 모델안의 메테리얼이 2개이상인 것도 존재하기 때문에 
                //이 상태로 하이라이팅 투명처리가 되지 않아 모드 변경
                //이 부분은 marterials의 색상을 변경 즉 기존 materials는 유지한 채 색상만 변경

                for (int j = 0; j < targets[i].materials.Length; j++)
                {
                    ChangeRenderMode(targets[i].materials[j], BlendMode.Transparent);
                    targets[i].materials[j] = targeMat;
                    targets[i].materials[j].SetColor("_Color",
                        Color.Lerp(targeMat.color, targetColor, Mathf.PingPong(Time.time, 1.5f)));
                }
                //Shader 특성 정확히 파악 필요
                //일반적으로 "_"를 붙여쓴다고 함 
                //Mathf은 숫자들을 다룰 때 많이 사용하는 클래스
                //해당 쉐이더 인스펙터창의 Properties의 있는 _~에 항목을 쓰는 것 
                //_항목은 스크립트에서 작업이 가능 유튜브 참고

                /* Mathf.PingPong(float value, float Max)
                 * value 값이 Max에 도달하면 -값이 되고 0이 되면 Max까지 반복해서 왔다갔다 한다.
                 * 
                 * Mathf.Lerp(float from, float to, float t)
                 * 시작점(from)과 종료점(to) 사이의 보간값(0과 1사이의 실수 값)(t)에 해당하는 값을 반환
                 * 이는 오브젝트를 부드럽게 이동 또는 회전할 때 많이 사용한다고 함
                 * 
                 */

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
                ///"Material doesn't have a color property '_Color' 에러가 발생하여 존재할때만 
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