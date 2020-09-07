using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabHolder : MonoBehaviour
{
    // UI prefabs
    public GameObject gaugePaintSprite, gaugeSelectedHalo;

    // player stats
    public GameObject[] hearts, pallets;

    //vfx
    public GameObject heartPop, aim;

    //bullet related vfx
    public GameObject palletExplosion, palletTrail;
    //number and order of GOs below refer to PaintballBehavior.ColorMode
    public GameObject palletExplosionRed, palletExplosionBlue, palletExplosionYellow;
    public GameObject palletTrailRed, palletTrailBlue, palletTrailYellow;
}
