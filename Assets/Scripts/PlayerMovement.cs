using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] public LayerMask movableLayer;

    [Header("Refs")]
    public WaterBar waterBar;
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider2D;
    private LineRenderer lr;
    Camera cam;
    private GameObject movable;
    private Rigidbody2D movableRb;

    [Header("Bools")]
    private bool onGround;
    private bool isFilled;
    public bool isInfinte;

    [Header("Physics Floats")]
    public float jumpPower = 1f;
    public float blastPower = 1f;
    public float radius = 30f;

    [Header("Floats")]
    public float waterLevel = 0f;
    public float spendTime = 1f;
    public float spendDelay = 1f;
    public float mass = 1;
    public float crouchMass = 5;
    public float spendModif = 1;

    private RaycastHit hit;

    [Header("Vectors")]
    Vector2 force;
    Vector2 blast;
    Vector3 mousePosition;
    Vector3 playerPosition;
    Vector2 heading;




    // Start is called before the first frame update
    void Start()
    {
        //Refs
        cam = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        lr = GetComponent<LineRenderer>();
        movable = GameObject.FindGameObjectWithTag("Movable");
        movableRb = movable.GetComponent<Rigidbody2D>();
    }

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }



    // Update is called once per frame
    void Update()
    {

        //Desliga a rotação do rb
        rb.freezeRotation = true;

        //Ref da posição do mouse e do rb
        mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
        playerPosition = rb.position;

        //Variaveis de direção e distancia entre o mouse e o rb
        var heading = mousePosition - playerPosition;
        var distance = heading.magnitude;

        var direction = heading / distance;

        Debug.DrawRay(playerPosition, heading, Color.red);

        MaxMousePosition();
        MaxWaterLevel();

        //Se clicar no M2 e estiver no chão = pule na direção do mouse
        if (Input.GetMouseButton(1))
        {
            rb.gravityScale = crouchMass;
        }

        if (Input.GetMouseButtonUp(1) && onGround == true)
        {
            rb.gravityScale = mass;
            force = new Vector2(direction.x * jumpPower, direction.y * jumpPower);

            rb.AddForce(force * jumpPower, ForceMode2D.Impulse);

        }

        //Se clicar no M1 = impulsione na direção oposta do mouse
        if (Input.GetMouseButton(0) & isFilled == true)
        {

            blast = new Vector2(direction.x * blastPower, direction.y * blastPower);
            rb.AddForce(-blast * blastPower);
            RenderLine(playerPosition, mousePosition);
            PushObject();

        }

        if (Input.GetMouseButtonDown(0))
        {
            InvokeRepeating("SpendWater", spendTime, spendDelay);
        }

        if ((Input.GetMouseButtonUp(0)))
        {
            EndLine();
            CancelInvoke();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Ground") ^ collision.CompareTag("Movable"))
        {
            Debug.Log("On Ground");
            onGround = true;
        }

    }

    void OnTriggerExit2D(Collider2D collision)
    {

        if (collision.CompareTag("Ground") ^ collision.CompareTag("Movable"))
        {
            Debug.Log("Not On Ground");
            onGround = false;
        }

    }

    void OnTriggerStay2D(Collider2D collision)
    {

        if (collision.CompareTag("Ground") ^ collision.CompareTag("Movable"))
        {
            Debug.Log("Still On Ground");
            onGround = true;
        }

    }

    void RenderLine(Vector3 startPoint, Vector3 endPoint)
    {
        lr.positionCount = 2;
        Vector3[] points = new Vector3[2];
        points[0] = playerPosition;
        points[1] = mousePosition;

        lr.SetPositions(points);
    }

    void EndLine()
    {
        lr.positionCount = 0;
    }

    void MaxMousePosition()
    {

        float distance = Vector3.Distance(playerPosition, mousePosition);

        if (distance > radius)
        {
            Vector3 fromOriginToObject = mousePosition - playerPosition;
            fromOriginToObject *= radius / distance;
            mousePosition = playerPosition + fromOriginToObject;
        } 

    }

    void MaxWaterLevel()
    {
        if (waterLevel > 100)
        {
            waterLevel = 100;
        }
    }

    void SpendWater()
    {
        var heading = mousePosition - playerPosition;
        var distance = heading.magnitude;

        float spendValue = distance / spendModif;

        if (waterLevel > 0)
        {
            waterLevel -= spendValue;
            isFilled = true;
        }
        else
        {
            waterLevel = 0;
            isFilled = false;
            EndLine();
        }

        Debug.Log(waterLevel);

    }

    void PushObject()
    {
        var heading = mousePosition - playerPosition;
        var distance = heading.magnitude;


        RaycastHit2D hit = Physics2D.Raycast(playerPosition, heading, distance, movableLayer);

        if (hit.rigidbody != null)
        {
            if (hit.rigidbody.CompareTag("Movable"))
            {
                movableRb = hit.collider.gameObject.GetComponent<Rigidbody2D>();

                movableRb.AddForce(blast * blastPower);
            }
        }


    }
}

