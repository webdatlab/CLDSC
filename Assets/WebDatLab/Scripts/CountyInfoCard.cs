using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountyInfoCard : MonoBehaviour
{
    public string countyName = "";
    public string fipsName = "";
    public Manager manager;
    public TMPro.TextMeshProUGUI nameTMPro;
    public TMPro.TextMeshProUGUI description;

    public void CloseCard()
    {
        manager.compareList.Remove(fipsName.ToLower());
        Destroy(gameObject);
    }

    void Start()
    {
        manager = Manager.GetCurrentManager();
        Invoke("Init", 0.5f);
    }

    public void Init()
    {
        nameTMPro.text = countyName;
    }
}
