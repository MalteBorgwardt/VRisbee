using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class RayDistance : MonoBehaviour
{

    public TMP_Text textScore;

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        XRRayInteractor ray = GetComponent<XRRayInteractor>();
        ray.TryGetCurrent3DRaycastHit(out hit);
        GameObject target = GameObject.Find("BoardTarget");
        float dist = Vector3.Distance(target.transform.position, hit.point);
        textScore.text = dist.ToString();
    }
}
