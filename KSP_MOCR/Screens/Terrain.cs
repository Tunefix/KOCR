﻿using KRPC.Client;
using KRPC.Client.Services.KRPC;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSP_MOCR
{
	class Terrain : MocrScreen
	{

		public Terrain(Screen form)
		{
			this.form = form;
			this.chartData = form.form.chartData;

			this.width = 120;
			this.height = 30;
		}

		public override void updateLocalElements(object sender, EventArgs e)
		{

			// Re-usable data variable for graph data
			List<List<KeyValuePair<double, double?>>> data = new List<List<KeyValuePair<double, double?>>>();
			List<Plot.Type> types = new List<Plot.Type>();

			if (form.form.connected && form.form.krpc.CurrentGameScene == GameScene.Flight)
			{
				data = new List<List<KeyValuePair<double, double?>>>();
				types = new List<Plot.Type>();
				data.Add(chartData["altitudeTime"]);
				types.Add(Plot.Type.CROSS);
				data.Add(chartData["terrainTime"]);
				types.Add(Plot.Type.LINE);
				screenCharts[0].setData(data, types, false);
			}
		}

		public override void makeElements()
		{
			for (int i = 0; i < 1; i++) screenCharts.Add(null); // Initialize Charts
			for (int i = 0; i < 3; i++) screenLabels.Add(null); // Initialize Labels
			for (int i = 0; i< 1; i++) screenInputs.Add(null); // Initialize Inputs

			screenInputs[0] = Helper.CreateInput(-2, -2, 1, 2); // Every page must have an input to capture keypresses on Unix

			screenLabels[0] = Helper.CreateLabel(39, 0, 42, 1, "======= TERRAIN / TIME =======");


			// Altitude vs. Time Graph
			screenCharts[0] = Helper.CreatePlot(0, 1, 120, 30, -1, -1, -100,3000);
			screenCharts[0].fixedXwidth = 600;
			screenCharts[0].setSeriesColor(0, Color.FromArgb(100, 251, 251, 251));
			screenCharts[0].setSeriesColor(1, Color.FromArgb(100, 0, 251, 0));
		}

		public override void resize()
		{

		}
	}
}
