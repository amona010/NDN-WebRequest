using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementScript : MonoBehaviour {

    public float maxMoveSpeed;

    public float speed;

    public bool ready;
    
    public Vector3 moveDirection;

    public Vector3 Destination;

    public float distance; 

    public string DestinationName;

    public ParticleSystem emit;

    public float destructionTime; 

    private void Start()
    {
        ready = false;
        emit = transform.Find("Particle System").GetComponent<ParticleSystem>();
        StartCoroutine(waitBeforeStarting());
        speed = distance / maxMoveSpeed;
    }

    void Update()
    {
        //moveDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        if (GameObject.Find(DestinationName))
        {
            Destination = GameObject.Find(DestinationName).transform.position;

            moveDirection = Vector3.MoveTowards(transform.position, Destination, speed * Time.deltaTime);
        }
    }

    void FixedUpdate()
    {
        //GetComponent<Rigidbody>().MovePosition(GetComponent<Rigidbody>().position + transform.TransformDirection(moveDirection) * moveSpeed * Time.deltaTime);
        if (GameObject.Find(DestinationName) && ready)
        {
            GetComponent<Rigidbody>().MovePosition(moveDirection);
        }
    }

    private void OnTriggerStay(Collider trigger)
    {
        if (trigger.gameObject.name == DestinationName)
        {
            destroyParticleObject();
        }
    }

    private void destroyParticleObject()
    {
        destructionTime -= Time.deltaTime; 

        if(destructionTime < 0)
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator waitBeforeStarting()
    {
        yield return new WaitForSeconds(.05f);

        ready = true;
    }

    public PlayerMovementScript(Vector3 Dest, string DestName)
    {
        Destination = Dest;
        DestinationName = DestName; 
    }
}
