using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DNA
{
    public List<float> genes = new List<float>();
    public float maxValue;
    public int genesCount;
    
    public DNA(float v, int c) {
        maxValue = v;
        genesCount = c;
        SetRandom();
    }
    public void SetRandom() {
        genes.Clear();
        for (int i = 0; i < genesCount; i++)
        {
            genes.Add(Random.Range(-maxValue, maxValue));
        }
    }

    public void Combine(DNA d1, DNA d2) {
        if (genesCount != d1.genesCount || genesCount != d2.genesCount) throw new System.ArgumentException();
        for (int i = 0; i < genesCount; i++)
        {
            genes[i] = Random.Range(0, 10) < 5 ? d1.genes[i] : d2.genes[i];
        }
    }

    public void Mutate() {
        for (int i = 0; i < genesCount; i++)
        {
            if (Random.Range(0, 100) == 1) genes[i] = Random.Range(0f, maxValue + 1);
        }
    }
}
