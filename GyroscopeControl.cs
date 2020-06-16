using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GyroscopeControl : MonoBehaviour
{
    // STATE
    private Transform rawGyroRotation;
    private Quaternion initialRotation; 
    private Quaternion gyroInitialRotation;

    public bool GyroEnabled { get; set; }

    // SETTINGS
    [SerializeField] private float smoothing = 0.1f;
    [SerializeField] private float speed = 60.0f;

    private void Awake() {
        Input.gyro.enabled = true;
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
        GyroEnabled = true;
        
        /* Get object and gyroscope initial rotation for calibration */
        initialRotation = transform.rotation; 
        recalibrate();

        /* GameObject instance used to prepare object movement */
        rawGyroRotation = new GameObject("GyroRaw").transform;
        rawGyroRotation.position = transform.position;
        rawGyroRotation.rotation = transform.rotation;
    }

    private void Update()
    {
        if (Time.timeScale == 1 && GyroEnabled)
        {
            ApplyGyroRotation(); // Get rotation state in rawGyroRotation
            Quaternion offsetRotation = Quaternion.Inverse(gyroInitialRotation) * rawGyroRotation.rotation; // Apply initial offset for calibration

            transform.rotation = Quaternion.Slerp(transform.rotation, initialRotation * offsetRotation, smoothing); // Progressive rotation of the object
        }
    }

    private void ApplyGyroRotation()
    {
        float curSpeed = Time.deltaTime * speed;
        Quaternion tempGyroRotation = new Quaternion(
            -Input.gyro.attitude.x * curSpeed, 
            0.0f, 
            -Input.gyro.attitude.y * curSpeed, 
            Input.gyro.attitude.w * curSpeed);
        rawGyroRotation.rotation = tempGyroRotation;
    }

    public void setSpeed(float speed){
        this.speed = speed;
    }

    public float getSpeed(){
        return speed;
    } 

    /* Used for calibrate gyro at start or during execution using UI button for exemple */
    public void recalibrate(){
        gyroInitialRotation.x = -Input.gyro.attitude.x;
        gyroInitialRotation.y = 0.0f; // Fixed Y axis
        gyroInitialRotation.z = -Input.gyro.attitude.y; // We rotate object on Y with Z axis gyro
        gyroInitialRotation.w = Input.gyro.attitude.w;
        print("Successfully recalibrated !");
    }
}