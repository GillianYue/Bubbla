using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLoader : Loader
{
    private bool[] loadDone; //when loadDone[0] == true, loading is done for the csv file
    public TextAsset enemyCsv;
    private string[,] data; //double array that stores all info 

    int[] enemyCode; //this one's kinda stupid, cuz enemyCode[0] = 0, enemyCode[1] = 1...
    string[] enemyName;
    int[] life, attack;
    float[] sizeScale;
    //float[] colliderScale; //colliderScale currently not in use (b/c of pixel perfect polyCollider), might be useful for scale change during game however
    string[] initial_sprite, s0_anim, s1_anim, s2_anim; //path to animations
    AnimationClip[] S0_ANIM, S1_ANIM, S2_ANIM;
    int[] movement;
    int[] moveSpeed;
    bool[] sprite_on_child;

    string[] projectileName;
    Sprite[] projectileSprites;
    int[] projectileType;
    float[] projectileSpeed, projectileAccl, projectileShootInterval, shootNoise, shootChanceIndividual, shootChanceEnemy; 
    //shootNoise is random noise in shoot interval; shootChanceIndividual is chance each individual shot will be fired; ..Enemy is chance this enemy is *able* to fire

    GameObject enemyMold, projectileMold;

    private bool enemyLoaderDone; //this will be set to true once EnemyLoader is ready for usage

    //colliders will be created (from PixelCollider script) as needed. When a new spawn happens, will check if collider for that enemy already exists here
    List<List<Vector2>>[] enemyColliders;

    [Inject(InjectFrom.Anywhere)]
    public GameControl gameControl;
    [Inject(InjectFrom.Anywhere)]
    public PrefabHolder prefabHolder;

    protected override void Start()
    {
        base.Start();
        loadDone = new bool[1];

        loadEnemyMold(); //ready the mold prefab(s)
        StartCoroutine(LoadScene.processCSV(loadDone, enemyCsv, setData, false)); //processCSV will call setData
        StartCoroutine(parseEnemyData()); //data will be parsed into local type arrays for speedy data retrieval

    }

    void Update()
    {
        
    }

    /// <summary>
    /// overrides parent
    /// </summary>
    /// <returns></returns>
    public override bool isLoadDone()
    {
        return enemyLoaderDone;
    }

    //this function should only be called by EnemySpawner, as it deals with base level data
    public GameObject getEnemyInstance(int eCode)
    {
        if (enemyMold == null)
        {
            Debug.LogError("enemyMold is null... Can't create enemy instance");
            return null;
        }

        GameObject e = Instantiate(enemyMold, enemyMold.transform.position, enemyMold.transform.rotation) as GameObject; //duplicate

        //SpriteRenderer
        if (sprite_on_child[eCode])
        {
            e.GetComponent<SpriteRenderer>().enabled = false;
            e.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
        }

        //Animator
        Animator animator = e.GetComponent<Animator>();

        AnimatorOverrideController animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);

        if (S0_ANIM[eCode] != null)
        {
            animatorOverrideController["State0anim"] = S0_ANIM[eCode];

        }

        if (S1_ANIM[eCode] != null)
            animatorOverrideController["State1anim"] = S1_ANIM[eCode];

        if (S2_ANIM[eCode] != null)
            animatorOverrideController["State2anim"] = S2_ANIM[eCode];

        animator.runtimeAnimatorController = animatorOverrideController;
        animator.Update(0.0f);

        //Enemy stats
        e.name = enemyName[eCode];
        Enemy eScript = e.GetComponent<Enemy>();
        eScript.passReferences(prefabHolder, gameControl);
        //NOTE: it's crucial that setLife is AFTER instantiation!
        eScript.setValues(life[eCode], attack[eCode]);
        eScript.setSizeScale(sizeScale[eCode]);
        //eScript.setColliderScale(colliderScale[eCode]);

        //projectile if exists
        if (projectileName[eCode] != "") {

            eScript.setProjectile(projectileMold, projectileSprites[eCode], eScript.attack, projectileType[eCode], 
                projectileSpeed[eCode], projectileAccl[eCode], projectileShootInterval[eCode], shootNoise[eCode], shootChanceIndividual[eCode], 
                shootChanceEnemy[eCode]);
        }

        EnemyMover mover = e.GetComponent<EnemyMover>();
        mover.enemyType = movement[eCode];
        mover.setSpeed(0, moveSpeed[eCode]); //TODO for now, setting horizontal speed to 0 (most don't move horizontally anyway)

        EnemySteering steer = e.GetComponent<EnemySteering>();
        steer.velocity = moveSpeed[eCode];


        //PolygonCollider2D;    assumes enemyMold prefab has PolygonCollider2D and PixelCollider attached already
        if (enemyColliders[eCode] == null) //collider for this enemy hasn't been generated and stored previously
        {
            List<List<Vector2>> enemy_Paths = e.GetComponent<PixelCollider2D>().Regenerate(sprite_on_child[eCode]);
            enemyColliders[eCode] = enemy_Paths;

        }
        else
        {
            PolygonCollider2D pol = e.GetComponent<PolygonCollider2D>();

            //transferring the points onto PolyCollider
            pol.pathCount = enemyColliders[eCode].Count;
            for (int p = 0; p < enemyColliders[eCode].Count; p++)
            {
                pol.SetPath(p, enemyColliders[eCode][p].ToArray());
            }
        }

        return e;
    }

    void loadEnemyMold()
    {
        enemyMold = Resources.Load("Prefabs/EnemyMold") as GameObject;
        if (enemyMold == null) Debug.LogError("load EnemyMold failed");

        projectileMold = Resources.Load("Projectile") as GameObject;
    }

    IEnumerator parseEnemyData()
    {
        yield return new WaitUntil(() => loadDone[0]); //this would mean that data is ready to be parsed

        int numRows = data.GetLength(1); 
        enemyCode = new int[numRows - 1]; //num rows, int[] is for the entire column
        enemyName = new string[numRows - 1];
        life = new int[numRows-1]; attack = new int[numRows-1];
        sizeScale = new float[numRows - 1]; // colliderScale = new float[numRows-1];
        initial_sprite = new string[numRows - 1];
        s0_anim = new string[numRows - 1]; s1_anim = new string[numRows - 1]; s2_anim = new string[numRows - 1];
        S0_ANIM = new AnimationClip[numRows - 1];  S1_ANIM = new AnimationClip[numRows - 1]; S2_ANIM = new AnimationClip[numRows - 1];
        movement = new int[numRows-1]; moveSpeed = new int[numRows - 1];
        sprite_on_child = new bool[numRows - 1];

        projectileName = new string[numRows - 1]; projectileSprites = new Sprite[numRows - 1]; 
        projectileType = new int[numRows - 1];
        projectileSpeed = new float[numRows - 1]; projectileAccl = new float[numRows - 1];
        projectileShootInterval = new float[numRows - 1]; shootNoise = new float[numRows - 1];
        shootChanceIndividual = new float[numRows - 1]; shootChanceEnemy = new float[numRows - 1];

        enemyColliders = new List<List<Vector2>>[numRows - 1];

        //skip row 0 because those are all descriptors
        for (int r = 1; r < numRows; r++) //-1 because title row doesn't count
        {
            enemyName[r-1] = data[1, r]; //r-1 is for such that enemyName[enemyCode] matches that with the data
            int.TryParse(data[2, r], out life[r - 1]);
            int.TryParse(data[3, r], out attack[r - 1]);
            float.TryParse(data[4, r], out sizeScale[r - 1]);
            //float.TryParse(data[5, r], out colliderScale[r - 1]); 
            initial_sprite[r - 1] = data[5, r];
            s0_anim[r - 1] = data[6, r];
            s1_anim[r - 1] = data[7, r];
            s2_anim[r - 1] = data[8, r];
            int.TryParse(data[9, r], out movement[r - 1]);
            int.TryParse(data[10, r], out moveSpeed[r - 1]);
            if (data[11, r] != "") bool.TryParse(data[11, r], out sprite_on_child[r - 1]);
            if (data[12, r] != "") projectileName[r - 1] = data[12, r];
            if (data[13, r] != "") int.TryParse(data[13, r], out projectileType[r - 1]);
            if (data[14, r] != "") float.TryParse(data[14, r], out projectileSpeed[r - 1]);
            if (data[15, r] != "") float.TryParse(data[15, r], out projectileAccl[r - 1]);
            if (data[16, r] != "") float.TryParse(data[16, r], out projectileShootInterval[r - 1]);
            if (data[17, r] != "") float.TryParse(data[17, r], out shootNoise[r - 1]);
            if (data[18, r] != "") float.TryParse(data[18, r], out shootChanceIndividual[r - 1]);
            if (data[19, r] != "") float.TryParse(data[18, r], out shootChanceEnemy[r - 1]);
        }

        loadAnimationClips(S0_ANIM, S1_ANIM, S2_ANIM);
        loadSprites(projectileSprites);

        yield return new WaitUntil(() => {
            return (S0_ANIM[S0_ANIM.Length - 1] != null); }); //anim loaded, theoretically everything all set

        enemyLoaderDone = true;
        Debug.Log("EnemyLoader ready");
    }

    public void setData(string[,] d)
    {
        data = d;
    }

    private void loadAnimationClips(AnimationClip[] s0, AnimationClip[] s1, AnimationClip[] s2)
    {

        //load the animation clips into the arrays
        for (int eCode = 0; eCode < s0.GetLength(0); eCode++)
        {
            if (!s0_anim[eCode].Equals(""))
            {
                var tmpAnim0 = Resources.Load("Animation/"+s0_anim[eCode]) as AnimationClip;

            if (tmpAnim0 == null)
            {
                Debug.LogError("Animation 0 NOT found");
            }
            else
            {
                s0[eCode] = tmpAnim0;
            }
        }


            if (!s1_anim[eCode].Equals(""))
            {
                var tmpAnim1 = Resources.Load("Animation/" + s1_anim[eCode]) as AnimationClip;

                if (tmpAnim1 == null)
            {
                Debug.LogError("Animation 1 NOT found");
            }
            else
            {
                s1[eCode] = tmpAnim1;
            }
        }

            if (!s2_anim[eCode].Equals(""))
            {
                var tmpAnim2 = Resources.Load("Animation/" + s2_anim[eCode]) as AnimationClip;

                if (tmpAnim2 == null)
                {
                    Debug.LogError("Animation 2 NOT found");
                }
                else
                {
                    s2[eCode] = tmpAnim2;
                }
            }
        }
    }

    private void loadSprites(Sprite[] projSprites)
    {
        for (int eCode = 0; eCode < projectileName.GetLength(0); eCode++)
        {
            if (projectileName[eCode] != null)
            {
                Sprite spr = Resources.Load<Sprite>("Sprites/" + projectileName[eCode]);

                if (spr == null)
                {
                    Debug.LogError("sprite "+ projectileName[eCode]+" NOT found");
                }
                else
                {
                    projSprites[eCode] = spr;
                }
            }

        }
    }

}
