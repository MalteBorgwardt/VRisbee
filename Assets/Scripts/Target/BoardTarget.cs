using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class BoardTarget : MonoBehaviour
{
    private int hitCount = 0;
    private int score = 0;
    public TMP_Text textScore;
    public GameObject discPrefabRightHand;
    public GameObject discPrefabLeftHand;
    public TMP_Text debugText;
    public TMP_Text textMisses;

    private void Awake()
    {
        spawnDisc();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Disc"))
        {
            hitCount++;
            GameObject[] discs = GameObject.FindGameObjectsWithTag("Disc");
            GameObject latestDisc = discs[^1];
            float dist = Vector3.Distance(latestDisc.transform.position, transform.position);
            if (dist <= 0.3){
                AudioSource _audioSource = GetComponent<AudioSource>();
                _audioSource.Play();
                score += 100;
            }else if(dist <= 0.6){
                score += 80;
            }else if(dist <= 0.9){
                score += 60;
            }else if(dist <= 1.2){
                score += 40;
            }else{
                score += 20;
            }
            GameObject throwsValue = GameObject.Find("Throws Value");
            TMP_Text throwsValueText = throwsValue.GetComponent<TMP_Text>();
            int currentValue = int.Parse(throwsValueText.text);
            currentValue++;
            throwsValueText.text = currentValue.ToString();
            textScore.text = score.ToString();
            latestDisc.GetComponent<Rigidbody>().isKinematic = true;
            latestDisc.GetComponent<XRGrabInteractable>().enabled = false;
            latestDisc.GetComponent<BoxCollider>().enabled = false;
            if(discs.Length > 5){
                Destroy(discs[0]);
            }
            spawnDisc();
        }
    }

    private void spawnDisc(){
        var discPrefab = GameSettings.isLeftHanded ? discPrefabLeftHand : discPrefabRightHand;
        GameObject disc = Instantiate(discPrefab) as GameObject;
        disc.transform.position = new Vector3(0.019f, 0.153483f, 0.7621412f);
    }

    void Update()
    {
        GameObject[] discs = GameObject.FindGameObjectsWithTag("Disc");
        GameObject latestDisc = discs[^1];
        if (transform.position.z - latestDisc.transform.position.z < -0.3f) {
            float dist = Vector3.Distance(latestDisc.transform.position, transform.position);
            debugText.text = "Missed by " + dist.ToString();
            GameObject throwsValue = GameObject.Find("Throws Value");
            TMP_Text throwsValueText = throwsValue.GetComponent<TMP_Text>();
            int currentValue = int.Parse(throwsValueText.text);
            currentValue++;
            throwsValueText.text = currentValue.ToString();
            int currentValueMisses = int.Parse(textMisses.text);
            currentValueMisses++;
            textMisses.text = currentValueMisses.ToString();
            Destroy(discs[^1]);
            spawnDisc();
        }
    }
}
