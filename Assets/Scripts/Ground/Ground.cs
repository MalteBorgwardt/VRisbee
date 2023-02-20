using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Ground : MonoBehaviour
{
    public GameObject discPrefab;
    public TMP_Text textMisses;
    public TMP_Text debugText;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Disc"))
        {
            GameObject throwsValue = GameObject.Find("Throws Value");
            TMP_Text throwsValueText = throwsValue.GetComponent<TMP_Text>();
            int currentValue = int.Parse(throwsValueText.text);
            currentValue++;
            throwsValueText.text = currentValue.ToString();
            int currentValueMisses = int.Parse(textMisses.text);
            currentValueMisses++;
            textMisses.text = currentValueMisses.ToString();
            GameObject[] discs = GameObject.FindGameObjectsWithTag("Disc");
            debugText.text = "Hit Ground!";
            Destroy(discs[^1]);
            spawnDisc();
        }
    }

    private void spawnDisc(){
        GameObject disc = Instantiate(discPrefab) as GameObject;
        disc.transform.position = new Vector3(0.019f, 0.153483f, 0.7621412f);
    }
}
