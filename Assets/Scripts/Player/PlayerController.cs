using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour // ������ʵ�ı���������ͷ���
{
    [SerializeField]
    private Rigidbody rb; // ��ȡ��������
    [SerializeField]
    private Camera cam;

    private Vector3 velocity = Vector3.zero; // �ٶȣ�ÿ�����ƶ��ľ���
    private Vector3 yRotation = Vector3.zero; // ��ת��ɫ
    private Vector3 xRotation = Vector3.zero; // ��ת�ӽ�
    private float recoilForce = 0f; // ������

    private float cameraRoationTotal = 0f; // �ۼ�ת�˶��ٶ�
    [SerializeField]
    private float camearRotationLimit = 85f;

    private Vector3 thrusterForce = Vector3.zero; // ���ϵ�����

    private float eps = 0.01f;
    private Vector3 lastFramePosition = Vector3.zero; // ��¼��һ֡��λ��
    private Animator animator;

    private float distToGround = 0f;

    private void Start()
    {
        lastFramePosition = transform.position;
        animator = GetComponentInChildren<Animator>();
        distToGround = GetComponent<Collider>().bounds.extents.y;
    }

    public void Move(Vector3 _velocity) // ��Input���ȡ���ٶȴ�����
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

    private void PerformMovement() // ����velocity�����ƶ�һ�ξ���
    {
        if (velocity != Vector3.zero) // ��Ҫ���Ż����ٶȲ���0��ȥ����
        {
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime); // fixedDeltaTime����������FixedUpdateִ�м�� 
        }
        if (thrusterForce != Vector3.zero)
        {
            rb.AddForce(thrusterForce); // ����������һ����������Time.fixedDeltaTime��
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
            rb.transform.Rotate(yRotation + rb.transform.up * Random.Range(-2f * recoilForce, 2f * recoilForce)); // ���ҷ��������
        }

        if (xRotation != Vector3.zero || recoilForce > 0)
        {
            // cam.transform.Rotate(xRotation);
            cameraRoationTotal += xRotation.x - recoilForce; // �ۼ�
            cameraRoationTotal = Mathf.Clamp(cameraRoationTotal, -camearRotationLimit, camearRotationLimit); // �����ۼ�ֵ���бƣ�С��-camearRotationLimit�͸ĳ�-camearRotationLimit������camearRotationLimit�͸ĳ�camearRotationLimit
            cam.transform.localEulerAngles = new Vector3(cameraRoationTotal, 0f, 0f); // ����x������ת
        }

        recoilForce *= 0.5f;
    }

    private void PerformAnimation()
    {
        Vector3 deltaPosition = transform.position - lastFramePosition;
        lastFramePosition = transform.position;

        float forward = Vector3.Dot(deltaPosition, transform.forward);
        float right = Vector3.Dot(deltaPosition, transform.right);

        int direction = 0; // ��ֹ
        if (forward > eps)
        {
            direction = 1; // ǰ
        } else if (forward < -eps)
        {
            if (right > eps)
            {
                direction = 4; // �Һ�
            } else if (right < -eps)
            {
                direction = 6; // ���
            }else
            {
                direction = 5; // ��
            }
        } else if (right > eps)
        {
            direction = 3; // ��
        } else if (right < -eps)
        {
            direction = 7; // ��
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

    private void FixedUpdate() // ���ڴ�������켣��һ��ʹ��FixedUpdate,��������Update // �����������FixedUpdate���޸�
    {
        if (IsLocalPlayer)
        {
            PerformMovement();
            PerfromRotation();
        }

        if (IsLocalPlayer) // ��ֹ����С�鲽
        {
            PerformAnimation();
        }
    }

    private void Update() // Զ���������Update���޸�
    {
        if (!IsLocalPlayer)
        {
            PerformAnimation(); // ��ֹ����С�鲽
        }
    }
}
