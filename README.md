<img src="https://capsule-render.vercel.app/api?type=waving&color=auto&height=200&section=header&text=CyberPlant-MR&fontSize=80" />

# 🚀 MR 기반 EDG Local Control Panel 기동 절차 시스템

> **HoloLens 2 및 MR 환경에서 EDG Local Control Panel의 기동 절차를 시뮬레이션하는 프로젝트**  
> **MRTK를 활용한 MR 인터랙션 및 실시간 데이터 시각화 기능 포함**

---

## 📌 **프로젝트 개요**
- **개발 환경**: `Unity 2021.3 LTS`, `C#`, `MRTK`, `HoloLens2`
- **주요 기능**
  - 📋 **Panel 진행 절차 관리** (CSV 데이터 기반 자동화)
  - 🔍 **Panel 리스트 뷰어** (진행 절차 목록 제공)
  - 🎯 **Target 상태 표현** (애니메이션, 하이라이트 등)
  - 📊 **실시간 Graph 데이터 뷰어**
  - 🛠 **MRTK 적용** (가상 물체 조작)

---

## 📂 **프로젝트 구조**
📦 CyberPlant-MR <br>
├── 📁 Assets <br>
  &nbsp;&nbsp;&nbsp;├── 📁 Scripts # 주요 기능을 담당하는 C# 스크립트 <br>
  &nbsp;&nbsp;&nbsp;├── 📁 Prefabs # 사전 제작된 MR 오브젝트 <br>
  &nbsp;&nbsp;&nbsp;├── 📁 Scenes # Unity 씬 파일 <br> 
  &nbsp;&nbsp;&nbsp;├── 📁 Audio # 음성 및 효과음 리소스 <br> 
  &nbsp;&nbsp;&nbsp;├── 📁 Models # 3D 모델 데이터 <br> 
  &nbsp;&nbsp;&nbsp;├── 📁 Textures # 텍스처 및 UI 리소스 <br> 
  &nbsp;&nbsp;&nbsp;├── 📄 README.md # 프로젝트 개요 문서 <br>
  &nbsp;&nbsp;&nbsp;└── 📄 .gitignore

---

## 🎮 **주요 기능**

### 🟢 **1. Panel 진행 절차 관리** <a id="panel-procedure-projectmanager"></a>

> **사용된 스크립트**: `ProjectManager.cs`  
> Panel 기동 절차를 관리하고 CSV 데이터를 로드하여 단계별 실행을 담당합니다.

```c#
public void OnClickModeButton(string tableName)
{
    seqRowData = CSV.Read(tableName); // CSV 데이터 로드
    seqRowData.Sort((x, y) => int.Parse(x["No"]).CompareTo(int.Parse(y["No"])));
    this.tableName = tableName;
    SceneManager.LoadScene("Main");
}
```

➡ **📂 전체 코드 보기**: [ProjectManager.cs (L53-L59)](https://github.com/kalstjd96/KEPCO_VR_Scenario/blob/main/Scripts/ProjectManager.cs#L53-L59)

<br><br><br>


### 📋 **2. Panel 리스트 뷰어** <a id="panel-list-viewer"></a>
> **사용된 스크립트**: ProcedureList.cs
> 사용자가 진행해야 할 절차 목록을 리스트 형태로 보여줍니다.

```c#
public void Create()
{
    buttonList = new List<GameObject>();
    for (int i = 0; i < procedureRowDic.Count; i++)
    {
        int rowIndex = procedureRowDic.ElementAt(i).Key;
        GameObject listObj = Instantiate(listPrefab, listParent.transform);
        listObj.GetComponentInChildren<TMP_Text>().text = procedureRowDic.ElementAt(i).Value;
        buttonList.Add(listObj);
    }
}
```

➡ **📂 전체 코드 보기**: [ProcedureList.cs](https://github.com/kalstjd96/KEPCO_VR_Scenario/blob/main/Scripts/ProcedureList.cs)


<img src="https://user-images.githubusercontent.com/47016363/217998078-331fba74-9df0-4c51-ac18-9ff4d9780b5e.png" width="400"/>

<br><br><br>

### 🎯 **3. Target 상태 표현** <a id="target-state"></a>

> **사용된 스크립트**: Highlight.cs, GuideMessage.cs
> 진행 중인 Target을 강조하여 안내합니다. (애니메이션, 하이라이트 적용)

```c#
// 하이라이트 효과 적용
for (int i = 0; i < targets.Length; i++)
{
    targets[i].materials[0].SetColor("_Color",
        Color.Lerp(highlightMaterial.color, Color.white, Mathf.PingPong(Time.time, 1.5f)));
}
```

➡ **📂 전체 코드 보기**: [ProcedureList.cs](https://github.com/kalstjd96/KEPCO_VR_Scenario/blob/main/Scripts/Highlight.cs)<br>
➡ **📂 전체 코드 보기**: [ProcedureList.cs](https://github.com/kalstjd96/KEPCO_VR_Scenario/blob/main/Scripts/GuideMessage.cs)

<img src="https://user-images.githubusercontent.com/47016363/217998187-0a5727b9-833d-4189-af01-abc630d038c0.png" width="400"/>


<br><br><br>

### 📊 **4. Graph Data Viewer** <a id="graph-data-viewer"></a>
> 사용된 오픈소스 라이브러리
> 데이터베이스에서 값을 가져와 실시간으로 그래프를 렌더링합니다.

<img src="https://user-images.githubusercontent.com/47016363/217997541-07d916e2-a315-4baa-97a3-63c46751ec48.png" width="400"/>

<br><br><br>

### 🛠 **5. MRTK 적용 (HoloLens 2 인터랙션)** <a id="mrtk"></a>
> MRTK를 활용하여 가상 오브젝트 조작 기능을 구현했습니다.
> 사용자는 가상 물체의 크기, 위치, 회전 등을 조작할 수 있습니다.

<img src="https://user-images.githubusercontent.com/47016363/217986555-00894438-ebaa-4e50-9ef7-49df1b70e041.png" width="400"/> 
<img src="https://user-images.githubusercontent.com/47016363/217989203-7a7d481d-4426-46e0-8399-3153e20877ce.png" width="400"/>

<br><br><br>

## 🎥 실행 영상 (GIF)
<img src="images/Cyberplant_HoloLens2_EDGPanel.gif" width="800"/>
