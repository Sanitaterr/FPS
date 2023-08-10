using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour // 负责真实改变物体坐标和方向
{
    [SerializeField]
    private Rigidbody rb; // 获取物理属性
    [SerializeField]
    private Camera cam;

    private Vector3 velocity = Vector3.zero; // 速度，每秒钟移动的距离
    private Vector3 yRotation = Vector3.zero; // 旋转角色
    private Vector3 xRotation = Vector3.zero; // 旋转视角
    private float recoilForce = 0f; // 后坐力

    private float cameraRoationTotal = 0f; // 累计转了多少度
    [SerializeField]
    private float camearRotationLimit = 85f;

    private Vector3 thrusterForce = Vector3.zero; // 向上的推力

    private float eps = 0.01f;
    private Vector3 lastFramePosition = Vector3.zero; // 记录上一帧的位置
    private Animator animator;

    private float distToGround = 0f;

    private void Start()
    {
        lastFramePosition = transform.position;
        animator = GetComponentInChildren<Animator>();
        distToGround = GetComponent<Collider>().bounds.extents.y;
    }

    public void Move(Vector3 _velocity) // 将Input里获取的速度传过来
    {
        velocity = _velocity;
    }

    public void Rotate(Vector3 _yRotation, Vector3 _xRotation)
    {
        yRotation = _yRotation;
        xRotation = _xRotation;
    }

    public void Thrust(Vector3 _thrusterForce)
    {
        thrusterForce = _thrusterForce;
    }

    public void AddRecoilForce(float newRecoilForce)
    {
        recoilForce += newRecoilForce;
    }

    private void PerformMovement() // 朝着velocity方向移动一段距离
    {
        if (velocity != Vector3.zero) // 重要的优化，速度不是0再去计算
        {
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime); // fixedDeltaTime：相邻两次FixedUpdate执行间隔 
        }
        if (thrusterForce != Vector3.zero)
        {
            rb.AddForce(thrusterForce); // 给刚体作用一个力，作用Time.fixedDeltaTime秒
            thrusterForce = Vector3.zero;
        }
    }

    private void PerfromRotation()
    {
        if (recoilForce < 0.1)
        {
            recoilForce = 0f;
        }

        if (yRotation != Vector3.zero || recoilForce > 0)
        {
            rb.transform.Rotate(yRotation + rb.transform.up * Random.Range(-2f * recoilForce, 2f * recoilForce)); // 左右方向后坐力
        }

        if (xRotation != Vector3.zero || recoilForce > 0)
        {
            // cam.transform.Rotate(xRotation);
            cameraRoationTotal += xRotation.x - recoilForce; // 累加
            cameraRoationTotal = Mathf.Clamp(cameraRoationTotal, -camearRotationLimit, camearRotationLimit); // 处理累加值，夹逼，小于-camearRotationLimit就改成-camearRotationLimit，大于camearRotationLimit就改成camearRotationLimit
            cam.transform.localEulerAngles = new Vector3(cameraRoationTotal, 0f, 0f); // 更新x方向旋转
        }

        recoilForce *= 0.5f;
    }

    private void PerformAnimation()
    {
        Vector3 deltaPosition = transform.position - lastFramePosition;
        lastFramePosition = transform.position;

        float forward = Vector3.Dot(deltaPosition, transform.forward);
        float right = Vector3.Dot(deltaPosition, transform.right);

        int direction = 0; // 静止
        if (forward > eps)
        {
            direction = 1; // 前
        } else if (forward < -eps)
        {
            if (right > eps)
            {
                direction = 4; // 右后
            } else if (right < -eps)
            {
                direction = 6; // 左后
            }else
            {
                direction = 5; // 后
            }
        } else if (right > eps)
        {
            direction = 3; // 右
        } else if (right < -eps)
        {
            direction = 7; // 左
        }

        if (!Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f))
        {
            direction = 8;
        }

        if (GetComponent<Player>().IsDead())
        {
            direction = -1;
        }

        animator.SetInteger("direction", direction);
    }

    private void FixedUpdate() // 用于处理物理轨迹的一般使用FixedUpdate,其他则是Update // 本地玩家是在FixedUpdate里修改
    {
        if (IsLocalPlayer)
        {
            PerformMovement();
            PerfromRotation();
        }

        if (IsLocalPlayer) // 防止出现小碎步
        {
            PerformAnimation();
        }
    }

    private void Update() // 远程玩家是在Update里修改
    {
        if (!IsLocalPlayer)
        {
            PerformAnimation(); // 防止出现小碎步
        }
    }
}
