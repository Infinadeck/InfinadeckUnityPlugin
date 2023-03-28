using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InfinaDEMO : MonoBehaviour
{
    public Text DTRTextN;
    public Text DTRTextE;
    public Text DTRTextS;
    public Text DTRTextW;
    public GameObject holder;
    public int demoTimeRemaining = 120;
    public int demoTime = 120;
    private bool init = false;
    private bool counting = false;

    public InfinaDATA preferences;
    public InfinadeckInterpreter iI;



    void OnEnable()
    {
        if (FindObjectOfType<InfinadeckReferenceObjects>())
        {
            holder = FindObjectOfType<InfinadeckReferenceObjects>().gameObject;
            this.transform.parent = holder.transform;
            this.transform.localPosition = Vector3.zero;
            this.transform.localRotation = Quaternion.identity;
            this.transform.localScale = Vector3.one;
        }
        init = true;
        StartCoroutine(DecrementDemoTime());
    }

    void OnDisable()
    {
        if (counting)
        {
            StopCoroutine(DecrementDemoTime());
            counting = false;
        }
    }

    private IEnumerator DecrementDemoTime()
    {
        counting = true;
        yield return new WaitForSeconds(1.0f);
        while (true)
        {
            if (iI.InfIntGetTreadmillRunState)
            {
                demoTimeRemaining--;
            }
            yield return new WaitForSeconds(1.0f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!init) { return; }

        if (demoTimeRemaining <= 0) {
            demoTimeRemaining = demoTime;
            iI.InfIntStopTreadmill();
        }

        //Demo Time Remaining Text
        float timer = (float)demoTimeRemaining;
        DTRTextN.text = Mathf.Floor(timer / 60).ToString() + ":" + Mathf.RoundToInt(timer % 60).ToString("00");
        DTRTextE.text = DTRTextN.text;
        DTRTextS.text = DTRTextN.text;
        DTRTextW.text = DTRTextN.text;
    }

    public void SetTheTimer(int i)
    {
        preferences.Write("demoMode", "true");
        DTRTextN.enabled = true;
        DTRTextE.enabled = true;
        DTRTextS.enabled = true;
        DTRTextW.enabled = true;
        demoTime = i;
        demoTimeRemaining = i;
    }

    public void ToggleDemoMode()
    {
        bool wasDemo = preferences.ReadBool("demoMode");
        if (wasDemo) { preferences.Write("demoMode", "false"); }
        else { preferences.Write("demoMode", "true"); }
        DTRTextN.enabled = !wasDemo;
        DTRTextE.enabled = !wasDemo;
        DTRTextS.enabled = !wasDemo;
        DTRTextW.enabled = !wasDemo;
    }
}