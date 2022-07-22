using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Controller")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gravity = 20f;
    [SerializeField] private LayerMask aimLayer = Physics.AllLayers;
    
    [Header("Weapons")]
    [SerializeField] private GameObject MainWeapon;
    [SerializeField] private GameObject FinishingWeapon;
    
    [Header("Animation")]
    [SerializeField] private float rotationSpeed = 20f;
    [SerializeField] private float bodyRotationOffset = 90f;
    [SerializeField] private Transform targetBone;
    [SerializeField] private Animator animator;

    [Header("Finishing")]
    [SerializeField] private Vector3 finishingOffset;
    [SerializeField] private float finishingDistance = 1f;
    [SerializeField] private float finishingRadius = 0.5f;
    [SerializeField] private float finishingTime = 1f;
    [SerializeField] private Transform finishingText;

    private int inputMagnitudeHash = Animator.StringToHash("InputMagnitude");
    private int horizontalHash = Animator.StringToHash("Horizontal");
    private int verticalHash = Animator.StringToHash("Vertical");
    private int finishHash = Animator.StringToHash("Finish");

    private float inputBlocked = 0;
    private CharacterController controller;
    private Vector3 velocity;
    private Enemy lastHitTarget;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        SetMainWeapon(true);
    }

    public void SetMainWeapon(bool isActive)
    {
        MainWeapon.SetActive(isActive); 
        FinishingWeapon.SetActive(!isActive); 
    }

    public Transform GetFinishingWeapon()
    {
        return FinishingWeapon.transform;
    }

    public void OnEnemyHit()
    {
        lastHitTarget.Kill(this);
    }

    private void Move(Vector3 input)
    {
        velocity = new Vector3(input.x * moveSpeed, velocity.y, input.z * moveSpeed);
        
        if (controller.isGrounded)
        {
            velocity.y = 0;
        }

        velocity.y -= gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    public bool CheckEnemy(out Enemy enemy)
    {
        Collider[] colliders = Physics.OverlapSphere(targetBone.TransformPoint(finishingOffset / 100), finishingRadius);

        for (int i = 0; i < colliders.Length; i++)
        {
            if(colliders[i].TryGetComponent(out enemy))
            {
                return true;
            }
        }

        enemy = null;
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(targetBone.TransformPoint(finishingOffset / 100), finishingRadius);
    }

    void LateUpdate()
    {
        inputBlocked -= Time.deltaTime;

        Vector2 input = Vector2.zero;
        if (inputBlocked <= 0)
        {
            input = Vector2.ClampMagnitude(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")), 1);
        
            if(Physics.Raycast(Camera.allCameras[0].ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100, aimLayer))
            {
                Vector3 diff = hit.point - transform.position;
                diff.Normalize();

                float rot = Mathf.Atan2(diff.z, diff.x) * Mathf.Rad2Deg;
                targetBone.rotation *= Quaternion.Euler(rot + transform.eulerAngles.y - bodyRotationOffset, 0, 0);
            }
        }

        Vector3 cameraForward = Quaternion.Euler(0, Camera.allCameras[0].transform.eulerAngles.y, 0) * new Vector3(input.x, 0, input.y);

        Move(cameraForward);

        if(input.magnitude > 0)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(cameraForward), rotationSpeed * Time.deltaTime);

        cameraForward = transform.InverseTransformDirection(cameraForward);


        animator.SetFloat(inputMagnitudeHash, cameraForward.magnitude, 0.1f, Time.deltaTime);
        animator.SetFloat(horizontalHash, cameraForward.x, 0.1f, Time.deltaTime);
        animator.SetFloat(verticalHash, cameraForward.z, 0.1f, Time.deltaTime);
        
        if(inputBlocked <= 0 && CheckEnemy(out Enemy enemy))
        {
            finishingText.gameObject.SetActive(true);
            if(Input.GetKeyDown(KeyCode.Space))
            {
                lastHitTarget = enemy;
                SetMainWeapon(false);
                inputBlocked = finishingTime;
                animator.SetTrigger(finishHash);
                Vector3 diff = enemy.transform.position - transform.position;
                controller.enabled = false;
                transform.position = enemy.transform.position - diff.normalized * finishingDistance;
                controller.enabled = true;

                diff.y = 0;
                transform.rotation = Quaternion.LookRotation(diff);
            }
        }
        else
        {
            finishingText.gameObject.SetActive(false);
        }
    }
}
