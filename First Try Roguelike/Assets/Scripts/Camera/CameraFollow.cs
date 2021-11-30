using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0,0,-10);
    
    private void Start() {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        SetTarget(player.transform);
    }  
    private void FixedUpdate() 
    {  
        if(target) transform.position = target.position + offset;
        //if target == null means main character is dead so camera fixes on center of screen
        else transform.position = new Vector3(0,0,-10);
    }
    
    public void SetTarget(Transform newTarget){
        target = newTarget;
    }
}
