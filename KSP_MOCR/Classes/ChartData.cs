﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace KSP_MOCR
{
	public partial class Form1 : Form
	{
		public Dictionary<String, Dictionary<int, Nullable<double>>> chartData = new Dictionary<String, Dictionary<int, Nullable<double>>>();

		private StreamCollection graphStreams;

		public void setupChartData()
		{
			graphStreams = new StreamCollection(connection);

			chartData.Add("altitudeTime", new Dictionary<int, double?>());
			for (int i = 0; i < 600; i++) chartData["altitudeTime"].Add(i, null);

			chartData.Add("apoapsisTime", new Dictionary<int, double?>());
			for (int i = 0; i < 600; i++) chartData["apoapsisTime"].Add(i, null);

			chartData.Add("periapsisTime", new Dictionary<int, double?>());
			for (int i = 0; i < 600; i++) chartData["periapsisTime"].Add(i, null);

			chartData.Add("geeTime", new Dictionary<int, double?>());
			for (int i = 0; i < 600; i++) chartData["geeTime"].Add(i, null);

			chartData.Add("terrainTime", new Dictionary<int, double?>());
			for (int i = 0; i < 600; i++) chartData["terrainTime"].Add(i, null);

			chartData.Add("dynPresTime", new Dictionary<int, double?>());
			for (int i = 0; i < 600; i++) chartData["dynPresTime"].Add(i, null);

			chartData.Add("altitudeSpeed", new Dictionary<int, double?>());
			for (int i = 0; i< 3000; i++) chartData["altitudeSpeed"].Add(i, null);
		}

		public void updateChartData(object sender, EventArgs e)
		{
			if (connected && krpc.CurrentGameScene == KRPC.Client.Services.KRPC.GameScene.Flight && graphStreams != null)
			{
				double altitude = graphStreams.GetData(DataType.flight_meanAltitude);
				double apoapsis = graphStreams.GetData(DataType.orbit_apoapsisAltitude);
				double periapsis = graphStreams.GetData(DataType.orbit_periapsisAltitude);
				double speed = graphStreams.GetData(DataType.orbit_speed);
				double MET = graphStreams.GetData(DataType.vessel_MET);
				double elevation = graphStreams.GetData(DataType.flight_elevation);
				float gee = graphStreams.GetData(DataType.flight_gForce);
				float dynPress = graphStreams.GetData(DataType.flight_dynamicPressure);


				if (MET > 600)
				{
					for (int i = 1; i < 600; i++)
					{
						chartData["altitudeTime"][i - 1] = chartData["altitudeTime"][i];
						chartData["apoapsisTime"][i - 1] = chartData["apoapsisTime"][i];
						chartData["periapsisTime"][i - 1] = chartData["periapsisTime"][i];
						chartData["geeTime"][i - 1] = chartData["geeTime"][i];
						chartData["terrainTime"][i - 1] = chartData["terrainTime"][i];
						chartData["dynPresTime"][i - 1] = chartData["dynPresTime"][i];
					}
					chartData["altitudeTime"][599] = altitude;
					chartData["apoapsisTime"][599] = apoapsis;
					chartData["periapsisTime"][599] = periapsis;
					chartData["geeTime"][599] = gee;
					chartData["terrainTime"][599] = elevation;
					chartData["dynPresTime"][599] = dynPress;
				}
				else
				{
					chartData["altitudeTime"][(int)MET] = altitude;
					chartData["apoapsisTime"][(int)MET] = apoapsis;
					chartData["periapsisTime"][(int)MET] = periapsis;
					chartData["geeTime"][(int)MET] = gee;
					chartData["terrainTime"][(int)MET] = elevation;
					chartData["dynPresTime"][(int)MET] = dynPress;
				}

				if (speed < 3000)
				{
					chartData["altitudeSpeed"][(int)speed] = altitude;
				}
			}
		}
	}
}
