using UnityEngine;

public class ClawRotateSystem : MonoBehaviour
{
    [SerializeField] private float minRotation, maxRotation;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private bool isRotating, isMoveRight;
    [SerializeField] private Transform hookObject;
    float rotateAngle;
    public bool canRotate = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (canRotate) Rotate();
    }

    private void Rotate()
    {
        if (!isRotating)
        {
            if (isMoveRight)
            {
                rotateAngle += rotationSpeed * Time.deltaTime;
                if (rotateAngle >= maxRotation)
                {
                    rotateAngle = maxRotation;
                    isMoveRight = false;
                }
            }
            else
            {
                rotateAngle -= rotationSpeed * Time.deltaTime;
                if (rotateAngle <= minRotation)
                {
                    rotateAngle = minRotation;
                    isMoveRight = true;
                }
            }
        }

        hookObject.rotation = Quaternion.Euler(0, 0, rotateAngle);
    }
}
