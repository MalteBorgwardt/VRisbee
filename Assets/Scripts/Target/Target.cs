using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Target : MonoBehaviour
{

    public GameObject defaultA;
    public GameObject hit;

    public int hitCount;

    private float targetTime = 0.0f;
    private bool targetHitable = true;

    private Target[] allTargets;

    public TMP_Text textScore;

    void Update(){
        if(targetTime >= 0.0f){
            targetTime -= Time.deltaTime;
        }
        
        if (targetTime <= 0.0f){
            targetHitable = true;
        }
    }

    public int HitCount
    {
        get => hitCount;
        set
        {
            if (value % 2 == 0) {
                defaultA.SetActive(true);
                hit.SetActive(false);
            }
            else
            {
                defaultA.SetActive(false);
                hit.SetActive(true);
            }
            hitCount = value;
        }
    }

    void Start()
    {
        allTargets = FindObjectsOfType<Target>();
        hitCount = 0;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Disc") && targetHitable)
        {
            targetTime = 2.0f;
            targetHitable = false;
            HitCount++;
            int finalScore = 0;
            foreach(Target t in allTargets){
                finalScore += t.hitCount;
            }
            textScore.text = finalScore.ToString();
        }
    }
}
