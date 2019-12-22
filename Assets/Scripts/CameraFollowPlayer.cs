using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{

    [Inject(InjectFrom.Anywhere)]
    public Player player;

    public GameObject moveAlong; //parent of group of GOs that should move along with the camera
    public GameObject Boundary;

    public int startCameraZ = 700;
    private int spaceBeyond = 50; //some space beyond edge of background will show for a more natural look

    private CapsuleCollider2D playerCollider;
    private int edge_layer_mask;

    private RaycastHit2D[] upHits, downHits, leftHits, rightHits;

    // Start is called before the first frame update
    void Start()
    {
        playerCollider = player.GetComponent<CapsuleCollider2D>();

        //a layer mask, when specified as param in rayCast, will make it so that the raycast only hits stuff in this layer
        edge_layer_mask = LayerMask.GetMask("Boundary");

        upHits = new RaycastHit2D[5]; downHits = new RaycastHit2D[5];
        leftHits = new RaycastHit2D[5]; rightHits = new RaycastHit2D[5];

        transform.position = new Vector3(transform.position.x, transform.position.y, startCameraZ);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 target = player.gameObject.transform.position;
        Vector2 delta = target - transform.position; //first quadrant: +,+; second: -,+; third: -,-; fourth: +,-
        //here we're ignoring the z-axis difference between camera and player

        ///////collision checking with raycast

        playerCollider.Raycast(Vector2.up, upHits, Global.MainCanvasHeight, edge_layer_mask);
        playerCollider.Raycast(Vector2.down, downHits, Global.MainCanvasHeight, edge_layer_mask);
        playerCollider.Raycast(Vector2.left, leftHits, Global.MainCanvasHeight, edge_layer_mask);
        playerCollider.Raycast(Vector2.right, rightHits, Global.MainCanvasHeight, edge_layer_mask);

        /**
         * NOTE: distance will only display properly when the maxDistance of Raycast is long enough
         * (if not, will return misleading results --> wherever the ray ends is the "RaycastHit")
         */


        //if (upHits[0].collider != null) {
        //    Debug.Log("wall up:" + upHits[0].distance);
        //}

        //if (leftHits[0].collider != null) { Debug.Log("wall left:" + leftHits[0].distance); }
        //if (rightHits[0].collider != null) { Debug.Log("wall right:" + rightHits[0].distance); }
        //if (downHits[0].collider != null) {
        //    Debug.Log("wall down:" + downHits[0].distance);
        //}

        if ((upHits[0].collider != null && delta.y > 0 && upHits[0].distance < Global.MainCanvasHeight / 2 - spaceBeyond)
            //up hit & moving up & reaching upper wall
            || (downHits[0].collider != null && delta.y < 0 && downHits[0].distance < Global.MainCanvasHeight / 2 - spaceBeyond))
        {
            ////change up/down movement to 0 (horizontal might still work)
            delta.y = 0;
        }

        if ((leftHits[0].collider != null && delta.x < 0 && leftHits[0].distance < Global.MainCanvasWidth / 2 - spaceBeyond)
        //left hit & moving left & reaching left wall
     || (rightHits[0].collider != null && delta.x > 0 && rightHits[0].distance < Global.MainCanvasWidth / 2 - spaceBeyond))
        {
            ////change left/right movement to 0 (vertical might still work)
            delta.x = 0;
        }

        //////end raycast check

        transform.position += (Vector3)delta; //delta might/not be edited up above, depending on the situation
        //camera needs to be at a certain distance from canvas

        moveAlong.transform.position = new Vector3(transform.position.x, transform.position.y, 0); 


    }
}
