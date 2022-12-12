using UnityEngine;

public class MovementInput
{
    private string horizontal = "Horizontal";
    private string vertical = "Vertical";
    private KeyCode runningKey = KeyCode.LeftControl;
    private Vector2 movement = Vector2.zero;
    private bool runningKeyPressed = false;

    public MovementInput() {
        horizontal = "Horizontal";
        vertical = "Vertical";
        runningKey = KeyCode.LeftControl;
        movement = Vector2.zero;
        runningKeyPressed = false;
    }

    public void updateInputs()
    {
        movement.x = Input.GetAxisRaw(horizontal);
        movement.y = Input.GetAxisRaw(vertical);

        if (Input.GetKey(runningKey)) runningKeyPressed = true;
        else runningKeyPressed = false;
    }

    public void resetMoveAxis()
    {
        movement = Vector2.zero;
    }

    public Vector2 getMovement() {
        return movement;
    }

    public void setRunningKey(KeyCode newKey)
    {
        runningKey = newKey;
    }
    
    
    public bool isRunningKeyPressed() {
        return runningKeyPressed;
    }
}
