using UnityEngine;

public class GyroscopeControl : MonoBehaviour
{
    // STATE
    private Transform rawGyroRotation;
    private Quaternion initialRotation; 
    private Quaternion gyroInitialRotation;

    public bool GyroEnabled { get; set; }
    private bool gyroInitialized = false;

    // SETTINGS
    [SerializeField] private float smoothing = 0.1f;
    [SerializeField] private float speed = 60.0f;

    public bool debug;

    private void InitGyro() {
        if(!gyroInitialized){
            Input.gyro.enabled = true;
            Input.gyro.updateInterval = 0.0167f;
        }
        gyroInitialized = true;
    }

    private void Start()
    {
        if(HasGyro()){
            InitGyro();
            GyroEnabled = true;
        } else GyroEnabled = false;
        
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
            Input.gyro.attitude.w * curSpeed
        );
        rawGyroRotation.rotation = tempGyroRotation;
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
        gyroInitialRotation.x = -Input.gyro.attitude.x;
        gyroInitialRotation.y = 0.0f; // Fixed Y axis
        gyroInitialRotation.z = -Input.gyro.attitude.y; // We rotate object on Y with Z axis gyro
        gyroInitialRotation.w = Input.gyro.attitude.w;
        print("Successfully recalibrated !");
    }

    void OnGUI () {
        if(debug){
            GUIStyle style = new GUIStyle();
            style.fontSize = Mathf.RoundToInt(Mathf.Min(Screen.width, Screen.height) / 20f);
            style.normal.textColor = Color.white;
            GUILayout.BeginVertical("box");
            GUILayout.Label("Attitude: " + Input.gyro.attitude.ToString(), style);
            GUILayout.Label("Rotation:: " + transform.rotation.ToString(), style);
            GUILayout.Label("Initial Rotation: " + initialRotation.ToString(), style);
            GUILayout.Label("Raw Rotation: " + rawGyroRotation.rotation.ToString(), style);
            GUILayout.EndVertical();
        }
    }
}