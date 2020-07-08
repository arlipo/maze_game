using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PopulationManager : MonoBehaviour
{
    public GameObject botPrefab;
    public int populationSize;
    public List<GameObject> population = new List<GameObject>();
    public float elapsed = 0;
    public float trialTime = 10;
    public int generation = 1;

    public bool ended = false;

    public float timeScale;

    // GUIStyle guiStyle = new GUIStyle();
    // void OnGUI() {
    //     guiStyle.fontSize = 25;
    //     guiStyle.normal.textColor = Color.white;
    //     GUI.BeginGroup(new Rect(10, 10, 250, 150));
    //     GUI.Box(new Rect(0,0,140,140), "Stats", guiStyle);
    //     GUI.Label(new Rect(10, 25, 200, 30), "Gen: " + generation, guiStyle);
    //     GUI.Label(new Rect(10, 50, 200, 30), string.Format("Time: {0:0.00}", elapsed), guiStyle);
    //     GUI.Label(new Rect(10, 75, 200, 30), "Population: " + population.Count, guiStyle);
    //     GUI.EndGroup();
    // }

    GameObject CreateGameObject() {
        Vector3 pos = new Vector3(this.transform.position.x, 
                                        this.transform.position.y, 
                                        this.transform.position.z);
        var go = Instantiate(botPrefab, pos, this.transform.rotation);
        go.GetComponent<Brain>().Init();
        return go;

    }

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = timeScale;
        for (int i = 0; i < populationSize; i++)
        {
            GameObject go = CreateGameObject();
            population.Add(go);
        }
    }

    GameObject Breed(GameObject parent1, GameObject parent2) {
        GameObject go = CreateGameObject();
        var brain = go.GetComponent<Brain>();
        if (Random.Range(0, 100) < 10) brain.dna.Mutate();
        else {
            var newBrain = Random.Range(0, 10) < 5 ? parent1.GetComponent<Brain>() : parent2.GetComponent<Brain>();
            brain.CLone(newBrain);
        }
        return go;        
    }

    List<GameObject> GetSortedPopulation() {
        return population.OrderBy(o => o.GetComponent<Brain>().GetFitness()).ToList();
    }

    void BreedNewPopulation() {
        var sortedList = GetSortedPopulation(); 
        population.Clear();

        for (int i = (int) (sortedList.Count / 2) - 1; i < sortedList.Count - 1; i++)
        {
            population.Add(Breed(sortedList[i], sortedList[i + 1]));
            population.Add(Breed(sortedList[i + 1], sortedList[i]));
        }


        foreach (var go in sortedList)
        {
            Destroy(go);
        }
        generation++;
    }

    // Update is called once per frame
    void Update()
    {
        elapsed += Time.deltaTime;
        if (elapsed >= trialTime) {
            Debug.Log(GetBest().GetComponent<Brain>().GetFitness());
            BreedNewPopulation();
            elapsed = 0;
        }
    }

    public GameObject GetBest() {
        return GetSortedPopulation().Last();
    }
}
