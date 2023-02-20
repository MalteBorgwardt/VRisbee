using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public Target target;
    float time;
    float seconds;
    float minutes;

    [SerializeField]
    TMP_Text textTime;

    // Start is called before the first frame update
    void Start()
    {
        time = 0;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        seconds = (int)(time % 60);
        minutes = (int)(time / 60);

        textTime.text = minutes.ToString("00") + ":" + seconds.ToString("00");
    }
}
