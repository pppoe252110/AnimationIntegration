using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float respawnTime = 5f;
    [SerializeField] private float punchForce = 5f;

    private Rigidbody[] ragdollBodies;
    private Animator animator;
    private Collider collider;

    void Start()
    {
        ragdollBodies = GetComponentsInChildren<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        collider = GetComponent<Collider>();
        SetRagdollActive(false);
    }

    public void SetRagdollActive(bool isActive)
    {
        collider.enabled = !isActive;
        animator.enabled = !isActive;
        for (int i = 0; i < ragdollBodies.Length; i++)
        {
            ragdollBodies[i].isKinematic = !isActive;
        }
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnTime);
        SetRagdollActive(false);
        transform.position = new Vector3(Random.Range(-25, 25), 0, Random.Range(-25, 25));
    }

    public void Kill(PlayerController player)
    {
        SetRagdollActive(true);
        for (int i = 0; i < ragdollBodies.Length; i++)
        {
            ragdollBodies[i].AddForce((ragdollBodies[i].position - player.GetFinishingWeapon().position).normalized * punchForce, ForceMode.VelocityChange);
        }
        StartCoroutine(Respawn());
    }
}
