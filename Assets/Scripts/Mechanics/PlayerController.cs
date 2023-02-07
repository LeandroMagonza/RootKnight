using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Gameplay;
using static Platformer.Core.Simulation;
using Platformer.Model;
using Platformer.Core;
using UnityEngine.UI;

namespace Platformer.Mechanics {
    /// <summary>
    /// This is the main class used to implement control of the player.
    /// It is a superset of the AnimationController class, but is inlined to allow for any kind of customisation.
    /// </summary>
    public class PlayerController : KinematicObject {
        public AudioClip jumpAudio;
        public AudioClip respawnAudio;
        public AudioClip ouchAudio;
        public AudioClip deathSound;
        public AudioClip ranged;
        public AudioClip melee;
        public AudioClip consume;
        
        private Rigidbody2D _rigidbody2D;
        public TreeEntrance treeEntrance;

        /// <summary>
        /// Max horizontal speed of the player.
        /// </summary>
        public float maxSpeed = 7;

        /// <summary>
        /// Initial jump velocity at the start of a jump.
        /// </summary>
        public float jumpTakeOffSpeed = 7;

        public JumpState jumpState = JumpState.Grounded;

        private bool stopJump;

        /*internal new*/
        public Collider2D collider2d;
        public Collider2D hurtboxCollider;

        /*internal new*/
        public AudioSource audioSource;
        public bool controlEnabled = true;

        public GameObject rangeAttack;
        public GameObject meleeAttack;
        
        public bool consumeIsAvailable = true;
        public float consumeCooldown = 25f;

        public bool fireIsAvailable = true;
        public float fireCooldown = 1f;
        
        public bool fireUpIsAvailable = true;
        public float fireUpCooldown = 1f;
        
        public bool meleeIsAvailable = true;
        public float meleeCooldown = 1f;
        
        public float hp = 100;
        public float maxHp = 100;
        bool jump;
        Vector2 move;
        SpriteRenderer spriteRenderer;
        internal Animator animator;
        readonly PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        public float invinsibilityTimer = 0f;
        public float maxIinvinsibilityTimer = 1f;

        public Slider playerHpIndicator;
        public float meleeOffset = 1;

        public Transform respawnLocation;
        

        void Awake() {
            audioSource = GetComponent<AudioSource>();
            collider2d = GetComponent<Collider2D>();
            _rigidbody2D = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
        }

        protected override void Start() {
            hp = maxHp;
            playerHpIndicator.maxValue = maxHp;
            playerHpIndicator.value = hp;
            base.Start();
        }

        protected override void Update() {
            
            if (treeEntrance.gameOver) return;
            
            if (controlEnabled) {
                move.x = Input.GetAxis("Horizontal");
                if (jumpState == JumpState.Grounded && Input.GetButtonDown("Jump"))
                    jumpState = JumpState.PrepareToJump;
                else if (Input.GetButtonUp("Jump")) {
                    stopJump = true;
                    Schedule<PlayerStopJump>().player = this;
                }
                MeleeCooldown();
                ConsumeCooldown();
                FireCooldown();
                FireUpCooldown();
            }
            else {
                move.x = 0;
            }

            if (velocity.y > 0) {
                collider2d.enabled = false;
                hurtboxCollider.enabled = false;
            }
            else {
                collider2d.enabled = true;
                hurtboxCollider.enabled = true;
            }

            
            UpdateJumpState();
            base.Update();
        }
        private void CC()
        {
            consumeCooldown -= Time.deltaTime;
        }

        private void ConsumeCooldown()
        {
            if (consumeIsAvailable)
            {
                CheckForConsume();   
            }
            else
            {
                CC();
                if(consumeCooldown < 0f)
                {
                    consumeIsAvailable = true;
                    consumeCooldown = 10f;
                }
            }
        }
        private void FC()
        {
            fireCooldown -= Time.deltaTime;
        }  
        private void FUC()
        {
            fireUpCooldown -= Time.deltaTime;
        }

        private void FireCooldown()
        {
            if (fireIsAvailable) {
                CheckForRangeAttack();
            }
            else
            {
                FC();
                if (fireCooldown < 0f)
                {
                    fireIsAvailable = true;
                    fireCooldown = 2f;
                }
            }
        }private void FireUpCooldown()
        {
            if (fireUpIsAvailable) {
                CheckForRangeAttackUp();
            }
            else
            {
                FUC();
                if (fireUpCooldown < 0f)
                {
                    fireUpIsAvailable = true;
                    fireUpCooldown = 2f;
                }
            }
        }
        private void MC()
        {
            meleeCooldown -= Time.deltaTime;
        }

        private void MeleeCooldown()
        {
            if (meleeIsAvailable) {
                CheckForMeleeAttack();
            }
            else
            {
                MC();
                if (meleeCooldown < 0f)
                {
                    meleeIsAvailable = true;
                    meleeCooldown = 2f;
                }
            }
        }
        private void CheckForRangeAttack() {
            if (Input.GetButtonUp("RangeAttack")) {
                
                animator.SetTrigger("ranged");
                audioSource.PlayOneShot(ranged);
                // Debug.Log("rageattackpressed");
                GameObject newRangeAttack = Instantiate(rangeAttack, new Vector3(
                    transform.position.x,
                    transform.position.y,
                    transform.position.z
                ), Quaternion.identity);
                if (spriteRenderer.flipX) {
                    newRangeAttack.GetComponent<RangeAttack>().SetDirection(Vector3.left);
                }
                else {
                    newRangeAttack.GetComponent<RangeAttack>().SetDirection(Vector3.right);
                }
            }
        }
        private void CheckForRangeAttackUp() {
            if (Input.GetButtonUp("RangeAttackUp")) {
                
                animator.SetTrigger("ranged");
                audioSource.PlayOneShot(ranged);
                // Debug.Log("rageattackpressed");
                GameObject newRangeAttack = Instantiate(rangeAttack, new Vector3(
                    transform.position.x,
                    transform.position.y,
                    transform.position.z
                ), Quaternion.identity);
                newRangeAttack.GetComponent<RangeAttack>().SetDirection(Vector3.up);
            }
        }

