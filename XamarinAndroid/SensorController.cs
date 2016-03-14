/*
** SENSOR VALUES
    Sensor.TYPE_ACCELEROMETER: All values are in SI units (m/s^2)
values[0]: Acceleration minus Gx on the x-axis
values[1]: Acceleration minus Gy on the y-axis
values[2]: Acceleration minus Gz on the z-axis

Sensor.TYPE_LIGHT:
values[0]: Ambient light level in SI lux units

Sensor.TYPE_PRESSURE:
values[0]: Atmospheric pressure in hPa (millibar)

Sensor.TYPE_PROXIMITY:
values[0]: Proximity sensor distance measured in centimeters

Sensor.TYPE_GRAVITY:

Sensor.TYPE_LINEAR_ACCELERATION: 
A three dimensional vector indicating acceleration along 
each device axis, not including gravity. 
All values have units of m/s^2. The coordinate system is the 
same as is used by the acceleration sensor.

Sensor.TYPE_ROTATION_VECTOR:
Typically the output of the gyroscope is integrated over 
time to calculate a rotation describing the change of angles 
over the timestep, for example:
In practice, the gyroscope noise and offset will introduce 
some errors which need to be compensated for. This is usually 
done using the information from other sensors, but is beyond 
the scope of this document.
Elements of the rotation vector are unitless. The x,y, and z axis 
are defined in the same way as the acceleration sensor.
The reference coordinate system is defined as a direct orthonormal 
basis, where:
X is defined as the vector product Y.Z (It is tangential to the ground at the device's current location and roughly points East).
Y is tangential to the ground at the device's current location and points towards magnetic north.
Z points towards the sky and is perpendicular to the ground.
values[0]: x*sin(&#952/2)
values[1]: y*sin(&#952/2)
values[2]: z*sin(&#952/2)
values[3]: cos(&#952/2)
values[4]: estimated heading Accuracy (in radians) (-1 if unavailable)

Sensor.TYPE_ORIENTATION: All values are angles in degrees.
values[0]: Azimuth, angle between the magnetic north direction and the y-axis, around the z-axis (0 to 359). 0=North, 90=East, 180=South, 270=West

Sensor.TYPE_RELATIVE_HUMIDITY:
values[0]: Relative ambient air humidity in percent

*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Hardware;

namespace XamarinAndroid
{
    class SensorController : Java.Lang.Object, ISensorEventListener
    {
        private Activity mActivity;
        private TextView mTextView;

        public SensorController(Activity activity, TextView textView)
        {
            mActivity = activity;
            mTextView = textView;
        }

        public void OnAccuracyChanged(Sensor sensor, [GeneratedEnum] SensorStatus accuracy)
        {
        }

        public void OnSensorChanged(SensorEvent e)
        {
            mActivity.RunOnUiThread(delegate
            {
                mTextView.Text = "";
                try
                {
                    mTextView.Text = "X: " + e.Values[0] + "\n";
                    mTextView.Text += "Y: " + e.Values[1] + "\n";
                    mTextView.Text += "Z: " + e.Values[2] + "\n";
                }
                catch (ArgumentOutOfRangeException ex) { }
            });
        }
    }
}