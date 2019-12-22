using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{

    [Inject(InjectFrom.Anywhere)]
    public Player player;

    public GameObject moveAlong; //parent of group of GOs that should move along with the camera
    public GameObject Boundary;

    private CapsuleCollider2D playerCollider;
    private int edge_layer_mask;

    private RaycastHit2D[] upHits, downHits, leftHits, rightHits;

    // Start is called before the first frame update
    void Start()
    {
        playerCollider = player.GetComponent<CapsuleCollider2D>();

        edge_layer_mask = LayerMask.GetMask("Boundary");

        upHits = new RaycastHit2D[5]; downHits = new RaycastHit2D[5];
        leftHits = new RaycastHit2D[5]; rightHits = new RaycastHit2D[5];
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 target = player.gameObject.transform.position;
        Vector2 delta = target - transform.position;

        RaycastHit2D up, left, down, right;
        ///////collision checking with raycast

        playerCollider.Raycast(Vector2.up, upHits, Global.MainCanvasHeight, edge_layer_mask);
        playerCollider.Raycast(Vector2.down, downHits, Global.MainCanvasHeight, edge_layer_mask);
        playerCollider.Raycast(Vector2.left, leftHits, Global.MainCanvasHeight, edge_layer_mask);
        playerCollider.Raycast(Vector2.right, rightHits, Global.MainCanvasHeight, edge_layer_mask);

        //up = playerCollider.Raycast(transform.position, Vector2.up, Mathf.Infinity, edge_layer_mask); //up/down flipped in our game
        //left = Physics2D.Raycast(transform.position, -Vector2.right, Mathf.Infinity, edge_layer_mask);
        //down = Physics2D.Raycast(transform.position, -Vector2.up, Mathf.Infinity, edge_layer_mask);
        //right = Physics2D.Raycast(transform.position, Vector2.right, Mathf.Infinity, edge_layer_mask);

        //Debug.Log("wall up:" + upHits[0].distance + " left: " + leftHits[0].distance + " right: " + rightHits[0].distance + " down: " + downHits[0].distance);

        if (upHits[0].collider != null) {
            Debug.Log("wall up:" + upHits[0].distance);
            Debug.DrawLine(player.transform.position,
            player.transform.position + new Vector3(0, upHits[0].distance, 10), Color.blue);
        }

        if (leftHits[0].collider != null) { Debug.Log("wall left:" + leftHits[0].distance); }
        if (rightHits[0].collider != null) { Debug.Log("wall right:" + rightHits[0].distance); }
        if (downHits[0].collider != null) { Debug.Log("wall down:" + downHits[0].distance);

            Debug.DrawLine(player.transform.position,
               player.transform.position + new Vector3(0, -downHits[0].distance, 10), Color.red);

        }

        if (upHits[0].collider != null && delta.y > 0 && upHits[0].distance < Global.MainCanvasHeight / 2) //up hit & moving up & reaching wall
        {
            //change up movement to 0 (horizontal might still work)
            transform.position = target + new Vector3(0, 0, -700);
            target.z = 0;
            moveAlong.transform.position = target;

            
        }
        else
        {
            //////end raycast check


            transform.position = target + new Vector3(0, 0, -700);
            target.z = 0;
            moveAlong.transform.position = target;


        }

    }
}
