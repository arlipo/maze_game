using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowDataScript : MonoBehaviour
{
    public PopulationManager populationManager;
    // Start is called before the first frame update
    /// <summary>
    /// OnGUI is called for rendering and handling GUI events.
    /// This function can be called multiple times per frame (one call per event).
    /// </summary>
    void OnGUI()
    {
        if (!populationManager.ended) GUI.Label(new Rect(10, 25, 200, 30), "Gen: " + populationManager.generation);
        GUI.Label(new Rect(10, 50, 200, 30), "Fitness: " + populationManager.GetBest().GetComponent<Brain>().GetFitness());
        GUI.Label(new Rect(10, 75, 200, 30), "Population Size: " + populationManager.population.Count);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
