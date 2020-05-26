﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirTurretController : MonoBehaviour
{
    public float health;
    public float damagePerSecond;
    public float range;
    public Transform pointShoot;
    public GameObject head;
    public SphereCollider impactZone;

    public bool placed;

    List<Collider> colliders = new List<Collider>();

    private bool hasTarget;
    private GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        impactZone.radius = range;
        GameManager.instance.AddActiveDefense(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (placed)
        {
            colliders.RemoveAll(col => col == null);
            if (target == null) hasTarget = false;
            if (!hasTarget)
            {
                foreach (Collider nearbyObject in colliders)
                {
                    target = nearbyObject.gameObject;
                    hasTarget = true;
                    return;
                }                
            }            
            else
            {
                Debug.Log(target);
                head.transform.LookAt(target.transform, Vector3.up);

                RaycastHit hit;
                if (Physics.Raycast(pointShoot.position, (target.transform.position - pointShoot.position).normalized, out hit, range, GameManager.instance.player.shootLayerMask))
                {
                    FlyingEnemy fEnemy = hit.collider.GetComponentInParent<FlyingEnemy>();
                    if (fEnemy != null)
                    {
                        fEnemy.TakeDamage(damagePerSecond * Time.deltaTime);
                        if (fEnemy.health <= 0)
                        {
                            hasTarget = false;
                            colliders.Remove(target.GetComponent<Collider>());
                        }
                    }
                    else hasTarget = false;
                }

                if (Vector3.Distance(pointShoot.position, target.transform.position) > range) hasTarget = false;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0) DestroyDefenses();
    }

    public void DestroyDefenses()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider collider)
    {

        if (collider.GetComponentInParent<FlyingEnemy>() != null && collider.tag != "CriticalBox") colliders.Add(collider); 
            if (collider.gameObject.layer == LayerMask.NameToLayer("EnemyAttack"))
        {
            if (collider.gameObject.GetComponentInParent<FlyingEnemy>() != null) TakeDamage(collider.gameObject.GetComponentInParent<FlyingEnemy>().damage);
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.GetComponentInParent<FlyingEnemy>() != null) colliders.Remove(collider);
    }
}