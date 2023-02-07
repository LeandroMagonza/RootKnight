using System;
using System.Collections;
using System.Collections.Generic;
using Platformer.Gameplay;
using UnityEngine;
using UnityEngine.UIElements;
using static Platformer.Core.Simulation;
using Random = UnityEngine.Random;

namespace Platformer.Mechanics
{
    /// <summary>
    /// A simple controller for enemies. Provides movement control over a patrol path.
    /// </summary>
    [RequireComponent( typeof(Collider2D))]
    public class EnemyController : MonoBehaviour
    {
        public PatrolPath path;
        public AudioClip ouch;

        
        internal Collider2D _collider;
        internal AudioSource _audio;
        SpriteRenderer spriteRenderer; 
        private Rigidbody2D _rigidBody2D;
        private Animator _animator;
        public Animator _rootAnimator;

        public AudioClip raiz;
        public float speed = 3;
        
        public float rootAmount = 0;
        public float maxRootAmount = 30;

        public float playerDamage = 10;
        public float treeDamage = 50;

        public bool isGrounded;
        public GameObject rootedIndicator;
        
        public float timeUntilJump = 2;
        public float maxTimeUntilJump = 2;

        public FacingDirection direction = FacingDirection.LEFT;
        
        public bool isFullyRooted { get; set; }
        public float fullyRootedTimer = 0;
        public float maxFullyRootedTimer = 5;
        public float derootSpeed = 1;
        public float jumpForce = 0;
        public bool isConsumble = true;


        void Awake()
        {
            _collider = GetComponent<Collider2D>();
            _audio = GetComponent<AudioSource>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            _rigidBody2D = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            SetDirection((FacingDirection)Random.Range(0,2));
        }

        void Update()
        {
            
        }
        void OnCollisionEnter2D(Collision2D collision) {
            if (collision.gameObject.layer == 3) {
                isGrounded = true;
                _animator.SetBool("grounded", true);
            }

        }

        void OnCollisionExit2D(Collision2D collision) {
            if (collision.gameObject.layer == 3) {
                isGrounded = false;
                _animator.SetBool("grounded",false);
            }
        }

        private void OnTriggerEnter2D(Collider2D col) {
            var attack = col.gameObject.GetComponent<Attack>();
            if (attack) {
                rootAmount += attack.rootAmount;
                if (rootAmount >= maxRootAmount) {
                    isFullyRooted = true;
                    rootedIndicator.SetActive(true);
                    fullyRootedTimer = maxFullyRootedTimer;
                    _audio.PlayOneShot(raiz);
                }

                attack.OnCollition();
            }
            
            var directionChanger = col.gameObject.GetComponent<DirectionChanger>();

            if (directionChanger)
            {
                if (direction == FacingDirection.LEFT)
                {
                    SetDirection(FacingDirection.RIGHT);
                }
                else
                {
                    SetDirection(FacingDirection.LEFT);
                }
                return;
            }
            
            var hurtbox = col.gameObject.GetComponent<Hurtbox>();

            if (hurtbox != null && !isFullyRooted) {
                hurtbox.player.ReceiveDamage(playerDamage);
                //deal damage
            }
            var treeEntrance = col.gameObject.GetComponent<TreeEntrance>();

            if (treeEntrance) {
                treeEntrance.ReceiveDamage(treeDamage);
                
                Die();
            }
        }

        private void Die() {
            Destroy(gameObject);
        }

        public void GetConsumed() {
            _rootAnimator.SetTrigger("consume");
            spriteRenderer.enabled = false;
            isConsumble = false;
            Destroy(gameObject,1f);
        }

        void FixedUpdate() {
            
            if (!isFullyRooted)
            {
                if (timeUntilJump<=0) {
                    timeUntilJump = maxTimeUntilJump;
                    float rootSpeedReduction = 1- (rootAmount / maxRootAmount);
                    if (direction == FacingDirection.LEFT)
                    {
                        _rigidBody2D.AddForce(jumpForce * new Vector2(0.5f*rootSpeedReduction,0.5f));
                    }
                    else
                    {
                        _rigidBody2D.AddForce(jumpForce * new Vector2(-0.5f*rootSpeedReduction,0.5f));
                    }
                }
                else {
                    timeUntilJump -= Time.deltaTime;
                }

            }

            if (isFullyRooted) {
                fullyRootedTimer -= Time.deltaTime;
                if (fullyRootedTimer<0) {
                    isFullyRooted = false;
                    rootedIndicator.SetActive(false);
                }
            }
            else if (rootAmount > 0) {
                rootAmount -= Time.deltaTime*derootSpeed;
            }
            _rootAnimator.SetFloat("rootPercentaje", (rootAmount / maxRootAmount)*100);
        }

        public void SetDirection(FacingDirection newDirection) {
            if (newDirection == FacingDirection.RIGHT) {
                direction = FacingDirection.RIGHT;
                spriteRenderer.flipX = true;
            }
            else {
                direction = FacingDirection.LEFT;
                spriteRenderer.flipX = false;
            }
        }

    }
}

public enum FacingDirection{
    LEFT,
    RIGHT
}