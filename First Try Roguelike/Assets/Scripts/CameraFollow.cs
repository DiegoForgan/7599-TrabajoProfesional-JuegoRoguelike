using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0,0,-10);
    
    
    private void FixedUpdate() 
    {  
        if(target) transform.position = target.position + offset;
        //if target == null means main character is dead so camera fixes on center of screen
        else transform.position = new Vector3(0,0,-10);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
