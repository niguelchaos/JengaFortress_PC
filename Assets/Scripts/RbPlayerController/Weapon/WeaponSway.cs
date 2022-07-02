using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{

    [SerializeField] private float smooth;
    [SerializeField] private float swayMultiplier;
    [SerializeField] private Quaternion rotationX;
    [SerializeField] private Quaternion rotationY;
    [SerializeField] private Quaternion targetRotation;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Sway();
    }

    private void Sway()
    {
        Vector2 swayAmount = InputManager.Instance.lookInput * swayMultiplier;

        // calculate target rotation - return rotation amount
        rotationX = Quaternion.AngleAxis(-InputManager.Instance.lookInput.y, Vector3.right);
        rotationY = Quaternion.AngleAxis(InputManager.Instance.lookInput.x, Vector3.up);
        
        rotationX.x = Mathf.Clamp(rotationX.x, -0.1f, 0.1f);
        rotationY.y = Mathf.Clamp(rotationY.y, -0.1f, 0.1f);

        targetRotation = rotationX * rotationY;

        // rotate the thing, smoothlerping it
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);
    }


}
