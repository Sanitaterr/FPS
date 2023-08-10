using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField]
    private int maxHealth = 100;
    [SerializeField]
    private Behaviour[] componentsToDisable;
    private bool[] componentsEnable;
    private bool colliderEnable; // ���Ǽ̳���NetworkBehaviour����Ҫ��������

    private NetworkVariable<int> currentHealth = new NetworkVariable<int>();
    private NetworkVariable<bool> isDead = new NetworkVariable<bool>();

    public void Setup()
    {
        componentsEnable = new bool[componentsToDisable.Length];
        for (int i = 0; i < componentsToDisable.Length; i++)
        {
            componentsEnable[i] = componentsToDisable[i].enabled;
        }
        Collider col = GetComponent<Collider>();
        colliderEnable = col.enabled;

        SetDefaults();
    }

    private void SetDefaults()
    {
        for (int i = 0; i < componentsToDisable.Length; i ++)
        {
            componentsToDisable[i].enabled = componentsEnable[i];
        }
        Collider col = GetComponent<Collider>();
        col.enabled = colliderEnable;

        if (IsServer)
        {
            currentHealth.Value = maxHealth;
            isDead.Value = false;
        }
    }

    public bool IsDead()
    {
        return isDead.Value;
    }

    public void TakeDamage(int damage) // �ܵ��˺���ֻ�ڷ������˵���
    {
        if (isDead.Value) return;

        currentHealth.Value -= damage;

        if (currentHealth.Value <= 0)
        {
            currentHealth.Value = 0;
            isDead.Value = true;

            if (!IsHost)
            {
                DieOnServer(); // ���ڷ������˵���
            }
            DieClientRpc(); // �ٷ��͵��ͻ��˵���
        }
    }

    private IEnumerator Respawn() // ����  
    {
        yield return new WaitForSeconds(GameManager.Singleton.MatchingSettings.respawnTime);

        SetDefaults();
        GetComponentInChildren<Animator>().SetInteger("direction", 0);
        GetComponent<Rigidbody>().useGravity = true;

        if (IsLocalPlayer)
        {
            transform.position = new Vector3(0f, 10f, 0f);
        }
    }
    
    private void DieOnServer()
    {
        Die();
    }

    [ClientRpc]
    private void DieClientRpc()
    {
        Die();
    }

    private void Die() // ȥ������
    {
        GetComponent<PlayerShooting>().StopShooting();

        GetComponentInChildren<Animator>().SetInteger("direction", -1);
        GetComponent<Rigidbody>().useGravity = false;

        for (int i = 0; i < componentsEnable.Length; i ++)
        {
            componentsToDisable[i].enabled = false;
        }
        Collider col = GetComponent<Collider>();
        col.enabled = false;

        StartCoroutine(Respawn()); // �����߳�ȥִ�и���
    }
    
    public int GetHealth()
    {
        return currentHealth.Value;
    }
}
