using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponGraphics : MonoBehaviour
{
    public ParticleSystem muzzleFlash; // 枪口火花
    public GameObject metalHitEffectPrefab; // 击中金属特效 动态生成的一般都叫GameObject
    public GameObject stoneHitEffectPrefab; // 击中石头特效
}
