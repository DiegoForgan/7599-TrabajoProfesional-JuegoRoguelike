using UnityEngine;

public class Entity : MonoBehaviour, Collidable
{
    public int maxHealth = 100;
    public int health;
    
    public HealthBar healthBar;
   
    private void Start() {
        initializeHealth();
    }

    public void initializeHealth(){
        health = maxHealth;
        healthBar.initialize(health);
    }

    public int GetHealth(){
        return health;
    }
    
    //Implementing "Collidable" interface method
    public virtual void TakeDamage(int damage){
         health -= damage;
         healthBar.SetHealth(health);
         //checks if entity is dead
         if(health <= 0) DestroyElement();
     }

    public virtual void DestroyElement()
    {
        Destroy(gameObject);
    }

}
