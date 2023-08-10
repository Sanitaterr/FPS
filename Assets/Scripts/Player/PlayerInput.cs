using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour // 负责用户输入
{
    [SerializeField] // 让代码里写的值可以在unity里调试
    private float speed = 5f; // 速度倍率
    [SerializeField]
    private float lookSensitivity = 5f; // 鼠标灵敏度
    [SerializeField]
    private float thrusterForce = 20f;
    [SerializeField]
    private PlayerController controller;

    private float distToGround = 0f;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // 锁住鼠标，让鼠标消失
        distToGround = GetComponent<Collider>().bounds.extents.y;
    }

    // Update is called once per frame
    void Update()
    {
        float xMov = Input.GetAxisRaw("Horizontal"); // 获取x方向有没有移动,x方向的单位距离
        float yMov = Input.GetAxisRaw("Vertical");

        Vector3 velocity = (transform.right * xMov + transform.forward * yMov).normalized * speed; // 速度合成，两个垂直向量相加然后标准化，即朝一个方向速度为1的速度
        controller.Move(velocity); // 传入速度

        float xMouse = Input.GetAxisRaw("Mouse X"); // 获取鼠标的。。。。
        float yMouse = Input.GetAxisRaw("Mouse Y");

        Vector3 yRotation = new Vector3(0f, xMouse, 0f) * lookSensitivity;
        Vector3 xRotation = new Vector3(-yMouse, 0f, 0f) * lookSensitivity;
        controller.Rotate(yRotation, xRotation);

        if (Input.GetButtonDown("Jump")) // 如果摁了空格键
        {
            if (Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f))
            {
                Vector3 force = Vector3.up * thrusterForce; // 力的大小是20，方向向上
                controller.Thrust(force);
            }
            
        }
    }
}