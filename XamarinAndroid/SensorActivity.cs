using System;
using System.Collections.Generic;

using Android.App;
using Android.OS;
using Android.Widget;
using Android.Hardware;

namespace XamarinAndroid
{
    [Activity(Label = "SensorActivity")]
    public class SensorActivity : Activity
    {
        private ListView mSensorListView;
        private ArrayAdapter mSensorListViewAdapter;
        private SensorManager mSensorManager;
        private List<SensorType> mSensorTypes;
        private SensorController mSensorController;
        private Sensor mSelectedSensor;

        private TextView mSensorTitleTextView;
        private TextView mSensorDataTextView;
        private ProgressBar mSensorProgressBar;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SensorActivity);

            mSensorTypes = new List<SensorType>();

            mSensorListView = FindViewById<ListView>(Resource.Id.sensorListView);
            var deviceHeight = Resources.DisplayMetrics.HeightPixels;
            mSensorListView.LayoutParameters.Height = (int)((deviceHeight / Resources.DisplayMetrics.Density)/0.75);

            mSensorListViewAdapter = new ArrayAdapter(
                this,
                Resource.Layout.SensorListItem,
                Resource.Id.sensorListItemTextView,
                new List<string>());
            mSensorListView.Adapter = mSensorListViewAdapter;

            mSensorManager = (SensorManager)GetSystemService(Service.SensorService);
            foreach(Sensor s in mSensorManager.GetSensorList(SensorType.All))
            {
                mSensorListViewAdapter.Add(s.Name);
                mSensorTypes.Add(s.Type);
                mSensorListViewAdapter.NotifyDataSetChanged();
            }
            mSensorListView.ItemClick += SensorListView_ItemClick;

            mSensorTitleTextView = FindViewById<TextView>(Resource.Id.sensorTitleTextView);
            mSensorDataTextView = FindViewById<TextView>(Resource.Id.sensorTextView);
            mSensorProgressBar = FindViewById<ProgressBar>(Resource.Id.sensorProgressBar);
            mSensorController = new SensorController(this, mSensorDataTextView);
            mSensorTitleTextView.Text = "No Sensor Selected";
            mSensorDataTextView.Text = "";
            mSensorProgressBar.Progress = 0;
        }

        protected override void OnPause()
        {
            base.OnPause();
            mSensorManager.UnregisterListener(mSensorController, mSelectedSensor);
        }

        private void SensorListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            string selectedSensorName = (string) mSensorListViewAdapter.GetItem(e.Position);
            Toast toast = Toast.MakeText(this, selectedSensorName, ToastLength.Short);
            toast.Show();
            if (mSelectedSensor != null) mSensorManager.UnregisterListener(mSensorController, mSelectedSensor);
            mSelectedSensor = mSensorManager.GetDefaultSensor(mSensorTypes[e.Position]);
            mSensorTitleTextView.Text = selectedSensorName;
            string dataViewString = "Reporting Mode: " + mSelectedSensor.ReportingMode.ToString() +"\n";
            if (mSensorDataTextView == null) mSensorDataTextView = FindViewById<TextView>(Resource.Id.sensorTextView);
            if (mSensorController == null) mSensorController = new SensorController(this, mSensorDataTextView);
            mSensorManager.RegisterListener(mSensorController, mSelectedSensor, SensorDelay.Normal);
        }
    }
}