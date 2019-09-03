using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSpawner : MonoBehaviour
{
    public Vector3 source;
    public Vector3 Destination;
    public string DestinationName;
    public float spawnWait;
    public float currentTime;
    public bool ready = true; 
    public GameObject particle;

    // Use this for initialization
    void Start()
    {
        currentTime = spawnWait;
    }

    // Update is called once per frame
    void Update()
    {
        //currentTime -= 1 * Time.deltaTime;
        if (currentTime <= 0 && ready)
        {
            ParticleSpawn();
            currentTime = spawnWait;
        }

        if(GameObject.FindGameObjectsWithTag("Particle").Length == 0)
        {
            ready = true;
            currentTime -= 1 * Time.deltaTime;
        }
    }

    public void ParticleSpawn()
    {
        Debug.Log("I AM SPAWNING");
        Vector3 nothing = new Vector3(0, 0, 0);

        if(source != nothing && Destination != nothing)
        {
            GameObject newParticle = GameObject.Instantiate(particle);
            newParticle.transform.position = source;
            newParticle.GetComponent<PlayerGravityBody>().attractorPlanet = GameObject.Find("Earth").GetComponent<PlanetScript>();
            newParticle.GetComponent<PlayerMovementScript>().Destination = Destination;
            newParticle.GetComponent<PlayerMovementScript>().DestinationName = DestinationName;
            newParticle.GetComponent<PlayerMovementScript>().distance = Vector3.Distance(source, Destination);
            ready = false;
        }
    }

    //Add Spawner Script logic and then go about adding way to implement the script in NDNData and DataVisualizer
}