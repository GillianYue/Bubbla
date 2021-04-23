using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// something that moves in some way. Supports destination based movement (speed varies based on distance) and linear movement 
/// 
/// assumes has rigidbody to collide with stuff
/// </summary>
public abstract class MovingObject : MonoBehaviour
{
    public enum MovementMode { LINEAR, DEST_BASED };
    public abstract MovementMode movementMode { get; set; }

    public abstract Vector3 destPos { get; set; }

    /// <summary>
    /// used for DEST_BASED movement mode, 1 is no lag follow, 0.1f is a normal amount
    /// </summary>
    public abstract float followSpeedPercent { get; set; }
    /// <summary>
    /// used for LINEAR movement mode, a normal range is 5-50
    /// 
    /// each second, the object (if moving linearly) will move linearSpeed * (1 / Time.fixedDeltaTime) units
    /// </summary>
    public abstract float linearSpeed { get; set; } 

    protected bool timestepToggle; //used to half the frequency of calls in fixedUpdate()

    /// <summary>
    ///a layer mask, when specified as param in rayCast, will make it so that the raycast only hits stuff in this layer
    ///needs to be set before base.Start() called
    /// </summary>
    public abstract string layerMaskName { get; set; }
    protected int layerMaskIndex = -1;
    /// <summary>
    /// needs to be set before base.Start() called
    /// </summary>
    public abstract Collider2D objCollider { get; set; } 

    public abstract bool active { get; set; }

    /// <summary>
    /// first set stuff like layerMaskName and objCollider, then call base.Start()
    /// </summary>
    protected virtual void Start()
    {
        if(layerMaskName != "") 
        layerMaskIndex = LayerMask.GetMask(layerMaskName);
        
    }

    /// <summary>
    /// needs to be called in the override script
    /// </summary>
    protected virtual void FixedUpdate()
    {
        if (active && timestepToggle)
        {
            switch (movementMode)
            {
                case MovementMode.LINEAR:
                    //print("linear move object: " + destPos);
                    Global.nudgeTowards(gameObject, (int)destPos.x, (int)destPos.y, linearSpeed, layerMaskIndex);
                    break;
                case MovementMode.DEST_BASED:
                    //will change in speed depending on how far away the target destination is, advances followSpeedPercent of distance each time
                    float deltaMagnitude = (((Vector2)destPos - (Vector2)transform.position) * followSpeedPercent).magnitude;
                    Global.nudgeTowards(gameObject, (int)destPos.x, (int)destPos.y, deltaMagnitude, layerMaskIndex);
                    break;
            }

        }

        timestepToggle = !timestepToggle;
    }

    /// <summary>
    /// needs to be called in the override script
    /// 
    /// NOTE: dest is position, not local position
    /// sets the object movement to be linear and sets the speed accordingly to the time it's supposed to move over
    /// </summary>
    /// <param name="dest"></param>
    /// <param name="duration"></param>
    public virtual IEnumerator moveWorldDestLinear(Vector3 dest, float duration)
    {
        movementMode = MovementMode.LINEAR;
        float distPerSec = Mathf.Abs(Vector2.Distance(new Vector2(destPos.x, destPos.y), new Vector2(transform.position.x, transform.position.y))) / duration;

        linearSpeed = distPerSec * Time.fixedDeltaTime;
        destPos = new Vector3(dest.x, dest.y, destPos.z);

        float startTime = Time.time;

        yield return new WaitUntil(()=> {
            if (Vector2.Distance(new Vector2(destPos.x, destPos.y), new Vector2(transform.position.x, transform.position.y)) < 20) return true;
            if (Time.time - startTime > 10f)
                Debug.LogError("dest not reached within 10 seconds, skipped");
                return true; });
    }

    /// <summary>
    /// needs to be called in the override script
    /// 
    /// NOTE: dest is position, not local position
    /// sets the object movement to be non-linear
    /// </summary>
    /// <param name="dest"></param>
    /// <param name="duration"></param>
    public virtual IEnumerator moveWorldDestAccl(Vector3 dest)
    {
        movementMode = MovementMode.DEST_BASED;

        destPos = new Vector3(dest.x, dest.y, destPos.z);

        float startTime = Time.time;

        yield return new WaitUntil(() => {
            if (Vector2.Distance(destPos, transform.position) < 5) return true;
            if (Time.time - startTime > 10f)
                Debug.LogError("dest not reached within 10 seconds, skipped");
            return true;
        });
    }

}
