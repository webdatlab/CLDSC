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

    public List<Texture2D> stateFlags;
    public RawImage hoverInfoFlag;

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

    // ---------------- CHART MANAGEMENT ----------------
    public Camera chartCamera;
    public Transform chartBase;
    public Transform chartGraphPrefab;
    public Transform chartSpawnOrigin;
    public Transform chartSpawnParent;
    public float chartSpawnLength = 30.0f;
    public Transform employmentTypePieChart; // employment types always add up to 100 per county, use this to create pie chart
    
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
        COUNT
    }    

    [System.Serializable]
    public class CountyField
    {
        public string name = "FieldName";
        public CountyFieldType type = CountyFieldType.NONE;
        public bool positiveContext = true;
        public float value = -1.0f;
    }

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

    public void SearchCounties()
    {
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
            //brokenOnWebGL[i].gameObject.SetActive(false);
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        stateHoverInfo.gameObject.SetActive(false);
        countyHoverInfo.gameObject.SetActive(false);
        SwitchToMapMode();
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
