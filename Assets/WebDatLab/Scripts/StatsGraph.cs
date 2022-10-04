using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsGraph : MonoBehaviour
{
    public List<int> countyDataIDs;
    public Manager.CountyData chartData;
    public Manager manager;
    public TMPro.TextMeshPro chartTitle;

    public Transform chartBar01;
    public Transform chartBar02;
    public Transform chartBar03;
    public Transform chartBar04;
    public Transform chartBar05;

    public Transform chartTextPoint01;
    public Transform chartTextPoint02;
    public Transform chartTextPoint03;
    public Transform chartTextPoint04;
    public Transform chartTextPoint05;

    public TMPro.TextMeshPro chartValueTMPro01;
    public TMPro.TextMeshPro chartValueTMPro02;
    public TMPro.TextMeshPro chartValueTMPro03;
    public TMPro.TextMeshPro chartValueTMPro04;
    public TMPro.TextMeshPro chartValueTMPro05;

    /*public float textScaleHoverMod = 2.0f;
    public Vector3 chartValueOriginalScale01;
    public Vector3 chartValueOriginalScale02;
    public Vector3 chartValueOriginalScale03;
    public Vector3 chartValueOriginalScale04;
    public Vector3 chartValueOriginalScale05;*/

    public float maxBarScale = 3.7f;

    public float FixNaN(float value)
    {
        if(float.IsNaN(value))
        {
            return 0f;
        }
        else
        {
            return value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (manager == null) { manager = Manager.GetCurrentManager(); }
        for(int i = 0; i < countyDataIDs.Count; i++)
        {
            if(i == 0)
            {
                chartData = manager.countyData[countyDataIDs[i]];
            }
            else
            {
                chartData.bachelorsDegreePercent += manager.countyData[countyDataIDs[i]].bachelorsDegreePercent;
                chartData.medianIncome += manager.countyData[countyDataIDs[i]].medianIncome;
                chartData.noHSDiplomaPercent += manager.countyData[countyDataIDs[i]].noHSDiplomaPercent;
                chartData.percentWaterArea += manager.countyData[countyDataIDs[i]].bachelorsDegreePercent;
                chartData.povertyPercent += manager.countyData[countyDataIDs[i]].povertyPercent;
            }
        }

        chartData.bachelorsDegreePercent *= (1f / (float)countyDataIDs.Count);
        chartData.medianIncome *= (1f / (float)countyDataIDs.Count);
        chartData.noHSDiplomaPercent *= (1f / (float)countyDataIDs.Count);
        chartData.percentWaterArea *= (1f / (float)countyDataIDs.Count);
        chartData.povertyPercent *= (1f / (float)countyDataIDs.Count);


        chartBar01.localScale = new Vector3(chartBar01.localScale.x, FixNaN(chartData.noHSDiplomaPercent * 0.01f) * maxBarScale, chartBar01.localScale.z);
        chartBar02.localScale = new Vector3(chartBar01.localScale.x, FixNaN(chartData.bachelorsDegreePercent * 0.01f) * maxBarScale, chartBar01.localScale.z);
        chartBar03.localScale = new Vector3(chartBar01.localScale.x, FixNaN(chartData.medianIncome * 0.00001f) * maxBarScale, chartBar01.localScale.z);
        chartBar04.localScale = new Vector3(chartBar01.localScale.x, FixNaN(chartData.percentWaterArea * 0.01f) * maxBarScale, chartBar01.localScale.z);
        chartBar05.localScale = new Vector3(chartBar01.localScale.x, FixNaN(chartData.povertyPercent * 0.01f) * maxBarScale, chartBar01.localScale.z);

        chartValueTMPro01.text = chartData.noHSDiplomaPercent.ToString("0.0") + "%";
        chartValueTMPro02.text = chartData.bachelorsDegreePercent.ToString("0.0") + "%";
        chartValueTMPro03.text = "$"+chartData.medianIncome.ToString("0") + "/yr";
        chartValueTMPro04.text = chartData.percentWaterArea.ToString("0.0") + "%";
        chartValueTMPro05.text = chartData.povertyPercent.ToString("0.0") + "%";

        chartValueTMPro01.transform.position = chartTextPoint01.position;
        chartValueTMPro02.transform.position = chartTextPoint02.position;
        chartValueTMPro03.transform.position = chartTextPoint03.position;
        chartValueTMPro04.transform.position = chartTextPoint04.position;
        chartValueTMPro05.transform.position = chartTextPoint05.position;


        /*chartValueOriginalScale01 = chartValueTMPro01.transform.localScale;
        chartValueOriginalScale02 = chartValueTMPro02.transform.localScale;
        chartValueOriginalScale03 = chartValueTMPro03.transform.localScale;
        chartValueOriginalScale04 = chartValueTMPro04.transform.localScale;
        chartValueOriginalScale05 = chartValueTMPro05.transform.localScale;*/
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
