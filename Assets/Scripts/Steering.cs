using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//GO movement
public class Steering : MonoBehaviour
{
    //Mode.pathFollow uses:
    public SteerPath path;
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
    public enum Mode { pathFollow, seek, escape, pursuit }
    //TODO arrival which slows down when getting close to target, obj never passing the destination
    public Mode movementType;

    public GameObject markingPrefab;

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
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (path == null) path = new SteerPath();
            Vector3 mouse = Global.ScreenToWorld(Input.mousePosition);
            path.addNode(mouse);
            GameObject marking = Instantiate(markingPrefab);
            marking.transform.position = mouse;

        }

        currTarget = Global.ScreenToWorld(Input.mousePosition); //target is set to mouse

        Vector2 moveForce = new Vector2(); //0,0

        switch (movementType)
        {
            case Mode.pathFollow:
                if (path != null)
                {
                    moveForce = pathFollowing(); //returns spd clamped force needed to "seek" current target
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

public class SteerPath
{
    private ArrayList nodes;
 
    public SteerPath()
	{
        this.nodes = new ArrayList();
	}

	public void addNode(Vector2 coord){
        nodes.Add(coord);
    }

    public ArrayList getNodes(){
        return nodes;
    }
}
