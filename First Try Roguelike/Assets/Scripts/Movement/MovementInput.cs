using UnityEngine;

public class MovementInput
{
    private string horizontal;
    private string vertical;
    private KeyCode runningKey;
    private Vector2 movementAxis;
    private bool runningKeyPressed;

    public MovementInput() {
        horizontal = "Horizontal";
        vertical = "Vertical";
        runningKey = KeyCode.LeftShift;
        movementAxis = Vector2.zero;
        runningKeyPressed = false;
    }

    public void updateInputs()
    {
        movementAxis.x = Input.GetAxisRaw(horizontal);
        movementAxis.y = Input.GetAxisRaw(vertical);
        runningKeyPressed = Input.GetKey(runningKey);
        Debug.Log(movementAxis);
    }

    public void resetMoveAxis()
    {
        movementAxis = Vector2.zero;
    }

    public Vector2 getMovementAxis() {
        return movementAxis;
    }

    public void setRunningKey(KeyCode newKey)
    {
        runningKey = newKey;
    }
    
    
    public bool isRunningKeyPressed() {
        return runningKeyPressed;
    }
}
