using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Steering : MonoBehaviour
{

    public SteerPath path;
    public int currentNode;
    public int precisionRadius = 10; //how close to the target the obj has to be to qualify "getting there"
    public float max_velocity = 1, velocity, mass = 1;
    public bool smooth; //if true, will not instantly change velocity, but will steer towards ideal velocity
    public Vector2 currForce, target;

    public GameObject markingPrefab;

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

    /// <summary>
    /// linear, movement is straight line and will result in sudden turns and changes of direction
    ///
    /// this function returns normalized force needed to "seek" the target * max_velocity
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    private Vector2 seek(Vector2 t)
    {
        return Vector3.Normalize(t - (Vector2)transform.position) * max_velocity;
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

        if (path != null)
        {
            Vector2 seekForce = pathFollowing(); //returns spd clamped force needed to "seek" current target
            seekForce = seekForce / mass;

            if (smooth)
            {
                Vector2 steering = seekForce - currForce; //"currForce" is actually the force in previous run
                                                          //maybe some truncating of the steering force / limit w max force variable here
                steering /= 5; //arbitrary; TODO
                currForce = Vector3.Normalize(currForce + steering) * max_velocity; //so that overall the velo is the same
            }
            else
                currForce = seekForce;

            transform.position = transform.position + (Vector3)currForce;
        }

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
