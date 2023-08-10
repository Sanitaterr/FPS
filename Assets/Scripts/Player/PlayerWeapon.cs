using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable] // 能够串行化
public class PlayerWeapon
{
    public string name = "M16A1";
    public int damage = 10;
    public float range = 100f;

    public float shootRate = 10f; // 一秒打多少发子弹，如果小于等于0，则为单发，
    public float shootCoolDownTime = 0.75f; // 单发模式的冷却时间
    public float recoilForce = 2f; // 后坐力

    public int maxBullets = 30;
    public int bullets = 30;
    public float reloadTime = 2f;

    [HideInInspector]
    public bool isReloading = false;

    public GameObject graphics;
}
