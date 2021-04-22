using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MovingObject
{

    [Inject(InjectFrom.Anywhere)]
    public Player player;

    public bool followPlayer = false;
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
        active = false; //defaults to inactive, will need to be manually activated if in Travel GameMode
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
            if (followPlayer) destPos = player.transform.position - startOffset; //setting destPos relative to cam-player offset

            Vector2 delta = new Vector2();
            if (movementMode == MovementMode.DEST_BASED)
            {
                delta = (destPos - transform.position) * followSpeedPercent; //first quadrant: +,+; second: -,+; third: -,-; fourth: +,-
                                                                                     //here we're ignoring the z-axis difference between camera and player
            }else if (movementMode == MovementMode.LINEAR)
            {
                delta = (destPos - transform.position).normalized * linearSpeed;
            }

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
            { 
                delta.y = 0;
                destPos = new Vector3(destPos.x, transform.position.y, destPos.z);
            }
            //up hit & moving up & reaching upper wall
            if (downHits[0].collider != null && delta.y < 0 && downHits[0].distance < Screen.height / 2 - spaceBeyond) 
            { 
                delta.y = 0;
                destPos = new Vector3(destPos.x, transform.position.y, destPos.z);
            }

            if (leftHits[0].collider != null && delta.x < 0 && leftHits[0].distance < Screen.width / 2 - spaceBeyond)
            { 
                delta.x = 0;
                destPos = new Vector3(transform.position.x, destPos.y, destPos.z);
            }
                 //left hit & moving left & reaching left wall
             if(rightHits[0].collider != null && delta.x > 0 && rightHits[0].distance < Screen.width / 2 - spaceBeyond)
            {
                ////change left/right movement to 0 (vertical might still work)
                delta.x = 0;
                destPos = new Vector3(transform.position.x, destPos.y, destPos.z);
            }

            //////end raycast check
            transform.position += (Vector3)delta; //delta might/not be edited up above, depending on the situation
                                                  //camera needs to be at a certain distance from canvas
        }

        timestepToggle = !timestepToggle;
    }

    /// <summary>
    /// refocuses on player
    /// </summary>
    public void recenterOnPlayer()
    {
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, startCameraZ);
    }

    /// <summary>
    /// needs to be called in the override script
    /// differs from base because half of the fixedUpdates are ignored in CameraFollow
    /// 
    /// NOTE: dest is position, not local position
    /// sets the object movement to be linear and sets the speed accordingly to the time it's supposed to move over
    /// </summary>
    /// <param name="dest"></param>
    /// <param name="duration"></param>
    public IEnumerator moveWorldDestLinear(Vector3 dest, float duration, float lingerAfterReach, bool followPlayerAfterReach)
    {
        followPlayer = false;

        movementMode = MovementMode.LINEAR;
        float distPerSec = Mathf.Abs(Vector2.Distance((Vector2)dest, (Vector2)transform.position)) / duration;

        linearSpeed = distPerSec * Time.fixedDeltaTime * 2;
        destPos = new Vector3(dest.x, dest.y, destPos.z);

        print("destPos " + destPos + " " + transform.position + " " + Vector2.Distance(new Vector2(destPos.x, destPos.y), new Vector2(transform.position.x, transform.position.y)));

        float startTime = Time.time;

        yield return new WaitUntil(() => {
            if (Vector2.Distance(new Vector2(destPos.x, destPos.y), new Vector2(transform.position.x, transform.position.y)) < 5) 
            {
                print("destPos " + destPos + " " + transform.position + " reached " + Vector2.Distance(new Vector2(destPos.x, destPos.y), new Vector2(transform.position.x, transform.position.y)));
                return true; 
            }

            if (Time.time - startTime > 10f)
            {
                Debug.LogError("dest not reached within 10 seconds, skipped");
                return true;
            }

            return false;
        });

        yield return new WaitForSeconds(lingerAfterReach);

        followPlayer = followPlayerAfterReach;
    }

    /// <summary>
    /// needs to be called in the override script
    /// differs from base because half of the fixedUpdates are ignored in CameraFollow
    /// defaults to restore whatever followPlayer was previously set to before the event is called
    /// 
    /// NOTE: dest is position, not local position
    /// sets the object movement to be linear and sets the speed accordingly to the time it's supposed to move over
    /// </summary>
    /// <param name="dest"></param>
    /// <param name="duration"></param>
    public IEnumerator moveWorldDestLinear(Vector3 dest, float duration, float lingerAfterReach)
    {
        bool prevFollowPlayer = followPlayer;
        yield return moveWorldDestLinear(dest, duration, lingerAfterReach, prevFollowPlayer);
    }

    /// <summary>
    /// needs to be called in the override script
    /// 
    /// NOTE: dest is position, not local position
    /// sets the object movement to be non-linear
    /// </summary>
    /// <param name="dest"></param>
    /// <param name="duration"></param>
    public override IEnumerator moveWorldDestAccl(Vector3 dest)
    {
        return base.moveWorldDestAccl(dest);
    }
}
