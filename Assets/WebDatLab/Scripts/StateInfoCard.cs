using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StateInfoCard : MonoBehaviour
{
    public string stateAbbreviation = "";
    public Manager manager;
    public TMPro.TextMeshProUGUI nameTMPro;
    public TMPro.TextMeshProUGUI description;
    public RawImage flag;

    public void CloseCard()
    {
        manager.compareList.Remove(stateAbbreviation.ToLower());    
        Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        manager = Manager.GetCurrentManager();
        Invoke("Init", 0.5f);
    }

    public void Init()
    {
        if (flag != null) 
        {
            flag.texture = manager.GetStateFlag(stateAbbreviation);
            nameTMPro.text = stateAbbreviation;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
