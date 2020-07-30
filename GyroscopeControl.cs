using UnityEngine;
using System.Collections;

public class GyroscopeControl : MonoBehaviour
{
    // STATE
    private Transform rawGyroRotation;
    private Quaternion initialRotation; 
    private Quaternion gyroInitialRotation;
    private Quaternion offsetRotation;

    public bool GyroEnabled { get; set; }
    private bool gyroInitialized = false;

    // SETTINGS
    [SerializeField] private float smoothing = 0.1f;
    [SerializeField] private float speed = 60.0f;
    [SerializeField] private bool waitGyroInitialization = true; 
    [SerializeField] private float waitGyroInitializationDuration = 1f; 

    public bool debug;

    private void InitGyro() {
        if(!gyroInitialized){
            Input.gyro.enabled = true;
            Input.gyro.updateInterval = 0.0167f;
        }
        gyroInitialized = true;
    }

    private void Awake() {
        if(waitGyroInitialization && waitGyroInitializationDuration < 0f){
            waitGyroInitializationDuration = 1f;
            throw new System.ArgumentException("waitGyroInitializationDuration can't be negative, it was set to 1 second");
        }
    }

    private IEnumerator Start()
    {
        if(HasGyro()){
            InitGyro();
            GyroEnabled = true;
        } else GyroEnabled = false;

        if(waitGyroInitialization)
            yield return new WaitForSeconds(waitGyroInitializationDuration);
        else
            yield return null;
        
        /* Get object and gyroscope initial rotation for calibration */
        initialRotation = transform.rotation; 
        Recalibrate();

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

            transform.rotation = Quaternion.Slerp(transform.rotation, initialRotation * rawGyroRotation.rotation, smoothing); // Progressive rotation of the object
        }
    }

    private void ApplyGyroRotation()
    {
        // Apply initial offset for calibration
        offsetRotation = Quaternion.Inverse(gyroInitialRotation) * GyroToUnity(Input.gyro.attitude);

        float curSpeed = Time.deltaTime * speed;
        Quaternion tempGyroRotation = new Quaternion(
            offsetRotation.x * curSpeed, 
            0f * curSpeed, 
            offsetRotation.y * curSpeed, 
            offsetRotation.w * curSpeed
        );
        rawGyroRotation.rotation = tempGyroRotation;
    }

    private Quaternion GyroToUnity(Quaternion gyro){
        return new Quaternion(gyro.x, gyro.y, -gyro.z, -gyro.w);
    }

    private bool HasGyro(){
        return SystemInfo.supportsGyroscope;
    }

    public void setSpeed(float speed){
        this.speed = speed;
    }

    public float getSpeed(){
        return speed;
    } 

    /* Used for calibrate gyro at start or during execution using UI button for exemple */
    public void Recalibrate(){
        Quaternion gyro = GyroToUnity(Input.gyro.attitude);
        gyroInitialRotation.x = gyro.x;
        gyroInitialRotation.y = gyro.y; // Fixed Y axis
        gyroInitialRotation.z = gyro.z; // We rotate object on Y with Z axis gyro
        gyroInitialRotation.w = gyro.w;
        print("Successfully recalibrated !");
    }

    void OnGUI () {
        if(debug){
            GUIStyle style = new GUIStyle();
            style.fontSize = Mathf.RoundToInt(Mathf.Min(Screen.width, Screen.height) / 20f);
            style.normal.textColor = Color.white;
            GUILayout.BeginVertical("box");
            GUILayout.Label("Attitude: " + Input.gyro.attitude.ToString(), style);
            GUILayout.Label("Rotation: " + transform.rotation.ToString(), style);
            GUILayout.Label("Offset Rotation: " + offsetRotation.ToString(), style);
            GUILayout.Label("Raw Rotation: " + rawGyroRotation.rotation.ToString(), style);
            GUILayout.EndVertical();
        }
    }
}