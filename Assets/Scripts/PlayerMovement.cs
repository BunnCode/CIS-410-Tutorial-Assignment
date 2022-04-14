using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float turnSpeed = 20f;

    Animator m_Animator;
    Rigidbody m_Rigidbody;
    AudioSource m_AudioSource;
    Vector3 m_Movement;
    Quaternion m_Rotation = Quaternion.identity;

    private List<Transform> enemies = new List<Transform>();
    public Transform enemyParent;
    public float fearDistance = 5;
    private Light fearLight;
    private float fearLightMaxBrightness;
    void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_AudioSource = GetComponent<AudioSource>();
        //Get all enemies
        foreach(Transform t in enemyParent)
        {
            enemies.Add(t);
        }
        //add the light
        fearLight = GetComponentInChildren<Light>();
        fearLight.enabled = false;
        fearLightMaxBrightness = fearLight.intensity;
    }

    void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        m_Movement.Set(horizontal, 0f, vertical);
        m_Movement.Normalize();

        bool hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
        bool hasVerticalInput = !Mathf.Approximately(vertical, 0f);
        bool isWalking = hasHorizontalInput || hasVerticalInput;
        m_Animator.SetBool("IsWalking", isWalking);

        if (isWalking)
        {
            if (!m_AudioSource.isPlaying)
            {
                m_AudioSource.Play();
            }
        }
        else
        {
            m_AudioSource.Stop();
        }

        Vector3 desiredForward = Vector3.RotateTowards(transform.forward, m_Movement, turnSpeed * Time.deltaTime, 0f);
        m_Rotation = Quaternion.LookRotation(desiredForward);

   
        Transform closestEnemy = null;
        float closestDistance = float.PositiveInfinity;
        //Not super duper efficient but there are like 10 enemies so it doesn't really matter
        foreach (var enemy in enemies) {
            float distance = Vector3.Distance(enemy.position, transform.position);
            //too far away
            if (distance > fearDistance)
                continue;
            if (distance < closestDistance) {
                closestEnemy = enemy;
                closestDistance = distance;
            }
        }
        //If we are near an enemy
        if (closestEnemy) {
            fearLight.enabled = true;
            fearLight.intensity = Mathf.Lerp(fearLightMaxBrightness, 0, (closestDistance / fearDistance));
        }
        else {
            fearLight.enabled = false;
        }
    }

    void OnAnimatorMove()
    {
        m_Rigidbody.MovePosition(m_Rigidbody.position + m_Movement * m_Animator.deltaPosition.magnitude);
        m_Rigidbody.MoveRotation(m_Rotation);
    }
}