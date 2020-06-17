using UnityEngine;
using UnityEngine.AI;

public class ShipAI : MonoBehaviour
{
    public Movement movement;

    public GameObject target;

    public Vector3 targetPos;
    
    private void Start()
    {
        movement.moveTo(targetPos);
    }
}

