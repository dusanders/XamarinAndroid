using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Net;

using Android.App;
using Android.OS;
using Android.Widget;
using System.IO;

namespace XamarinAndroid
{
    [Activity(Label = "WeatherActivity")]
    class WeatherActivity : Activity
    {
        private Spinner mStateSpinner;
        private Spinner mStationSpinner;
        private TextView mWeatherTextView;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.WeatherActivity);
            mStateSpinner = FindViewById<Spinner>(Resource.Id.stateSpinner);
            mStationSpinner = FindViewById<Spinner>(Resource.Id.stationSpinner);
            mWeatherTextView = FindViewById<TextView>(Resource.Id.weatherTextView);
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri("http://w1.weather.gov/xml/current_obs/index.xml"));
            request.Method = "GET";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            request.UserAgent = "Chrome/48.0.2564.116";
            XDocument xDocument;
            using (WebResponse resp = request.GetResponse())
            {
                using(StreamReader reader = new StreamReader(resp.GetResponseStream()))
                {
                    xDocument = XDocument.Load(reader);
                }
            }
            List<XElement> xStateList = xDocument.Descendants().Where(x => x.Name == "state").ToList();
            List<string> stateList = new List<string>();
            stateList.AddRange(xStateList.Where(node=>!stateList.Contains(node.Value)).Select(x=>x.Value).Distinct().OrderBy(name=>name));
            mStateSpinner.ItemSelected += (stateClickObject, stateClickEvent) => 
            {
                mWeatherTextView.Text = "";
                List<XElement> xStationList = xDocument.Descendants()
                    .Where(x => x.Name == "state" && 
                            x.Value == stateList[stateClickEvent.Position])
                    .ToList();
                List<string> stationList = new List<string>();
                List<string> callNumberList = new List<string>();
                foreach(XElement e in xStationList)
                {
                    stationList.AddRange(e.ElementsAfterSelf("station_name").Select(x=>x.Value));
                    callNumberList.AddRange(e.ElementsBeforeSelf("station_id").Select(x => x.Value));
                }
                mStationSpinner.ItemSelected += (stationClickObject, stationClickEvent) =>
                {
                    mWeatherTextView.Text = "";
                    if (stateClickEvent.Position < callNumberList.Count)
                    {
                        HttpWebRequest weatherRequest = (HttpWebRequest)HttpWebRequest.Create(
                            new Uri(String.Format("http://w1.weather.gov/xml/current_obs/{0}.xml", callNumberList[stationClickEvent.Position])));
                        weatherRequest.Method = "GET";
                        weatherRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
                        weatherRequest.UserAgent = "Chrome/48.0.2564.116";
                        XDocument weatherDocument;
                        using (WebResponse response = weatherRequest.GetResponse())
                        {
                            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                            {
                                weatherDocument = XDocument.Load(reader);
                            }
                        }
                        String viewText = "";
                        viewText += "Location: " + weatherDocument.Descendants().FirstOrDefault(x=>x.Name == "location").Value + "\n";
                        viewText += weatherDocument.Descendants().FirstOrDefault(x => x.Name == "observation_time").Value + "\n";
                        viewText += "Conditions: " + weatherDocument.Descendants().FirstOrDefault(x => x.Name == "weather").Value + "\n";
                        viewText += "Temperature: " + weatherDocument.Descendants().FirstOrDefault(x => x.Name == "temperature_string").Value + "\n";
                        viewText += "Relative Humidity: " + weatherDocument.Descendants().FirstOrDefault(x => x.Name == "relative_humidity").Value + "\n";
                        viewText += "Wind: " + weatherDocument.Descendants().FirstOrDefault(x => x.Name == "wind_string").Value + "\n";
                        viewText += "Dewpoint: " + weatherDocument.Descendants().FirstOrDefault(x => x.Name == "dewpoint_string").Value + "\n";
                        mWeatherTextView.Text = viewText;
                    }
                };
                mStationSpinner.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, stationList);
            };
            mStateSpinner.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, stateList);
        }
    }
}