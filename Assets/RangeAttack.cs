using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeAttack : Attack
{
    public float speed = 10;
    public float lifespan = 3;

    private Vector3 _direction;
    // Start is called before the first frame update
    public void SetDirection(Vector3 direction) {
        _direction = direction;
    }
    private void Awake() {
        rootAmount = 10f;
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
        Destroy(gameObject);
    }
}