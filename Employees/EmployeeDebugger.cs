using UnityEngine;
using UnityEngine.InputSystem; // Required for New Input

public class EmployeeDebugger : MonoBehaviour
{
    public EmployeeController targetEmployee;
    public Transform deskLocation;
    public Transform waterCoolerLocation;
     public Transform chairLocation;

    void Update()
    {
        // New Input System check for Key Press
        // Key.Digit1 corresponds to Alpha1

        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            Debug.Log("Debugger: Sending John to Desk");
            targetEmployee.GoToLocation(deskLocation.position);
            targetEmployee.DoWorkEffect(3.0f);
        }

        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            Debug.Log("Debugger: Sending John to Water Cooler");
            targetEmployee.GoToLocation(waterCoolerLocation.position);
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Debug.Log("Debugger: SCARING JOHN");
            targetEmployee.GetScaredEffect(2.0f);
        }

          if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            Debug.Log("Debugger: Telling John to sit");
            targetEmployee.SitOnChair(chairLocation);
        }

        // Press 4 to Stand Up
        if (Keyboard.current.digit4Key.wasPressedThisFrame)
        {
            Debug.Log("Debugger: Telling John to stand up");
            targetEmployee.StandUpFromChair();
        }
    }
}
