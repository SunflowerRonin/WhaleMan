using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement: MonoBehaviour
{
    [SerializeField] private LayerMask platformLayerMask;

    private Rigidbody2D rb;
    private CapsuleCollider2D capCollider2D;
    private LineRenderer lr;
    private CircleCollider2D circle;

    public float jumpPower = 1f;
    public float blastPower = 1f;
    public float extraHeightText = 1f;
    public float extraLenghtText = 0f;
    float groundLine1 = 0;
    float groundLine2 = 0;
    float groundLine3 = 0;


    Camera cam;
    Vector2 force;
    Vector2 blast;
    Vector3 mousePosition;
    Vector3 playerPosition;

    


    // Start is called before the first frame update
    void Start()
    {
        //Refs
        cam = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        capCollider2D = GetComponent<CapsuleCollider2D>();
        lr = GetComponent<LineRenderer>();
        circle = GetComponent<CircleCollider2D>();


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

        //Se clicar no M2 e estiver no chão = pule na direção do mouse
        if (Input.GetMouseButtonDown(1) && OnGround())
        {
            force = new Vector2(direction.x * jumpPower, direction.y * jumpPower);

            rb.AddForce(force * jumpPower, ForceMode2D.Impulse);
        }

        //Se clicar no M1 = impulsione na direção oposta do mouse
        if (Input.GetMouseButton(0))
        {
            blast = new Vector2 (direction.x * blastPower, direction.y * blastPower);

            rb.AddForce(-blast * blastPower);

            RenderLine(playerPosition, mousePosition);
        }

        if ((Input.GetMouseButtonUp(0)))
        {
            EndLine();
        }

        MaxMousePosition();
    }

    private bool OnGround() 
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(capCollider2D.bounds.center, capCollider2D.bounds.size, 0f, Vector2.down, extraHeightText, platformLayerMask);
        Color rayColor;
        if (raycastHit.collider !=null)
        {
            rayColor = Color.green;
        }
        else
        {
            rayColor = Color.red;
        }

        Debug.DrawRay(capCollider2D.bounds.center + new Vector3(capCollider2D.bounds.extents.x + groundLine1, 0), Vector2.down * (capCollider2D.bounds.extents.y + extraHeightText), rayColor, 2, false);
        Debug.DrawRay(capCollider2D.bounds.center - new Vector3(capCollider2D.bounds.extents.x + groundLine2, 0), Vector2.down * (capCollider2D.bounds.extents.y + extraHeightText), rayColor, 2, false);
        
        Debug.Log(raycastHit.collider);
        return raycastHit.collider != null;
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
        circle.bounds.Encapsulate(mousePosition);
    }
}

