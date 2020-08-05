using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public bool active = false;
    public float speed = 0.1f;
    public Vector3 startPos;
    public Transform endPortal;
    Transform fokusObject;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {

            if (fokusObject == null || fokusObject.transform.position.z < transform.position.z)
            {
                float refDistance = 100000000;
                float distance = 0;
                List<GameObject> objectsList = new List<GameObject>();
                objectsList.AddRange(GameObject.FindGameObjectsWithTag("AsteroidIn"));

                foreach (GameObject obj in objectsList)
                {
                    if (obj == null)
                        continue;
                    if (obj.transform.position.z < transform.position.z)
                        continue;

                    distance = Vector3.Distance(obj.transform.position, transform.position);
                    if (distance < refDistance)
                    {
                        refDistance = distance;
                        fokusObject = obj.transform;
                    }
                }
                distance = Vector3.Distance(endPortal.position, transform.position);
                if (distance < refDistance || distance < 2000)
                {
                    refDistance = distance;
                    fokusObject = endPortal;
                }
            }
            else
            {
                Vector3 position = transform.position;
                if (fokusObject.transform.position.z > transform.position.z)
                {

                    if (fokusObject.transform.position.x > transform.position.x && fokusObject.transform.position.x - transform.position.x >= 2)
                    {
                        position.x += speed;
                    }
                    else if (fokusObject.transform.position.x < transform.position.x && transform.position.x - fokusObject.transform.position.x >= 2)
                    {
                        position.x -= speed;
                    }

                    if (fokusObject.transform.position.y + (fokusObject == endPortal ? 0 : 200 *(Screen.height/2688)) > transform.position.y && fokusObject.transform.position.y + (fokusObject == endPortal ? 0 : 200 * (Screen.height/2688)) - transform.position.y >= 2)
                    {
                        position.y += speed;
                    }
                    else if (fokusObject.transform.position.y + (fokusObject == endPortal ? 0 : 200 * (Screen.height/2688)) < transform.position.y && transform.position.y - fokusObject.transform.position.y + (fokusObject == endPortal ? 0 : 200 * (Screen.height/2688)) >= 2)
                    {
                        position.y -= speed;
                    }
                }

                transform.position = position;
            }
        }
    }
}
