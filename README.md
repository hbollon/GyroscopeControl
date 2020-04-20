# Gyroscope control script
Unity script used for object rotation on x and z axis using x and y axis of gyro (can be easily edited). It included initial calibration with offset, rotation speed ( Time.deltaTime * velocity ) and smoothing parameters editable in Unity inspector.

## Settings
This script contain 2 parameters editable through Unity inspector :
* Speed : change rotation speed of the object (cf below)
* Smoothing : must be between 0 and 1
