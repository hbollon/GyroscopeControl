<h1 align="center">GyroscopeControl : Unity script for object rotation on mobile devices with gyroscope</h1>
Unity script used for smooth and customizable object rotation with gyroscope (initially configured to rotate x and z axis using x and y axis of gyro but can be easily edited). It includes initial calibration with offset, rotation speed (Time.deltaTime * velocity), smoothing parameter editable in Unity inspector and debug overlay.

This script has been designed to be easily customizable and to obtain the smoothest and most flexible rotation possible.
Tested on Android.

## Edit rotation axis
In order to personalize this script to match with your case, you just have to change which gyroscope axis control which object one.
To do that, you have to edit the ApplyGyroRotation function :

```C#
Quaternion tempGyroRotation = new Quaternion(
    offsetRotation.x * curSpeed, 
    0f * curSpeed, 
    offsetRotation.y * curSpeed, 
    offsetRotation.w * curSpeed
);
```
## Settings
This script contain 3 parameters editable through Unity inspector :
* Speed : change rotation speed of the object
* Smoothing : must be between 0 and 1, it changes the delay for the object in order to reach its final position
* Wait Gyro Initialization : used to enable or disable the initial delay to wait gyroscope activation
* Wait Gyro Initialization Duration : duration of the initial delay in seconds
* Smoothing : must be between 0 and 1, it changes the delay for the object in order to reach its final position
* Debug : displays an overlay which includes a lot of information such as: real-time gyro attitude, offset, initial position ect...

## 🤝 Contributing

Contributions, issues and feature requests are welcome!<br />Feel free to check [issues page](https://github.com/hbollon/GyroscopeControl/issues). 

## Show your support

Give a ⭐️ if this project helped you!
