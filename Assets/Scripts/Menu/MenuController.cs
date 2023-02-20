using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Feature {
    A, // WITHOUT visualization
    B, // WITH visualization
}

public class MenuController : MonoBehaviour
{
    /// <summary>
    /// Starts Frisbee Game with randomly assigned feature A or B
    /// </summary>
    public void StartGame(){
        const int minInclusive = 0;
        const int maxExclusive = 2;
        bool isFeatureA = 0 == Random.Range(minInclusive, maxExclusive);
        GameSettings.studyFeature = isFeatureA ? Feature.A : Feature.B;
        SceneManager.LoadScene(1);
    }
}
