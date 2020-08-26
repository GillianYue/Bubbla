using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//GO movement
public class EnemySteering : MonoBehaviour
{

    [Inject(InjectFrom.Anywhere)]
    public PathManager pathManager;

    //Mode.pathFollow uses:
    public SteerPath path; //the instance of that path, specific to this enemy/GO
    public int currentNode;
    //Mode.seek & Mode.escape & Mode.pursuit uses:
    public Vector2 currTarget; //in world space
    //pursuit
    public Vector2 prevTarget, targetVelocity;
    public bool dynamicT; //T is for how many updates ahead the pursuit should predict; dynamic == dependent on dist
    public int constantT = 3; //this is for if dynamicT == false

    public int precisionRadius = 10; //how close to the target the obj has to be to qualify "getting there"
    public float max_velocity = 1, velocity, mass = 1;

    public bool smooth; //if true, will not instantly change velocity, but will steer towards ideal velocity
    public Vector2 currForce, target;

    //for curveFollow

    float currentPathPercent;               //current percentage of completing the path

    public enum Mode { pathFollow, curveFollow, seek, escape, pursuit, wait }
    //TODO arrival which slows down when getting close to target, obj never passing the destination
    public Mode movementType;

    //returns force needed to go after the current target node 
    private Vector2 pathFollowing() {

        target = new Vector2();
 
        if (path != null) {
            ArrayList nodes = path.getNodes();
 
            target = (Vector2)nodes[currentNode];
 
            if (Global.findVectorDist(this.transform.position, target) <= precisionRadius) { //reach dest

                currentNode += 1;
 
                if (currentNode >= nodes.Count) {
                    currentNode = 0; //restart
                }
            }
        }
 
        return seek(target);
    }

    /// linear, movement is straight line and will result in sudden turns and changes of direction
    ///
    /// this function returns normalized force needed to "seek" the target * max_velocity
    private Vector2 seek(Vector2 t)
    {
        return Vector3.Normalize(t - (Vector2)transform.position) * max_velocity;
    }

    //flee is just seek but in the opposite direction; steering works in the same way
    private Vector2 flee(Vector2 t)
    {
        return -seek(t);
    }

    /// pursuit is basically following a predicted future pos of the target (param is how many updates up ahead should the
    /// prediction be)
    private Vector2 pursuit(Vector2 t, Vector2 tVelo, int T)
    {
        Vector2 futureTarget = t + tVelo * T;
        return seek(futureTarget);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (path == null) path = pathManager.paths[0];
        if (path.curve) movementType = Mode.curveFollow; else movementType = Mode.pathFollow;
    }

    // Update is called once per frame
    void Update()
    {

        //currTarget = Global.ScreenToWorld(Input.mousePosition); //target is set to mouse

        Vector2 moveForce = new Vector2(); //0,0

        switch (movementType)
        {
            case Mode.pathFollow:
                if (path != null)
                {
                    moveForce = pathFollowing(); //returns spd clamped force needed to "seek" current target
                }
                break;
            case Mode.curveFollow:
                currentPathPercent += velocity / 100 * Time.deltaTime;     //every update calculating current path percentage according to the defined speed TODO check variable use here

                transform.position = path.NewPositionByPath(path.nodesVector3, currentPathPercent); //moving the 'Enemy' to the path position, calculated in method NewPositionByPath
                if (path.rotationByPath)                            //rotating the 'Enemy' in path direction, if set 'rotationByPath'
                {
                    transform.right = path.Interpolate(path.CreatePoints(path.nodesVector3), currentPathPercent + 0.01f) - transform.position;
                    transform.Rotate(Vector3.forward * 90);
                }
                if (currentPathPercent > 1)                    //when the path is complete
                {
                    if (path.loop)                                   //when loop is set, moving to the path starting point; if not, destroying or deactivating the 'Enemy'
                        currentPathPercent = 0;
                    else
                    {
                        Destroy(gameObject); //TODO or, linger
                    }
                }
                break;
            case Mode.seek:
                moveForce = seek(currTarget);
                break;
            case Mode.escape:
                moveForce = flee(currTarget);
                break;
            case Mode.pursuit:
                targetVelocity = currTarget - prevTarget;
                prevTarget = currTarget;
                moveForce = pursuit(currTarget, targetVelocity,
    dynamicT? (int)(Global.findVectorDist(currTarget, (Vector2)transform.position) / max_velocity) :
    constantT); //dynamic T dependent on distance between the two
                break;
            case Mode.wait:
                moveForce = Vector2.zero;
                break;
        }

        moveForce = moveForce / mass;

        if (smooth)
        {
            Vector2 steering = moveForce - currForce; //"currForce" is actually the force in previous run
                                                      //maybe some truncating of the steering force / limit w max force variable here
            steering /= 5; //arbitrary; TODO max force --> how should this be determined? 
            currForce = Vector3.Normalize(currForce + steering) * max_velocity; //so that overall the velo is the same
        }
        else
            currForce = moveForce;

        transform.position = transform.position + (Vector3)currForce;

    }



}