        private void CheckForMeleeAttack() {
            if (Input.GetButtonUp("MeleeAttack")) {
                animator.SetTrigger("melee");
                audioSource.PlayOneShot(melee);
                Debug.Log("meleeattackpressed");
                float meleeOffsetDirection;
                if (spriteRenderer.flipX) {
                    meleeOffsetDirection = -meleeOffset;
                }
                else {
                    meleeOffsetDirection = meleeOffset;
                }
                GameObject newMeleeAttack = Instantiate(meleeAttack, new Vector3(
                    transform.position.x+ meleeOffsetDirection,
                    transform.position.y,
                    transform.position.z
                ), Quaternion.identity);
                if (spriteRenderer.flipX) {
                    newMeleeAttack.GetComponent<MeleeAttack>().SetDirection(Vector3.left);
                }
                else {
                    newMeleeAttack.GetComponent<MeleeAttack>().SetDirection(Vector3.right);
                }
            }
        }

        private void CheckForConsume() {
            if (Input.GetButtonUp("Consume")) {
                audioSource.PlayOneShot(consume);
                animator.SetTrigger("consume");
                var allEnemies = FindObjectsOfType<EnemyController>();
                int rootedEnemyCount = 0;
                foreach (var enemy in allEnemies) {
                    if (enemy.isFullyRooted && enemy.isConsumble) {
                        rootedEnemyCount++;
                        enemy.GetConsumed();
                    }
                }
                Debug.Log("healing for "+ (MathF.Pow(3,rootedEnemyCount)-1));
                treeEntrance.ReceiveDamage(-MathF.Pow(3,rootedEnemyCount)-1);
            }
        }

        void UpdateJumpState() {
            jump = false;
            switch (jumpState) {
                case JumpState.PrepareToJump:
                    jumpState = JumpState.Jumping;
                    jump = true;
                    stopJump = false;
                    break;
                case JumpState.Jumping:
                    if (!IsGrounded) {
                        Schedule<PlayerJumped>().player = this;
                        jumpState = JumpState.InFlight;
                    }

                    break;
                case JumpState.InFlight:
                    if (IsGrounded) {
                        Schedule<PlayerLanded>().player = this;
                        jumpState = JumpState.Landed;
                    }

                    break;
                case JumpState.Landed:
                    jumpState = JumpState.Grounded;
                    break;
            }
        }

        protected override void FixedUpdate() {
            if (treeEntrance.gameOver) return;
            
            if (invinsibilityTimer > 0) {
                invinsibilityTimer -= Time.deltaTime;
            }

            if (invinsibilityTimer <= 0) {
                invinsibilityTimer = 0;
                //spriteRenderer.color = Color.white;
                spriteRenderer.color = new Color(
                    spriteRenderer.color.r,
                    spriteRenderer.color.g,
                    spriteRenderer.color.b,
                    1f);
            }

            base.FixedUpdate();
        }

        protected override void ComputeVelocity() {
            if (jump && IsGrounded) {
                velocity.y = jumpTakeOffSpeed * model.jumpModifier;
                jump = false;
            }
            else if (stopJump) {
                stopJump = false;
                if (velocity.y > 0) {
                    velocity.y = velocity.y * model.jumpDeceleration;
                }
            }

            if (move.x > 0.01f)
                spriteRenderer.flipX = false;
            else if (move.x < -0.01f)
                spriteRenderer.flipX = true;

            animator.SetBool("grounded", IsGrounded);
            animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);

            targetVelocity = move * maxSpeed;
        }

        public enum JumpState {
            Grounded,
            PrepareToJump,
            Jumping,
            InFlight,
            Landed
        }

        public void ReceiveDamage(float playerDamage) {
            if(treeEntrance.gameOver) return;
            audioSource.PlayOneShot(ouchAudio);
            Debug.Log("receiven damage" + playerDamage);
            if (invinsibilityTimer > 0) {
                return;
            }

            hp -= playerDamage;
            playerHpIndicator.value = hp;
            if (hp <= 0) {
                hp = 0;
                Die();
            }
            else {
                //become invinsible
                invinsibilityTimer = maxIinvinsibilityTimer;
                spriteRenderer.color = new Color(
                    spriteRenderer.color.r,
                    spriteRenderer.color.g,
                    spriteRenderer.color.b,
                    .5f);
            }
        }


        private void Die() {
            treeEntrance.music.PlayOneShot(deathSound);
            treeEntrance.OnPlayerDeath();
        }

        public void Respawn() {
            treeEntrance.music.PlayOneShot(respawnAudio);
            transform.position = respawnLocation.transform.position;           
            Start();
        }
    }
}