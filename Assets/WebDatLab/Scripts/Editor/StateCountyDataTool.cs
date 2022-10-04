using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;
using System.IO;

[ExecuteInEditMode]
public class StateCountyDataTool : MonoBehaviour
{
    [MenuItem("Assets/Rebuild State and County Data")]
    public static void RebuildStateAndCountyData()
    {
        /*Manager.StateData[] stateData = null;
        stateData = JsonConvert.DeserializeObject<Manager.StateData[]>(File.ReadAllText(@"PyDataPrep/MergedStateData.json"));*/

        Manager manager = Manager.GetCurrentManager();
        manager.countyData = JsonConvert.DeserializeObject<Manager.CountyData[]>(File.ReadAllText(@"PyDataPrep/MergedCountyData_array.json"));

        for(int i = 0; i < manager.countyData.Length; i++)
        {
            string fips = "_" + manager.countyData[i].fips.ToString().PadLeft(5, '0');
            try
            {
                GameObject go = GameObject.Find(fips);
                go.GetComponent<MapCounty>().countyDataIndex = i;

                /*if (manager.countyData[i].fips == 48373)
                {
                    UnityEngine.Debug.LogWarning("POLK??");
                }*/
            } 
            catch(System.Exception e)
            {
                UnityEngine.Debug.Log("SEARCHING FOR FIPS: " + fips);
                UnityEngine.Debug.LogError(e.Message);
            }
        }

        MapCounty[] mapCounties = GameObject.FindObjectsOfType<MapCounty>();
        UnityEngine.Debug.LogWarning("FOUND " + mapCounties.Length.ToString() + " MapCounty OBJECTS");
        for (int i = 0; i < mapCounties.Length; i++) 
        { 
            if(mapCounties[i].countyDataIndex == -1)
            {
                UnityEngine.Debug.LogError("MAP COUNTY "+mapCounties[i].name+" HAS NO DATA");
            }
        }

        //CreateShapeWithTriangles("WV", meshInfo.vertices, meshInfo.triangles);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
