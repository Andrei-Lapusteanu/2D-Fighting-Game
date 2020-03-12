using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    const float PROJ_VELOCITY_MIN = 6.0f;
    const float PROJ_VELOCITY_MAX = 12.0f;
    const float PROJ_BASE_DAMAGE = 5.0f;

    public GameObject projectileExplosion;
    private Rigidbody2D rb;
    public static float damage;
    public float velocityX = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        velocityX = Random.Range(PROJ_VELOCITY_MIN, PROJ_VELOCITY_MAX);
        SetMovementDirection();

        damage = PROJ_BASE_DAMAGE;
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = new Vector2(velocityX, 0);
    }

    private void SetMovementDirection()
    {
        if(GameObject.Find("Ken").transform.position.x > transform.position.x)
        {

        }
        else
        { 
            velocityX = -velocityX;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            projectileExplosion.transform.localScale = new Vector3(8, 8, 0);
            GameObject objClone = Instantiate(projectileExplosion, transform.position, Quaternion.identity);
            Destroy(objClone, 0.45f);
            Destroy(gameObject);
        }
    }
}
