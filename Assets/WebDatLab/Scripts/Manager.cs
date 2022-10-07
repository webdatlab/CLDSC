using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    // Platform specifics
    public List<Transform> brokenOnWebGL;

    // ----------- MAP MANAGEMENT ------------
    public Camera mainCamera;
    public Material mapDefaultMaterial;
    public Material mapHoverMaterial;
    public Transform scrollViewContent;
    public Transform mapBase;
    public Transform stateMap;
    public List<Material> stateMapMaterials;
    public Transform countyMap;
    public List<Material> countyMapMaterials;
    public bool stateMapOn = true;
    public Transform mapSwitcher;
    public Transform mapTooltip;
    public Vector3 mapTooltipOffset = Vector3.zero;
    public TMPro.TextMeshProUGUI hoverInfoTMPro;
    public TMPro.TextMeshProUGUI hoverInfoStateTMPro;
    public TMPro.TextMeshProUGUI hoverInfoCountyTMPro;
    public Transform stateHoverInfo;
    public HoverInfo countyHoverInfo;
    public TMPro.TextMeshProUGUI countySearchTMPro;
    public TMPro.TextMeshPro countyNameTMPro;
    public Transform fieldSelectionPanel;
    public Transform fieldTogglePrefab;
    public bool useCustomFields = false;

    public List<MapCounty> mapCounties;
    public List<Texture2D> stateFlags;
    public RawImage hoverInfoFlag;
    public Transform countySearchResultPrefab;
    public List<Transform> countySearchResults;

    public float zoomMapSwitch = 40.0f;
    public float minZoom = 30.0f;
    public float maxZoom = 70.0f;
    public float zoomSpeed = 4.0f;

    public Transform stateInfoCardPrefab;
    public Transform countyInfoCardPrefab;
    public List<string> compareList;
    public Material stateWireFrameMAT;

    public bool mapMode = true;
    public Gradient percentageGradient;
    public Scrollbar compareScrollBar;
    public List<Color> initialMapColors;

    // ---------------- CHART MANAGEMENT ----------------
    public Camera chartCamera;
    public Transform chartBase;
    public Transform chartGraphPrefab;
    public Transform chartSpawnOrigin;
    public Transform chartSpawnParent;
    public float chartSpawnLength = 30.0f;
    public List<TMPro.TextMeshProUGUI> chartFieldTitles;
    public Transform employmentTypeieChart; // employment types always add up to 100 per county, use this to create pie chart
    
    public class ChartField
    {
        public string name = "";
        public Color barColor = Color.blue;
    }

    [System.Serializable]
    public class StateData
    {
        public string name = "";
        public string description = "";
    }
    public Dictionary<string, StateData> stateData;


    [System.Serializable]
    public enum CountyFieldDataSetType
    {
        NONE,
        USDA_NATURAL_AMENITIES,
        USDA_ECONOMIC_RESEARCH,
        USDA_RURAL_DATA_ATLAS
    }

    [System.Serializable]
    public class CountyFieldDataSet
    {
        public string name = "DataSet Name";
        public string description = "This dataset still needs a description";
        public string URL = "http://www.usda.gov/";
        public CountyFieldDataSetType countyFieldDataSetType = CountyFieldDataSetType.NONE;
    }

    [System.Serializable]
    public enum CountyFieldType
    {
        NONE,
        PERCENT,
        COUNT,
        CHANGE,
    }    

    [System.Serializable]
    public class CountyField
    {
        public string name = "FieldName";
        public string description = "Field Description";
        public CountyFieldType type = CountyFieldType.NONE;
        public bool positiveContext = true;
        public float value = -1.0f;
        public Toggle fieldToggle;
        public bool selected = false;
    }
    public List<CountyField> countyFields;

    [System.Serializable]
    public class CountyData
    {
        public string name;
        public int fips = -1;
        public string state;
        public int censusArea;
        public int ruralUrbanContinuum;
        public int urbanInfluence;
        public float percentWaterArea;
        public float povertyPercent;
        public float medianIncome;
        public float noHSDiplomaPercent;
        public float onlyHSDiplomaPercent;
        public float someCollegePercent;
        public float bachelorsDegreePercent;
        public float meanTempJan;
        public float meanSunlightJan;
        public float meanTempJul;
        public float meanHumidityJul;
        public float topographyCode;
        public float childPovertyPercent;
        public float immigrantPercent;
        public float ownHomePercent;
        public float unemploymentPercent;
        public float agricultureEmploymentPercent;
        public float miningEmploymentPercent;
        public float constructionEmploymentPercent;
        public float manufacturingEmploymentPercent;
        public float tradeEmploymentPercent;
        public float transportationEmploymentPercent;
        public float informationEmploymentPercent;
        public float fireEmploymentPercent;
        public float serviceEmploymentPercent;
        public float governmentEmploymentPercent;
        public float veteransInPovertyPercent;
        public float veteransUnemploymentPercent;
        public float oilBarrelProduction2011;
        public float gas1kCubicFeetProduction2011;
        public float oilChange;
        public float gasChange;
        public float airPollutantTons;
        public float airPollutantPPM;
        public float waterPollutantMGL;
        public float irrigatedAcresPercent;
        public float chemicalNematodeControlAcresPercent;
        public float manureAcresPercent;
        public float chemicalDiseaseControlAcresPercent;
        public float chemicallyDefoliatedAcresPercent;
        public float harvestedAcresPercent;
        public float animalUnitsPerAcrePercent;
        public float extremeDroughtPercent;
        public float publicTransitPercent;
        public float carFatalitiesPer100k;
        public float averageCommuteTimeMinutes;
        public float bingeDrinkingPercent;
        public float noHealthInsurancePercent;
        public float cancerDiagnosisPercent;
        public float athsmaDiagnosisPercent;
        public float depressionPercent;
        public float poorMentalHealthPercent;
        public float obesityPercent;
        public float strokePercent;
        public float noLeisureTimePercent;
        public float smokerPercent;
    }

    public CountyData[] countyData;

    [System.Serializable]
    public class CountyJSON
    {
        public Dictionary<string, CountyData> jsonData;
    }
    public CountyJSON countyJSON = null;

    public CountyData GetCountyData(string name)
    {
        int fips = int.Parse(name.Replace("_", ""));
        for(int i = 0; i < countyData.Length; i++)
        {
            if(countyData[i].fips == fips)
            {
                return countyData[i];
            }
        }
        return null;
    }

    public CountyData GetCountyData(int fips)
    {
        for (int i = 0; i < countyData.Length; i++)
        {
            if (countyData[i].fips == fips)
            {
                return countyData[i];
            }
        }
        return null;
    }

    public int GetCountyFipsFromName(string name)
    {
        return int.Parse(name.Replace("_", ""));
    }

    public void SearchCounties()
    {
        for(int i = 0; i < countySearchResults.Count; i++)
        {
            Destroy(countySearchResults[i].gameObject);
        }
        countySearchResults.Clear();

        string county = countySearchTMPro.text.Trim().ToLower();
        string countySearch = System.Text.Encoding.ASCII.GetString(System.Text.Encoding.ASCII.GetBytes(county)).Replace("?", "");
        if(countySearch.Length <= 3) { return; }
        for (int i = 0; i < mapCounties.Count; i++)
        {
            string mapCounty = countyData[mapCounties[i].countyDataIndex].name.ToLower();
            string mapCountyAscii = System.Text.Encoding.ASCII.GetString(System.Text.Encoding.ASCII.GetBytes(mapCounty));

            if (mapCountyAscii.Contains(countySearch))
            {
                Vector3 pos = new Vector3(mapCounties[i].GetWorldSpaceFromMesh().x, countyNameTMPro.transform.position.y, mapCounties[i].GetWorldSpaceFromMesh().z);
                Transform newSearchResult = Instantiate(countySearchResultPrefab, pos, countyNameTMPro.transform.rotation) as Transform;
                newSearchResult.parent = mapBase;
                newSearchResult.GetComponent<TMPro.TextMeshPro>().text = countyData[mapCounties[i].countyDataIndex].name + ", " + countyData[mapCounties[i].countyDataIndex].state;
                countySearchResults.Add(newSearchResult);
            }
        }  

        /*System.Text.Encoding ascii = System.Text.Encoding.ASCII;
        string county = countySearchTMPro.text.Trim().ToLower();
        for(int i = 0; i < countyData.Length; i++)
        {
            UnityEngine.Debug.Log("COMPARING: [" + county +"] in ["+countyData[i].name.ToLower() + "]");
            //Regex rgx = new Regex(@"\b" + county + @"\b");
            //Match match = rgx.Match(countyData[i].name.ToLower());
            string countySearch = ascii.GetString(ascii.GetBytes(county));
            string countyMatch = ascii.GetString(ascii.GetBytes(countyData[i].name.ToLower()));
            if (countyMatch.Contains(countySearch)) //(match.Success) //(countyData[i].name.ToLower().Contains(county.ToLower()))
            {
                try
                {
                    string fips = "_" + countyData[i].fips.ToString().PadLeft(5, '0');
                    UnityEngine.Debug.Log("FIPS SEARCH: "+fips);
                       
                    GameObject.Find(fips).GetComponent<MapCounty>().ScaleUp();
                }
                catch(System.Exception e)
                {
                    UnityEngine.Debug.LogError(e.Message);
                }
            }
            break;
        }*/
    }

    public void AddStateToCompareList(string name)
    {
        if (compareList.IndexOf(name.ToLower()) == -1)
        {
            Transform newCard = Instantiate(stateInfoCardPrefab, Vector3.zero, Quaternion.identity) as Transform;
            newCard.parent = scrollViewContent;
            newCard.localScale = Vector3.one;
            RectTransformExtensions.SetHeight(newCard.GetComponent<RectTransform>(), 70.0f);
            newCard.GetComponent<StateInfoCard>().stateAbbreviation = name.ToUpper();
            newCard.GetComponentInChildren<RawImage>().texture = GetStateFlag(name);
            newCard.Find("Name").GetComponent<TMPro.TextMeshProUGUI>().text = name.ToUpper();
            compareList.Add(name);
        }
    }

    public void AddCountyToCompareList(string name)
    {
        if (compareList.IndexOf(name.ToLower()) == -1)
        {
            CountyData cd = GetCountyDataByFips(name);
            Transform newCard = Instantiate(countyInfoCardPrefab, Vector3.zero, Quaternion.identity) as Transform;
            newCard.parent = scrollViewContent;
            newCard.localScale = Vector3.one;
            RectTransformExtensions.SetHeight(newCard.GetComponent<RectTransform>(), 70.0f);
            newCard.GetComponent<CountyInfoCard>().countyName = (cd.name+" County, "+cd.state);
            newCard.GetComponent<CountyInfoCard>().fipsName = ("_" + cd.fips.ToString().PadLeft(5, '0'));
            newCard.Find("Name").GetComponent<TMPro.TextMeshProUGUI>().text = name;
            compareList.Add(name);
        }
    }

    public CountyData GetCountyDataByFips(string name)
    {
        for(int i = 0; i < countyData.Length; i++)
        {
            if ("_"+countyData[i].fips.ToString().PadLeft(5, '0') == name)
            {
                return countyData[i];
            }
        }
        return null;
    }

    public void SwitchToMapMode()
    {
        StatsGraph[] statsGraphs = GameObject.FindObjectsOfType<StatsGraph>();
        for (int i = 0; i < statsGraphs.Length; i++)
        {
            Destroy(statsGraphs[i].gameObject);
        }

        mapBase.gameObject.SetActive(true);
        mainCamera.gameObject.SetActive(true);
        chartBase.gameObject.SetActive(false);
        chartCamera.gameObject.SetActive(false);
        mapMode = true;
        compareScrollBar.value = 1.0f;
    }

    public void SwitchToChartMode()
    {

        float spawnLength = chartSpawnLength * (1f / (float)compareList.Count);

        if(useCustomFields)
        {
            List<string> selectedFields = GetSelectedFields();
            for (int i = 0; i < 5; i++)
            {
                if (selectedFields.Count > i)
                {
                    chartFieldTitles[i].text = selectedFields[i];
                } 
                else
                {
                    chartFieldTitles[i].text = "";
                }
            }
        }

        // Generate charts
        for (int i = 0; i < compareList.Count; i++)
        {
            Transform newChart = Instantiate(chartGraphPrefab, chartSpawnOrigin.position, chartSpawnOrigin.rotation);
            newChart.parent = chartSpawnParent;
            newChart.localPosition = new Vector3(newChart.localPosition.x-(spawnLength*i), newChart.localPosition.y, newChart.localPosition.z);
            // Get list of county data IDs for comparison
            if(compareList[i].IndexOf("_") != -1)
            {
                try
                {
                    UnityEngine.Debug.Log("compareList[i]: " + compareList[i]);
                    GameObject go = GameObject.Find(compareList[i]);
                    MapCounty mc = go.GetComponent<MapCounty>();
                    StatsGraph sg = newChart.GetComponent<StatsGraph>();
                    sg.countyDataIDs.Add(mc.countyDataIndex);
                    sg.chartTitle.text = GetCountyDataByFips(compareList[i]).name +","+ GetCountyDataByFips(compareList[i]).state;
                } 
                catch(System.Exception e)
                {
                    UnityEngine.Debug.LogError(e.Message);
                    Destroy(newChart.gameObject);
                    continue;
                }
            }
            else
            {
                // It's a state
                for(int j = 0; j < countyData.Length; j++)
                {
                    if(countyData[j].state.ToLower() == compareList[i])
                    {
                        newChart.GetComponent<StatsGraph>().countyDataIDs.Add(j);
                    }
                }
                newChart.GetComponent<StatsGraph>().chartTitle.text = compareList[i].ToUpper();
            }
        }

        mapBase.gameObject.SetActive(false);
        mainCamera.gameObject.SetActive(false);
        chartBase.gameObject.SetActive(true);
        chartCamera.gameObject.SetActive(true);
        mapMode = true;
    }

    public void SaveChart()
    {
        ScreenCapture.CaptureScreenshot("CompareGraph_"+ System.DateTime.Now.ToString("MMMMddmmttss")+".png", 8);
    }

    public void Zoom(float deltaMagnitudeDiff)
    {
        mainCamera.transform.localPosition = new Vector3(mainCamera.transform.localPosition.x, mainCamera.transform.localPosition.y + (deltaMagnitudeDiff), mainCamera.transform.localPosition.z);
        if (mainCamera.transform.localPosition.y > maxZoom)
        {
            mainCamera.transform.localPosition = new Vector3(mainCamera.transform.localPosition.x, maxZoom, mainCamera.transform.localPosition.z);
        }
        if (mainCamera.transform.localPosition.y < minZoom)
        {
            mainCamera.transform.localPosition = new Vector3(mainCamera.transform.localPosition.x, minZoom, mainCamera.transform.localPosition.z);
        }
    }

    void Start()
    {
        for(int i = 0; i < brokenOnWebGL.Count; i++)
        {
            brokenOnWebGL[i].gameObject.SetActive(false);
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        stateHoverInfo.gameObject.SetActive(false);
        countyHoverInfo.gameObject.SetActive(false);
        fieldSelectionPanel.gameObject.SetActive(false);
        SwitchToMapMode();

        // Build field selection panel
        for(int i = 0; i < countyFields.Count; i++)
        {
            Transform newFieldToggle = Instantiate(fieldTogglePrefab, Vector3.zero, Quaternion.identity) as Transform;
            newFieldToggle.parent = fieldSelectionPanel;
            newFieldToggle.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = countyFields[i].name;
            newFieldToggle.transform.localScale = Vector3.one;
            countyFields[i].fieldToggle = newFieldToggle.GetComponent<Toggle>();
        }

        mapCounties = new List<MapCounty>(GameObject.FindObjectsOfType<MapCounty>());
    }

    

    public void ShowFieldSelectionPanel()
    {
        fieldSelectionPanel.gameObject.SetActive(true);
    }

    public void HideFieldSelectionPanel()
    {
        fieldSelectionPanel.gameObject.SetActive(false);
    }

    public void ToggleFieldSelectionPanel()
    {
        fieldSelectionPanel.gameObject.SetActive(!fieldSelectionPanel.gameObject.activeInHierarchy);
    }

    public List<string> GetSelectedFields()
    {
        List<string> selectedFields = new List<string>();
        for (int i = 0; i < countyFields.Count; i++)
        {
            if(countyFields[i].fieldToggle.isOn)
            {
                selectedFields.Add(countyFields[i].name);
            }
        }
        return selectedFields;
    }

    public float GetCountyPercentFromFieldName(int FIPS, string name)
    {
        CountyData cd = GetCountyData(FIPS);
        if(cd == null) { return 0.0f; }
        if (name == "percentWaterArea") { return cd.percentWaterArea; }
        if (name == "agricultureEmploymentPercent") { return cd.agricultureEmploymentPercent; }
        if (name == "animalUnitsPerAcrePercent") { return cd.animalUnitsPerAcrePercent; }
        if (name == "athsmaDiagnosisPercent") { return cd.athsmaDiagnosisPercent; }
        if (name == "bachelorsDegreePercent") { return cd.bachelorsDegreePercent; }
        if (name == "bingeDrinkingPercent") { return cd.bingeDrinkingPercent; }
        if (name == "cancerDiagnosisPercent") { return cd.cancerDiagnosisPercent; }
        if (name == "chemicalDiseaseControlAcresPercent") { return cd.chemicalDiseaseControlAcresPercent; }
        if (name == "chemicallyDefoliatedAcresPercent") { return cd.chemicallyDefoliatedAcresPercent; }
        if (name == "chemicalNematodeControlAcresPercent") { return cd.chemicalNematodeControlAcresPercent; }
        if (name == "childPovertyPercent") { return cd.childPovertyPercent; }
        if (name == "constructionEmploymentPercent") { return cd.constructionEmploymentPercent; }
        if (name == "percentWaterdepressionPercentArea") { return cd.depressionPercent; }
        if (name == "extremeDroughtPercent") { return cd.extremeDroughtPercent; }
        if (name == "fireEmploymentPercent") { return cd.fireEmploymentPercent; }
        if (name == "governmentEmploymentPercent") { return cd.governmentEmploymentPercent; }
        if (name == "harvestedAcresPercent") { return cd.harvestedAcresPercent; }
        if (name == "immigrantPercent") { return cd.immigrantPercent; }
        if (name == "informationEmploymentPercent") { return cd.informationEmploymentPercent; }
        if (name == "irrigatedAcresPercent") { return cd.irrigatedAcresPercent; }
        if (name == "manufacturingEmploymentPercent") { return cd.manufacturingEmploymentPercent; }
        if (name == "manureAcresPercent") { return cd.manureAcresPercent; }
        if (name == "noHealthInsurancePercent") { return cd.noHealthInsurancePercent; }
        if (name == "noHSDiplomaPercent") { return cd.noHSDiplomaPercent; }
        if (name == "noLeisureTimePercent") { return cd.noLeisureTimePercent; }
        if (name == "obesityPercent") { return cd.obesityPercent; }
        if (name == "onlyHSDiplomaPercent") { return cd.onlyHSDiplomaPercent; }
        if (name == "ownHomePercent") { return cd.ownHomePercent; }
        if (name == "poorMentalHealthPercent") { return cd.poorMentalHealthPercent; }
        if (name == "povertyPercent") { return cd.povertyPercent; }
        if (name == "publicTransitPercent") { return cd.publicTransitPercent; }
        if (name == "serviceEmploymentPercent") { return cd.serviceEmploymentPercent; }
        if (name == "smokerPercent") { return cd.smokerPercent; }
        if (name == "someCollegePercent") { return cd.someCollegePercent; }
        if (name == "strokePercent") { return cd.strokePercent; }
        if (name == "tradeEmploymentPercent") { return cd.tradeEmploymentPercent; }
        if (name == "transportationEmploymentPercent") { return cd.transportationEmploymentPercent; }
        if (name == "unemploymentPercent") { return cd.unemploymentPercent; }
        if (name == "veteransInPovertyPercent") { return cd.veteransInPovertyPercent; }
        if (name == "veteransUnemploymentPercent") { return cd.veteransUnemploymentPercent; }
        return 0.0f;
    }

    public Color GetCountyColorForSelectedFields(int FIPS)
    {
        List<float> percents = new List<float>();
        for (int i = 0; i < countyFields.Count; i++)
        {
            if (countyFields[i].fieldToggle.isOn)
            {
                if (countyFields[i].type == CountyFieldType.PERCENT)
                {
                    if (countyFields[i].positiveContext)
                    {
                        percents.Add(GetCountyPercentFromFieldName(FIPS, countyFields[i].name));
                    }
                    else
                    {
                        percents.Add(100.0f - GetCountyPercentFromFieldName(FIPS, countyFields[i].name));
                    }
                }
            }
        }

        float avgPercent = 0.0f;
        for(int i = 0; i < percents.Count; i++)
        {
            avgPercent += percents[i];
        }
        avgPercent = avgPercent * (1.0f / percents.Count);

        if(avgPercent <= 0.0f || avgPercent > 100.0f || avgPercent == float.NaN)
        {
            return initialMapColors[Random.Range(0, initialMapColors.Count)];
        }
        else 
        {
            return percentageGradient.Evaluate(Mathf.Clamp(avgPercent * 0.01f, 0.0f, 1.0f));
        }
    }
    
    public void HandleFieldToggle()
    {
        // Loop through fields, set county/state colors based on values for selection 
        List<string> selectedFields = GetSelectedFields();

        if(selectedFields.Count == 0)
        {
            for (int i = 0; i < mapCounties.Count; i++)
            {
                mapCounties[i].ResetCountyMeshColor();
            }
            useCustomFields = false;
        }
        else 
        {
            for (int i = 0; i < mapCounties.Count; i++)
            {
                mapCounties[i].SetCountyMeshColor(GetCountyColorForSelectedFields(GetCountyFipsFromName(mapCounties[i].gameObject.name)));
            }
            useCustomFields = true;
        }
    }

    public void SetHoverFlag(string name)
    {
        hoverInfoStateTMPro.text = name;
        hoverInfoFlag.texture = GetStateFlag(name);
    }

    public Texture2D GetStateFlag(string name)
    {
        for (int i = 0; i < stateFlags.Count; i++)
        {
            if (stateFlags[i].name.ToLower() == name.ToLower())
            {
                return stateFlags[i];
            }
        }
        return null;
    }

    public Vector3 GetMeshCenterPosition(Mesh mesh)
    {
        Vector3 avgPos = Vector3.zero;
        for(int i = 0; i < mesh.vertices.Length; i++)
        {
            avgPos += mesh.vertices[i];
        }
        return(avgPos / mesh.vertices.Length);
    }

    void Update()
    {
        Zoom(Input.mouseScrollDelta.y * -zoomSpeed * Time.deltaTime);


        /*if (mainCamera.transform.localPosition.y > zoomMapSwitch)
        {
            if(!stateMapOn)
            {
                SwitchToStateMap();
                mainCamera.transform.localPosition = new Vector3(mainCamera.transform.localPosition.x, zoomMapSwitch + 0.01f, mainCamera.transform.localPosition.z);
            }
        }
        else
        {
            if(stateMapOn)
            {
                SwitchToCountyMap();
                mainCamera.transform.localPosition = new Vector3(mainCamera.transform.localPosition.x, zoomMapSwitch - 0.01f, mainCamera.transform.localPosition.z);
            }
        }*/
    }

    public static Manager GetCurrentManager()
    {
        return GameObject.FindObjectOfType<Manager>();
    }

    public void SwitchToCountyMap()
    {
        stateMap.gameObject.SetActive(false);
        countyMap.gameObject.SetActive(true);
        stateMapOn = false;
    }

    public void SwitchToStateMap()
    {
        stateMap.gameObject.SetActive(true);
        countyMap.gameObject.SetActive(false);
        stateMapOn = true;
    }
}
