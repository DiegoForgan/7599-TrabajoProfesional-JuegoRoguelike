using UnityEditor;
using UnityEngine;

public class EnemyMovement : EntityMovement
{
    private GameObject target;
    private CharactersAnimator movementAnimator;
    private bool isVisible;
    private Enemy _enemy;
    //private HealthBar _healthBar;
    


    private float offset_y;
  
    private void OnBecameVisible() {
        Debug.Log("Enemy Visible!");
        isVisible = true;
    }

    private void OnBecameInvisible() {
        isVisible = false;
    }

    private void Awake() {
        _rigidBody = GetComponent<Rigidbody2D>();
        _enemy = GetComponent<Enemy>();
    }

    private void Start() {
        //_healthBar = _enemy.GetHealthBar();
        //offset_y = (Vector2.Distance(_rigidBody.position,_healthBar.transform.position));
        target  = GameObject.FindGameObjectWithTag("Player");
        if (!target) Debug.LogError("No target aquired!");
        movementAnimator = GetComponent<CharactersAnimator>();
        movementAnimator.ResetRigs();
        _attackPoint = transform.Find("AttackPoint");
        attackPointPositions = new Vector2[] { new Vector2(0f, -0.2f), new Vector2(0f, 2f), new Vector2(0.8f, 0.5f), new Vector2(-0.8f, 0.5f) };
        _attackPoint.localPosition = attackPointPositions[0];
    }

    private bool CanMove(){
        return target;// (isVisible && target);
    }

    private Vector2 GetMovementValues(){
        Vector2 movement = Vector2.zero;
        Vector2 distanceVector = target.transform.position - transform.position;
        movement.x = (distanceVector.x < 0) ? -1 : 1;
        movement.y = (distanceVector.y < 0) ? -1 : 1;
        Debug.Log(movement);
        return movement;        
    }

    private bool IsInAttackDistanceAndAbleToAttack(){
        //Checks if player is at the distance required for enemy to attack
        /*float playerEnemyDistance = Vector2.Distance(transform.position,currentTargetPosition);
        if ((playerEnemyDistance <= attackDistance) && (Time.time >= nextAttackTime)) return true;
        */return false;
    }

  
  private void Update() 
  {
        //It will only start moving towards the player if you faced the enemy on screen
        movement = CanMove() ? GetMovementValues() : Vector2.zero;
        /*if(CanMove()){
        GetMovementValues();
        //Checks if player is at the distance required for enemy to attack
        if (IsInAttackDistanceAndAbleToAttack()){
          _enemy.Attack();
          nextAttackTime = Time.time + 1f/attackRate;
        }
      }
      else movement = Vector2.zero;*/
  }   

    private void FixedUpdate() 
    {
      if(CanMove()){
            //Move the enemy
            Vector2 newPosition = _rigidBody.position + movement * movementSpeed * Time.fixedDeltaTime;
            _rigidBody.MovePosition(newPosition);
            Vector2 direction = GetMovementDirection(movement);
            moveAttackPointToDirection(direction);
            movementAnimator.HandleMovementAnimation(direction);
            //Update the location of the enemy healthbar on screen
            //UpdateHealthbarPosition();
        }
    }

  private void UpdateHealthbarPosition(){
    //1 - We get the angle that the enemy is facing the player
    float angleFacingPlayer = transform.rotation.eulerAngles.z;
    //2 - Separation beetween the healthbar and the entity
    float offset = offset_y;
    //3 - We check if the health bar must be placed above or below the enemy    
    if(!(angleFacingPlayer > 90 && angleFacingPlayer < 270)) offset = offset_y * -1;
    //4 - Update the health bar accordingly to the enemy movement and previous calculations
    //_healthBar.SetPosition(new Vector2(_rigidBody.position.x,_rigidBody.position.y+offset));
   }

}
