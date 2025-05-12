using UnityEngine;

public class gunHolder : MonoBehaviour
{
    private void Update()
    {
        Vector3 cameraEuler = Camera.main.transform.eulerAngles;
    // Get current parent Y rotation
    float parentX = transform.parent != null ? transform.parent.eulerAngles.x : 0f;
    // Calculate local Y offset to match camera
    float relativeX = cameraEuler.x - parentX;

    // Set local rotation to match only the Y component
    transform.localRotation = Quaternion.Euler(relativeX, 0f, 0f);
    }
}
