using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MovingObject
{

    [Inject(InjectFrom.Anywhere)]
    public Player player;

    public bool followPlayer = true;
    private Vector3 startOffset = new Vector3(0, -400, 0); //offset from center of screen (adjusts player's visible pos in game)

    public GameObject Boundary;

    public int startCameraZ;

    public Vector3 targetPos; //cam will always move towards this pos

    //override parent's abstract so we know those properties exist and need to be dealt with in child script
    public override Collider2D objCollider { get; set; }
    public override string layerMaskName { get; set; }

    public override Vector3 destPos { get; set; }

    public override MovementMode movementMode { get; set; }

    public override bool active { get; set; }

    public override float followSpeedPercent { get; set; }

    public override float linearSpeed { get; set; }

    private RaycastHit2D[] upHits, downHits, leftHits, rightHits; //cam only
    public float spaceBeyond; //space cam is allowed to move slightly beyond walls (to reveal blank space in beyond)

    protected override void Start()
    {
        //playerCollider = player.GetComponent<CapsuleCollider2D>();

        //a layer mask, when specified as param in rayCast, will make it so that the raycast only hits stuff in this layer
        //edge_layer_mask = LayerMask.GetMask("Boundary");

        transform.position = new Vector3(transform.position.x, transform.position.y, startCameraZ);

        objCollider = GetComponent<CapsuleCollider2D>();
        layerMaskName = "Boundary";
        movementMode = MovementMode.DEST_BASED;
        active = true;
        followSpeedPercent = 0.11f;

        upHits = new RaycastHit2D[5]; downHits = new RaycastHit2D[5];
        leftHits = new RaycastHit2D[5]; rightHits = new RaycastHit2D[5];

        base.Start();
    }


    /// <summary>
    /// camera is special in its collision checking logic, so does not call base.FixedUpdate() in this case
    /// </summary>
    protected override void FixedUpdate()
    {
        if (active && timestepToggle)
        {
            //updates the camera destPos to be player with offset
            if (followPlayer) destPos = player.gameObject.transform.position - startOffset; //setting destPos relative to cam-player offset

            Vector2 delta = (destPos - transform.position) * followSpeedPercent; //first quadrant: +,+; second: -,+; third: -,-; fourth: +,-
                                                                                 //here we're ignoring the z-axis difference between camera and player

            ///////collision checking with raycast
            objCollider.Raycast(Vector2.up, upHits, Screen.height, layerMaskIndex);
            objCollider.Raycast(Vector2.down, downHits, Screen.height, layerMaskIndex);
            objCollider.Raycast(Vector2.left, leftHits, Screen.width, layerMaskIndex);
            objCollider.Raycast(Vector2.right, rightHits, Screen.width, layerMaskIndex);


            /**
             * NOTE: distance will only display properly when the maxDistance of Raycast is long enough
             * (if not, will return misleading results --> wherever the ray ends is the "RaycastHit")
             */

            if ((upHits[0].collider != null && delta.y > 0 && upHits[0].distance < Screen.height / 2 - spaceBeyond)) 
            { delta.y = 0; }
            //up hit & moving up & reaching upper wall
            if (downHits[0].collider != null && delta.y < 0 && downHits[0].distance < Screen.height / 2 - spaceBeyond) 
            { delta.y = 0; }

            if (leftHits[0].collider != null && delta.x < 0 && leftHits[0].distance < Screen.width / 2 - spaceBeyond)
            { delta.x = 0; }
                 //left hit & moving left & reaching left wall
             if(rightHits[0].collider != null && delta.x > 0 && rightHits[0].distance < Screen.width / 2 - spaceBeyond)
            {
                ////change left/right movement to 0 (vertical might still work)
                delta.x = 0;
            }

            //////end raycast check
           

            transform.position += (Vector3)delta; //delta might/not be edited up above, depending on the situation
                                                  //camera needs to be at a certain distance from canvas

        }

        timestepToggle = !timestepToggle;
    }

}
