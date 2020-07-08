using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brain : MonoBehaviour
{
    public GameObject eyes;
    public DNA dna;
    public bool spawnEntered, checkointHighEntered, checkointLowEntered;
    float distance;
    int DNALength = 500;
    int curDna;
    int curFrame;
    bool alive = true;
    bool seeWall = false;
    int spinningTimes;
    Vector3 start;
    Vector3 center;

    int speed = 15;

    void Die() {
        alive = false;
        EraseValues();
    }
    void OnTriggerEnter(Collider other)
    {
        System.Action eraseValues = () => {
            start = this.transform.position;
            distance = 0;
        };

        switch(other.gameObject.tag) {
            case "spawn": spawnEntered = true; eraseValues(); break;
            case "checkpointh": checkointHighEntered = true; eraseValues(); break; 
            case "checkpointl": checkointLowEntered = true; eraseValues(); break; 
        }
    }
    void OnCollisionEnter(Collision obj)
    {
        if(obj.gameObject.tag == "dead") {
            Die();
        } else if (obj.gameObject.tag == "pbot") {
            Debug.Log("Collision detected!");
        }
    }
    public float GetFitness() {
        var fitness = -distance + (spawnEntered ? 0 : 0) + (checkointHighEntered ? 100 : 0) + (checkointLowEntered ? 100 : 0);
        return fitness;
    }
    public void Init() {
        dna = new DNA(1, DNALength);
        InitializeValues();
    }
    // Start is called before the first frame update
    void Start()
    {
        center = new Vector3(378.6f, this.transform.position.y, 102.6f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!alive) return;
        // RaycastHit hit;
        // seeWall = false;
        // Debug.DrawRay(eyes.transform.position, eyes.transform.forward, Color.red, eyes.transform.forward.magnitude / 2);
        // if (Physics.SphereCast(eyes.transform.position, 0.1f, eyes.transform.forward * 0.5f, out hit, 0.5f)) {
        //     if (hit.collider.gameObject.tag == "wall") {
        //         seeWall = true;
        //     }
        // }
    }

    void FixedUpdate()
    {
        if (!alive || curDna >= DNALength) return;
        curFrame++;
        RaycastHit hit;
        seeWall = false;
        
        Debug.DrawRay(eyes.transform.position, eyes.transform.forward, Color.red, eyes.transform.forward.magnitude / 2);
        if (Physics.SphereCast(eyes.transform.position, 0.1f, eyes.transform.forward * 0.5f, out hit, 0.5f)) {
            if (hit.collider.gameObject.tag == "wall") {
                seeWall = true;
            }
        }

        var z = 0;
        var rot = 0f;
        if (curFrame % 10 == 0) {
            rot = 90 * dna.genes[curDna];
            curDna++;
        }
        
        if (!seeWall) {
            z = speed;
            spinningTimes = 0;
        } else {
            spinningTimes++;
        }

        // if (spinningTimes > 50) Die();

        this.transform.Translate(0, 0, z * Time.deltaTime);
        this.transform.Rotate(0, rot, 0);
        
        
        distance = (this.transform.position - center).magnitude;
    }

    // var dot = Vector3.Dot(this.transform.forward, Vector3.forward);
            // if (dot >= 0.5) index = 0; // up
            // else if (dot <= -0.5) index = 1; // down
            // else {
            //     dot = Vector3.Dot(this.transform.forward, Vector3.right);
            //     if (dot >= 0.5) index = 2; // right
            //     else if (dot <= -0.5) index = 3; // left
            // }

    public void CLone(Brain brain) {
        DNALength = brain.DNALength;
        dna = brain.dna;
        InitializeValues();
    }

    void InitializeValues() {
        start = this.transform.position;
        curDna = 0;
        EraseValues();
    }

    void EraseValues() {
        spinningTimes = 0;
        spawnEntered = false;
        checkointHighEntered = false;
        checkointLowEntered = false;
    }
}
