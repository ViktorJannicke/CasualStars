using UnityEngine;

public class Bullet : MonoBehaviour
{
    float timer = 0;
    public int bulletDamage = 25;
    public float maxTime;

    public Transform targetPlayer;
    public Obstacle o;
    public bool move;
    public float speed = 10;

    private void Update()
    {
        float deltaT = Time.deltaTime;
        if (move)
        {
            if (targetPlayer == null)
            {
                Destroy(gameObject);
            }
            else
            {
                var heading = targetPlayer.position - transform.position;
                var distance = heading.magnitude;
                Vector3 direction = heading / distance;
                transform.Translate(direction * deltaT * speed, Space.World);
            }
        }

        timer += deltaT;

        if (timer >= maxTime)
            Destroy(gameObject);
    }
}
