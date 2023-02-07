using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : Attack
{
    public float speed = 20f;
    public float lifespan = .2f;

    private Vector3 _direction;
    // Start is called before the first frame update
    public void SetDirection(Vector3 direction) {
        _direction = direction;
    }
    private void Awake() {
        rootAmount = 20f;
    }

    private void FixedUpdate() {
        transform.position += _direction * (Time.deltaTime * speed);
        lifespan -= Time.deltaTime;
        if (lifespan<0) {
            Destroy(gameObject);
        }
    }

    public override void OnCollition()
    {
        //Destroy(gameObject);
    }
}