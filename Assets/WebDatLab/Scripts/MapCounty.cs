using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapCounty : MonoBehaviour
{
    public Manager manager;
    public Vector3 originalScale = Vector3.one;
    public float hoverHeightMod = 1.5f;
    public float cameraDistanceCheckTimer = 0.0f;
    public float cameraDistanceCheckDelay = 0.4f;
    public float cameraDistanceCheckRandom = 0.15f;
    public float cameraDistanceThreshold = 30.0f;
    public Vector3 originalPosition = Vector3.zero;
    public Vector3 hoverOffset = Vector3.zero;
    //public Manager.CountyData countyData;
    public int countyDataIndex;
    public MeshFilter meshFilter;

    public string formatCountyPercentage(float percentage, bool reverse = false)
    {
        string hexColor = "";
        if (reverse)
        {
            hexColor = ColorUtility.ToHtmlStringRGB(manager.percentageGradient.Evaluate(1.0f -(percentage * 0.02f)));
        }
        else
        {
            hexColor = ColorUtility.ToHtmlStringRGB(manager.percentageGradient.Evaluate(percentage * 0.02f));
        }

        return ("<color=#" + hexColor + ">" + percentage.ToString() + "</color>");
    }

    public void ScaleUp()
    {
        transform.localScale = new Vector3(originalScale.x, originalScale.y * hoverHeightMod, originalScale.z);
    }

    public Vector3 GetWorldSpaceFromMesh()
    {
        return transform.TransformPoint(meshFilter.mesh.vertices[0]);
    }

    void OnMouseEnter()
    {
        ScaleUp();
        manager.countyHoverInfo.gameObject.SetActive(true);
        manager.stateHoverInfo.gameObject.SetActive(true);
        manager.hoverInfoCountyTMPro.text = name;
        manager.countyNameTMPro.text = manager.countyData[countyDataIndex].name+", "+ manager.countyData[countyDataIndex].state;
        manager.countyNameTMPro.transform.position = new Vector3(GetWorldSpaceFromMesh().x, manager.countyNameTMPro.transform.position.y, GetWorldSpaceFromMesh().z);
        manager.SetHoverFlag(manager.countyData[countyDataIndex].state);

        try
        {
            string info = "<align=\"left\">County: </align> <b><size=160%><align=\"right\">" + manager.countyData[countyDataIndex].name;
            info += ("\n<u><b><color=#FFAAAA>----- ECOLOGICAL DATA -----</color></b></u>");
            info += ("\n</align>January Temp: <align=\"right\">" + manager.countyData[countyDataIndex].meanTempJan.ToString() + "°");
            info += ("\n</align>July Temp: <align=\"right\">" + manager.countyData[countyDataIndex].meanTempJul.ToString() + "°");
            info += ("\n</align>July Humidity: <align=\"right\">" + manager.countyData[countyDataIndex].meanHumidityJul.ToString());
            info += ("\n</align>Irrigated Acres: <align=\"right\">" + manager.countyData[countyDataIndex].irrigatedAcresPercent.ToString() + "%");
            info += ("\n</align>Manured Acres: <align=\"right\">" + manager.countyData[countyDataIndex].manureAcresPercent.ToString() + "%");
            info += ("\n</align>Harvested Acres: <align=\"right\">" + manager.countyData[countyDataIndex].harvestedAcresPercent.ToString() + "%");
            info += ("\n</align>AU by Acre: <align=\"right\">" + manager.countyData[countyDataIndex].animalUnitsPerAcrePercent.ToString() + "%");
            info += ("\n</align>Drought: <align=\"right\">" + manager.countyData[countyDataIndex].extremeDroughtPercent.ToString() + "%");
            info += ("\n</align>Water Area: <align=\"right\">" + manager.countyData[countyDataIndex].percentWaterArea.ToString() + "%");

            info += ("\n<u><b><color=#AAFFAA>----- ECONOMIC DATA -----</color></b></u>");
            info += ("\n</align>Unemployment: <align=\"right\">" + formatCountyPercentage(manager.countyData[countyDataIndex].unemploymentPercent, true) + "%");
            info += ("\n</align>Vet Unemployment: <align=\"right\">" + formatCountyPercentage(manager.countyData[countyDataIndex].veteransUnemploymentPercent, true) + "%");
            info += ("\n</align>In Poverty: <align=\"right\">" + formatCountyPercentage(manager.countyData[countyDataIndex].povertyPercent, true) + "%");
            info += ("\n</align>Child Poverty: <align=\"right\">" + formatCountyPercentage(manager.countyData[countyDataIndex].childPovertyPercent, true) + "%");
            info += ("\n</align>Vet Poverty: <align=\"right\">" + formatCountyPercentage(manager.countyData[countyDataIndex].veteransInPovertyPercent, true) + "%");
            info += ("\n</align>Median Income: <align=\"right\">$" + manager.countyData[countyDataIndex].medianIncome.ToString()+"/yr");
            info += ("\n</align>no HS Diploma: <align=\"right\">" + formatCountyPercentage(manager.countyData[countyDataIndex].noHSDiplomaPercent, true) + "%");
            info += ("\n</align>HS Diploma: <align=\"right\">" + manager.countyData[countyDataIndex].onlyHSDiplomaPercent.ToString()+"%");
            info += ("\n</align>Some College: <align=\"right\">" + manager.countyData[countyDataIndex].someCollegePercent.ToString() + "%");
            info += ("\n</align>Bachelors Degree: <align=\"right\">" + formatCountyPercentage(manager.countyData[countyDataIndex].bachelorsDegreePercent) + "%");


            info += ("\n<u><b><color=#AAAAFF>----- INDUSTRIAL DATA -----</color></b></u>");
            info += ("\n</align>Ag Employment: <align=\"right\">" + (manager.countyData[countyDataIndex].agricultureEmploymentPercent) + "%");
            info += ("\n</align>Mining Employment: <align=\"right\">" + (manager.countyData[countyDataIndex].miningEmploymentPercent) + "%");
            info += ("\n</align>Construction Employment: <align=\"right\">" + (manager.countyData[countyDataIndex].constructionEmploymentPercent) + "%");
            info += ("\n</align>Manufacturing Employment: <align=\"right\">" + (manager.countyData[countyDataIndex].manufacturingEmploymentPercent) + "%");
            info += ("\n</align>Trade Employment: <align=\"right\">" + (manager.countyData[countyDataIndex].tradeEmploymentPercent) + "%");
            info += ("\n</align>Transpo Employment: <align=\"right\">" + (manager.countyData[countyDataIndex].transportationEmploymentPercent) + "%");
            info += ("\n</align>InfoTech Employment: <align=\"right\">" + (manager.countyData[countyDataIndex].informationEmploymentPercent) + "%");
            info += ("\n</align>FireDept Employment: <align=\"right\">" + (manager.countyData[countyDataIndex].fireEmploymentPercent) + "%");
            info += ("\n</align>Service Employment: <align=\"right\">" + (manager.countyData[countyDataIndex].serviceEmploymentPercent) + "%");
            info += ("\n</align>Gov Employment: <align=\"right\">" + (manager.countyData[countyDataIndex].governmentEmploymentPercent) + "%");



            manager.hoverInfoCountyTMPro.text = info;
        } 
        catch(System.Exception e)
        {
            UnityEngine.Debug.LogError(e.Message);
            UnityEngine.Debug.LogError("COUNTY INDEX: "+countyDataIndex.ToString()+" NOT FOUND");
        }
    }

    void OnMouseExit()
    {
        transform.localScale = originalScale;
        manager.countyHoverInfo.gameObject.SetActive(false);
        manager.stateHoverInfo.gameObject.SetActive(false);
    }

    void OnMouseUpAsButton()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            manager.AddCountyToCompareList(name.ToLower());
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.name == "MapSwitcher")
        {
            GetComponent<Renderer>().enabled = true;
            transform.localPosition = originalPosition + hoverOffset;
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.name == "MapSwitcher")
        {
            GetComponent<Renderer>().enabled = false;
            transform.localPosition = originalPosition;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        manager = Manager.GetCurrentManager();
        GetComponent<Renderer>().enabled = false;
        originalPosition = transform.localPosition;
        originalScale = transform.localScale;
        // THIS HURTS PERFORMANCE - need to change to a set of 12 materials that are randomly chosen so batching will work 
        GetComponent<Renderer>().material.SetColor("_Color", (GetComponent<Renderer>().material.GetColor("_Color") * 0.75f) + (Random.ColorHSV() * 0.25f));
        meshFilter = GetComponent<MeshFilter>();
    }

    // Update is called once per frame
    void Update()
    {
        /*cameraDistanceCheckTimer += Time.deltaTime;
        if (cameraDistanceCheckTimer > cameraDistanceCheckDelay)
        {
            if (Vector3.Distance(transform.position, manager.mapSwitcher.position) > cameraDistanceThreshold)
            {
                GetComponent<Renderer>().enabled = false;
            }
            else
            {
                GetComponent<Renderer>().enabled = true;
            }
            cameraDistanceCheckTimer = 0.0f - Random.Range(0.0f, cameraDistanceCheckRandom);
        }*/
    }
}
