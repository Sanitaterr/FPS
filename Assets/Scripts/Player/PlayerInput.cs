using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour // �����û�����
{
    [SerializeField] // �ô�����д��ֵ������unity�����
    private float speed = 5f; // �ٶȱ���
    [SerializeField]
    private float lookSensitivity = 5f; // ���������
    [SerializeField]
    private float thrusterForce = 20f;
    [SerializeField]
    private PlayerController controller;

    private float distToGround = 0f;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // ��ס��꣬�������ʧ
        distToGround = GetComponent<Collider>().bounds.extents.y;
    }

    // Update is called once per frame
    void Update()
    {
        float xMov = Input.GetAxisRaw("Horizontal"); // ��ȡx������û���ƶ�,x����ĵ�λ����
        float yMov = Input.GetAxisRaw("Vertical");

        Vector3 velocity = (transform.right * xMov + transform.forward * yMov).normalized * speed; // �ٶȺϳɣ�������ֱ�������Ȼ���׼��������һ�������ٶ�Ϊ1���ٶ�
        controller.Move(velocity); // �����ٶ�

        float xMouse = Input.GetAxisRaw("Mouse X"); // ��ȡ���ġ�������
        float yMouse = Input.GetAxisRaw("Mouse Y");

        Vector3 yRotation = new Vector3(0f, xMouse, 0f) * lookSensitivity;
        Vector3 xRotation = new Vector3(-yMouse, 0f, 0f) * lookSensitivity;
        controller.Rotate(yRotation, xRotation);

        if (Input.GetButtonDown("Jump")) // ������˿ո��
        {
            if (Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f))
            {
                Vector3 force = Vector3.up * thrusterForce; // ���Ĵ�С��20����������
                controller.Thrust(force);
            }
            
        }
    }
}