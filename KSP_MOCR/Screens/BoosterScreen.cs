﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using KRPC.Client;
using KRPC.Client.Services.KRPC;
using KRPC.Client.Services.SpaceCenter;


namespace KSP_MOCR
{
	class BoosterScreen : MocrScreen
	{
		KRPC.Client.Services.SpaceCenter.Flight flight;
		//KRPC.Client.Services.SpaceCenter.Orbit orbit;
		KRPC.Client.Services.SpaceCenter.Control control;

		KRPC.Client.Stream<KRPC.Client.Services.SpaceCenter.Vessel> vessel_stream;
		KRPC.Client.Stream<KRPC.Client.Services.SpaceCenter.Flight> flight_stream;
		KRPC.Client.Stream<KRPC.Client.Services.SpaceCenter.Control> control_stream;

		int currentStage;
		Vessel vessel;
		
		double g = 8.80556;

		public BoosterScreen(Screen form)
		{
			this.form = form;
			this.chartData = form.form.chartData;
			
			screenStreams = new StreamCollection(form.connection);

			this.width = 120;
			this.height = 30;
		}

		public override void makeElements()
		{
			for (int i = 0; i < 100; i++) screenLabels.Add(null); // Initialize Labels
			for (int i = 0; i < 12; i++) screenIndicators.Add(null); // Initialize Indicators
			for (int i = 0; i < 2; i++) screenInputs.Add(null); // Initialize Inputs

			screenInputs[0] = Helper.CreateInput(-2, -2, 1, 2); // Every page must have an input to capture keypresses on Unix



			screenLabels[0] = Helper.CreateLabel(16, 1, 13); // Local Time
			screenLabels[1] = Helper.CreateLabel(0, 1, 14); // MET Time
			screenLabels[2] = Helper.CreateLabel(39, 0, 42, 1, "============= BOOSTER MODULE ============="); // Screen Title
			screenLabels[3] = Helper.CreateLabel(84, 0, 39, 1, "├───────────── STATUS ─────────────┤"); // Status Headline
			screenLabels[4] = Helper.CreateLabel(84, 1, 1, 1, "│");
			screenLabels[5] = Helper.CreateLabel(0, 2, 85, 1, "───────────────── ENGINES ──────────────────────────────────────────────────────────┤"); // Obrit/Position headline
			screenLabels[6] = Helper.CreateLabel(44, 3, 1, 1, "│");
			screenLabels[7] = Helper.CreateLabel(44, 4, 1, 1, "│");
			screenLabels[8] = Helper.CreateLabel(44, 5, 1, 1, "│");
			screenLabels[9] = Helper.CreateLabel(44, 6, 1, 1, "│");
			screenLabels[10] = Helper.CreateLabel(44, 7, 1, 1, "│");
			screenLabels[11] = Helper.CreateLabel(44, 8, 1, 1, "│");
			screenLabels[12] = Helper.CreateLabel(44, 9, 1, 1, "│");
			screenLabels[13] = Helper.CreateLabel(44, 10, 1, 1, "│");
			screenLabels[14] = Helper.CreateLabel(44, 11, 1, 1, "│");
			screenLabels[15] = Helper.CreateLabel(44, 12, 1, 1, "│");
			screenLabels[16] = Helper.CreateLabel(84, 3, 1, 1, "│");
			screenLabels[17] = Helper.CreateLabel(84, 4, 1, 1, "│");
			screenLabels[18] = Helper.CreateLabel(84, 5, 1, 1, "│");
			screenLabels[19] = Helper.CreateLabel(84, 6, 1, 1, "│");
			screenLabels[20] = Helper.CreateLabel(84, 7, 36, 1, "│┌─────────── SUPPLIES ────────────┐");
			screenLabels[21] = Helper.CreateLabel(84, 8, 1, 1, "│");
			screenLabels[22] = Helper.CreateLabel(84, 9, 1, 1, "│");
			screenLabels[23] = Helper.CreateLabel(84, 10, 1, 1, "│");
			screenLabels[24] = Helper.CreateLabel(84, 11, 1, 1, "│");
			screenLabels[25] = Helper.CreateLabel(84, 12, 1, 1, "│");
			screenLabels[26] = Helper.CreateLabel(44, 13, 1, 1, "│");
			screenLabels[27] = Helper.CreateLabel(44, 14, 1, 1, "│");
			screenLabels[28] = Helper.CreateLabel(84, 13, 1, 1, "│");
			screenLabels[29] = Helper.CreateLabel(84, 14, 1, 1, "│");
			screenLabels[30] = Helper.CreateLabel(44, 15, 1, 1, "│");



			// Engine data
			screenLabels[40] = Helper.CreateLabel(45, 3, 30, 1, "      THRUST  MAX THR  vISP"); // Headline
			screenLabels[41] = Helper.CreateLabel(45, 4, 30, 1, ""); // Engine 1
			screenLabels[42] = Helper.CreateLabel(45, 5, 30, 1, ""); // Engine 2
			screenLabels[43] = Helper.CreateLabel(45, 6, 30, 1, ""); // Engine 3
			screenLabels[44] = Helper.CreateLabel(45, 7, 30, 1, ""); // Engine 4
			screenLabels[45] = Helper.CreateLabel(45, 8, 30, 1, ""); // Engine 5
			screenLabels[46] = Helper.CreateLabel(45, 9, 30, 1, ""); // Engine 6
			screenLabels[47] = Helper.CreateLabel(45, 10, 30, 1, ""); // Engine 7
			screenLabels[48] = Helper.CreateLabel(45, 11, 30, 1, ""); // Engine 8
			screenLabels[49] = Helper.CreateLabel(45, 12, 30, 1, ""); // Engine 9
			screenLabels[50] = Helper.CreateLabel(45, 13, 30, 1, ""); // Engine 10
			screenLabels[51] = Helper.CreateLabel(45, 14, 30, 1, ""); // Engine 11
			screenLabels[52] = Helper.CreateLabel(45, 15, 30, 1, ""); // Engine 12
			screenLabels[53] = Helper.CreateLabel(45, 16, 30, 1, ""); // Engine 13
			screenLabels[54] = Helper.CreateLabel(45, 17, 30, 1, ""); // Engine 14
			screenLabels[55] = Helper.CreateLabel(45, 18, 30, 1, ""); // Engine 15
			screenLabels[56] = Helper.CreateLabel(45, 19, 30, 1, ""); // Engine 16
			screenLabels[57] = Helper.CreateLabel(45, 20, 30, 1, ""); // Engine 17
			screenLabels[58] = Helper.CreateLabel(45, 21, 30, 1, ""); // Engine 18
			screenLabels[59] = Helper.CreateLabel(45, 22, 30, 1, ""); // Engine 19
			screenLabels[60] = Helper.CreateLabel(45, 23, 30, 1, ""); // Engine 20

			// Weight and TWR
			screenLabels[35] = Helper.CreateLabel(0, 16, 45, 1, "────────────────────────────────────────────┤");
			screenLabels[36] = Helper.CreateLabel(1, 17, 30, 1, ""); // Weight
			screenLabels[37] = Helper.CreateLabel(1, 18, 30, 1, "        CUR   LOW   MAX");
			screenLabels[38] = Helper.CreateLabel(1, 19, 30, 1, ""); // TWR
			

			// Supplies
			screenLabels[70] = Helper.CreateLabel(85, 8, 35, 1, "                     "); // Supply line 1
			screenLabels[71] = Helper.CreateLabel(85, 9, 35, 1, "                     "); // Supply line 2
			screenLabels[72] = Helper.CreateLabel(85, 10, 35, 1, "                     "); // Supply line 3
			screenLabels[73] = Helper.CreateLabel(85, 11, 35, 1, "                     "); // Supply line 4
			screenLabels[74] = Helper.CreateLabel(85, 12, 35, 1, "                     "); // Supply line 5
			screenLabels[75] = Helper.CreateLabel(85, 13, 35, 1, "                     "); // Supply line 5
			screenLabels[78] = Helper.CreateLabel(85, 14, 35, 1, "└─────────────────────────────────┘"); // Supply line 6

			// Status
			screenIndicators[0] = Helper.CreateIndicator(86, 1, 10, 1, "SAS");
			screenIndicators[1] = Helper.CreateIndicator(97, 1, 10, 1, "RCS");
			screenIndicators[2] = Helper.CreateIndicator(108, 1, 10, 1, "GEAR");
			screenIndicators[3] = Helper.CreateIndicator(86, 2, 10, 1, "BRAKES");
			screenIndicators[4] = Helper.CreateIndicator(97, 2, 10, 1, "LIGHTS");
			screenIndicators[5] = Helper.CreateIndicator(108, 2, 10, 1, "ABORT");
			screenIndicators[6] = Helper.CreateIndicator(86, 4, 10, 1, "POWER HI");
			screenIndicators[7] = Helper.CreateIndicator(97, 4, 10, 1, "G HIGH");
			screenIndicators[8] = Helper.CreateIndicator(108, 4, 10, 1, "LOX LOW");
			screenIndicators[9] = Helper.CreateIndicator(86, 5, 10, 1, "POWER LOW");
			screenIndicators[10] = Helper.CreateIndicator(97, 5, 10, 1, "MONO LOW");
			screenIndicators[11] = Helper.CreateIndicator(108, 5, 10, 1, "FUEL LOW");

			// DELTA V BUDGET
			screenLabels[80] = Helper.CreateLabel(0, 26, 23, 1, "── DELTA V AVAILABLE ──");
			screenLabels[81] = Helper.CreateLabel(0, 27, 23, 1, "  STAGE: xxxxx.x m/s");
			screenLabels[82] = Helper.CreateLabel(0, 28, 23, 1, "  TOTAL: xxxxx.x m/s");
			
			screenLabels[83] = Helper.CreateLabel(23, 26, 15, 1, "");
			
			screenLabels[85] = Helper.CreateLabel(23, 28, 15, 1, "");
			
			screenLabels[86] = Helper.CreateLabel(0, 23, 12, 1, "REQUIRED ΔV:");
			screenLabels[84] = Helper.CreateLabel(0, 24, 22, 1, "  xxxxx.x m/s");
			
			screenInputs[1] = Helper.CreateInput(13, 23, 10, 1, HorizontalAlignment.Right);
			screenInputs[1].Text = "0";

			for (int i = 0; i < 1; i++) screenCharts.Add(null); // Initialize Charts

			// Gee-Force vs. Time Graph
			screenCharts[0] = Helper.CreatePlot(85, 15, 35, 15, 0, 300);
		}

		public override void updateLocalElements(object sender, EventArgs e)
		{
			// Re-usable data variable for graph data
			List<List<KeyValuePair<double, double?>>> data = new List<List<KeyValuePair<double, double?>>>();
			List<Plot.Type> types = new List<Plot.Type>();


			if (form.form.connected && form.form.krpc.CurrentGameScene == GameScene.Flight)
			{
				if (this.vessel_stream == null) this.vessel_stream = form.connection.AddStream(() => form.form.spaceCenter.ActiveVessel);
				if (this.flight_stream == null) this.flight_stream = form.connection.AddStream(() => form.form.spaceCenter.ActiveVessel.Flight(form.form.spaceCenter.ActiveVessel.Orbit.Body.ReferenceFrame));
				if (this.control_stream == null) this.control_stream = form.connection.AddStream(() => form.form.spaceCenter.ActiveVessel.Control);
				


				vessel = vessel_stream.Get(); // 1 RPC
				flight = flight_stream.Get(); // 1 RPC
				control = control_stream.Get(); // 1 RPC

				currentStage = screenStreams.GetData(DataType.control_currentStage);


				screenLabels[0].Text = " LT: " + Helper.timeString(DateTime.Now.TimeOfDay.TotalSeconds);
				screenLabels[1].Text = "MET: " + Helper.timeString(vessel.MET, 3); // 0 RPC

				/** 
				 * Engines
				 **/

				//  Get parts in current stage
				screenLabels[50].Text = "Stage: " + currentStage.ToString(); // 0 RPC

				bool foundEngine = false;
				double multiplier = 91;
				double maxDev = 0;

				double stageCurThr = 0;
				double stageMaxThr = 0;

				int n = 0;

				for (int i = currentStage; i >= 0; i--)
				{
					IList<Part> parts = vessel.Parts.InStage(i);
					foreach (Part part in parts)
					{
						Engine eng = part.Engine;
						if (eng != null)
						{
							foundEngine = true;
							Tuple<double, double, double> pos = part.Position(vessel.ReferenceFrame);

							double left = pos.Item1;
							double top = pos.Item3;

							double devX = Math.Abs(left);
							if(devX > maxDev) { maxDev = devX; }
							double devY = Math.Abs(top);
							if (devY > maxDev) { maxDev = devY; }

							if (screenEngines.Count < (n + 1))
							{
								screenEngines.Add(null);
								screenEngines[n] = Helper.CreateEngine(2, 3, 6, 3, (n + 1).ToString());
							}

							screenEngines[n].offsetX = left;
							screenEngines[n].offsetY = top;

							if (eng.Thrust > 0) { screenEngines[n].setStatus(true); }
							else { screenEngines[n].setStatus(false); }

							// Engine data
							screenLabels[41 + n].Text = Helper.prtlen((n + 1).ToString(), 2, Helper.Align.RIGHT) + ":"
								+ "  " + Helper.prtlen(Helper.toFixed(eng.Thrust/1000, 1), 7, Helper.Align.RIGHT)
								+ "  " + Helper.prtlen(Helper.toFixed(eng.MaxThrust/1000, 1), 7, Helper.Align.RIGHT)
								+ "  " + Helper.prtlen(Helper.toFixed(eng.VacuumSpecificImpulse, 1), 5, Helper.Align.RIGHT);

							stageCurThr += eng.Thrust;
							stageMaxThr += eng.MaxThrust;

							n++;
						}
					}

					if (foundEngine)
					{ 
						if (maxDev > 1)
						{
							multiplier = 91 / maxDev;
						}

						// position indicators
						for(int j = 0; j < n; j++)
						{
							int x = (int)Math.Round(167 + (screenEngines[j].offsetX * multiplier));
							int y = (int)Math.Round(157 + (screenEngines[j].offsetY * multiplier));
							screenEngines[j].Location = new Point(x, y);
						}

						break;
					}
				}

				// Disable other engineIndicators
				while(n < screenEngines.Count)
				{
					screenEngines[n].Dispose();
					screenEngines.RemoveAt(n);
					screenLabels[41 + n].Text = "";
					n++;
				}
				screenEngines.TrimExcess();
				
				// Weight and TWR
				double weight = vessel.Mass / 1000;
				double TWRc = (stageCurThr / 1000) / (weight * 9.81);
				double TWRm = (stageMaxThr / 1000) / (weight * 9.81); ;
				screenLabels[36].Text = "Weight: " + Helper.prtlen(Helper.toFixed(weight, 1), 5, Helper.Align.RIGHT) + "t";
				screenLabels[38].Text = "   TWR: " + Helper.prtlen(Helper.toFixed(TWRc, 2), 4, Helper.Align.RIGHT)
					+ "  " + Helper.prtlen(Helper.toFixed(TWRm, 2), 4, Helper.Align.RIGHT);

				
				// Supplies
				double mF = vessel.ResourcesInDecoupleStage(vessel.Control.CurrentStage, false).Max("LiquidFuel");
				double cF = vessel.ResourcesInDecoupleStage(vessel.Control.CurrentStage, false).Amount("LiquidFuel");

				double mO = vessel.ResourcesInDecoupleStage(vessel.Control.CurrentStage, false).Max("Oxidizer");
				double cO = vessel.ResourcesInDecoupleStage(vessel.Control.CurrentStage, false).Amount("Oxidizer");

				double mM = vessel.ResourcesInDecoupleStage(vessel.Control.CurrentStage, false).Max("MonoPropellant");
				double cM = vessel.ResourcesInDecoupleStage(vessel.Control.CurrentStage, false).Amount("MonoPropellant");

				double mE = vessel.ResourcesInDecoupleStage(vessel.Control.CurrentStage, false).Max("ElectricCharge");
				double cE = vessel.ResourcesInDecoupleStage(vessel.Control.CurrentStage, false).Amount("ElectricCharge");

				screenLabels[70].Text = "         LF     LO     MP     EC";
				screenLabels[71].Text = "STAGE:"
										+ Helper.prtlen(Math.Round(cF).ToString(), 7, Helper.Align.RIGHT)
										+ Helper.prtlen(Math.Round(cO).ToString(), 7, Helper.Align.RIGHT)
										+ Helper.prtlen(Helper.toFixed(cM, 2), 7, Helper.Align.RIGHT)
										+ Helper.prtlen(Math.Round(cE).ToString(), 7, Helper.Align.RIGHT);
				screenLabels[72].Text = "    %:"
										+ Helper.prtlen(Math.Round((cF / mF) * 100).ToString(), 7, Helper.Align.RIGHT)
										+ Helper.prtlen(Math.Round((cO / mO) * 100).ToString(), 7, Helper.Align.RIGHT)
										+ Helper.prtlen(Math.Round((cM / mM) * 100).ToString(), 7, Helper.Align.RIGHT)
										+ Helper.prtlen(Math.Round((cE / mE) * 100).ToString(), 7, Helper.Align.RIGHT);

				mF = vessel.Resources.Max("LiquidFuel");
				cF = vessel.Resources.Amount("LiquidFuel");

				mO = vessel.Resources.Max("Oxidizer");
				cO = vessel.Resources.Amount("Oxidizer");

				mM = vessel.Resources.Max("MonoPropellant");
				cM = vessel.Resources.Amount("MonoPropellant");

				mE = vessel.Resources.Max("ElectricCharge");
				cE = vessel.Resources.Amount("ElectricCharge");

				screenLabels[74].Text = "  TOT:"
										+ Helper.prtlen(Math.Round(cF).ToString(), 7, Helper.Align.RIGHT)
										+ Helper.prtlen(Math.Round(cO).ToString(), 7, Helper.Align.RIGHT)
										+ Helper.prtlen(Helper.toFixed(cM, 2), 7, Helper.Align.RIGHT)
										+ Helper.prtlen(Math.Round(cE).ToString(), 7, Helper.Align.RIGHT);
				screenLabels[75].Text = "    %:"
										+ Helper.prtlen(Math.Round((cF / mF) * 100).ToString(), 7, Helper.Align.RIGHT)
										+ Helper.prtlen(Math.Round((cO / mO) * 100).ToString(), 7, Helper.Align.RIGHT)
										+ Helper.prtlen(Math.Round((cM / mM) * 100).ToString(), 7, Helper.Align.RIGHT)
										+ Helper.prtlen(Math.Round((cE / mE) * 100).ToString(), 7, Helper.Align.RIGHT);

				// Status
				if (vessel.Control.SAS) { screenIndicators[0].setStatus(Indicator.status.GREEN); } else { screenIndicators[0].setStatus(Indicator.status.OFF); } // SAS
				if (vessel.Control.RCS) { screenIndicators[1].setStatus(Indicator.status.GREEN); } else { screenIndicators[1].setStatus(Indicator.status.OFF); } // RCS
				if (vessel.Control.Gear) { screenIndicators[2].setStatus(Indicator.status.GREEN); } else { screenIndicators[2].setStatus(Indicator.status.OFF); } // GEAR
				if (vessel.Control.Brakes) { screenIndicators[3].setStatus(Indicator.status.RED); } else { screenIndicators[3].setStatus(Indicator.status.OFF); } // Break
				if (vessel.Control.Lights) { screenIndicators[4].setStatus(Indicator.status.AMBER); } else { screenIndicators[4].setStatus(Indicator.status.OFF); } // Lights
				if (vessel.Control.Abort) { screenIndicators[5].setStatus(Indicator.status.RED); } else { screenIndicators[5].setStatus(Indicator.status.OFF); } // Abort

				if (flight.GForce > 4) { screenIndicators[7].setStatus(Indicator.status.RED); } else { screenIndicators[7].setStatus(Indicator.status.OFF); } // G High

				float maxR = vessel.Resources.Max("ElectricCharge");
				float curR = vessel.Resources.Amount("ElectricCharge");
				if (curR / maxR > 0.95) { screenIndicators[6].setStatus(Indicator.status.GREEN); } else { screenIndicators[6].setStatus(Indicator.status.OFF); } // Power High
				if (curR / maxR < 0.1) { screenIndicators[9].setStatus(Indicator.status.RED); } else { screenIndicators[9].setStatus(Indicator.status.OFF); } // Power Low

				maxR = vessel.Resources.Max("MonoPropellant");
				curR = vessel.Resources.Amount("MonoPropellant");
				if (curR / maxR < 0.1) { screenIndicators[10].setStatus(Indicator.status.RED); } else { screenIndicators[10].setStatus(Indicator.status.OFF); } // Monopropellant Low

				maxR = vessel.ResourcesInDecoupleStage(vessel.Control.CurrentStage, false).Max("LiquidFuel");
				curR = vessel.ResourcesInDecoupleStage(vessel.Control.CurrentStage, false).Amount("LiquidFuel");
				if (curR / maxR < 0.1) { screenIndicators[11].setStatus(Indicator.status.RED); } else { screenIndicators[11].setStatus(Indicator.status.OFF); } // Fuel Low

				maxR = vessel.ResourcesInDecoupleStage(vessel.Control.CurrentStage, false).Max("Oxidizer");
				curR = vessel.ResourcesInDecoupleStage(vessel.Control.CurrentStage, false).Amount("Oxidizer");
				if (curR / maxR < 0.1) { screenIndicators[8].setStatus(Indicator.status.RED); } else { screenIndicators[8].setStatus(Indicator.status.OFF); } // LOW Low


				// Delta V
				// TOTO: Maybe buttonifize this, so the calculations are not done every refresh.
				updateDeltaVStats();
				

				// Graphs
				data = new List<List<KeyValuePair<double, double?>>>();
				types = new List<Plot.Type>();
				data.Add(Helper.limit(chartData["geeTime"],300));
				types.Add(Plot.Type.LINE);
				screenCharts[0].setData(data, types, true);
			}
		}

		public override void resize() { }

		private void updateDeltaVStats()
		{
			int currentStage = vessel.Control.CurrentStage;
			
			Parts Parts = vessel.Parts;
			double[] stageWetMass = new double[currentStage + 1];
			double[] stageDryMass = new double[currentStage + 1];
			double[] stageISP = new double[currentStage + 1];
			double[] stageDV = new double[currentStage + 1];
			double[] stageThrust = new double[currentStage + 1];
			
			// Iterate all (remaining) stages
			for (int i = currentStage; i >= 0; i--)
			{
				IList<Part> stageParts = Parts.InDecoupleStage(i);
				double wetMass = 0;
				double dryMass = 0;
				List<double> engineThrust = new List<double>();
				List<double> engineISP = new List<double>();

				// Iterate all parts
				foreach (Part part in stageParts)
				{
					wetMass += part.Mass;
					dryMass += part.DryMass;

					Engine engine = part.Engine;
					if (engine != null)
					{
						engineThrust.Add(engine.MaxThrust);
						engineISP.Add(engine.VacuumSpecificImpulse);
					}
				}

				stageWetMass[i] = wetMass;
				stageDryMass[i] = dryMass;

				// Sum the engines
				double totalThrust = 0;
				double totalDivisor = 0;

				for (int j = 0; j < engineThrust.Count; j++)
				{
					totalThrust += engineThrust[j];
					totalDivisor += engineThrust[j] / engineISP[j];
				}
				stageThrust[i] = totalThrust;
				stageISP[i] = totalThrust / totalDivisor;
			}

			// Go through and calulate stage Delta-V
			String stageDeltas = "";
			bool curStage = false;
			for (int i = stageISP.Length - 1; i >= 0; i--)
			{
				double totalDryMass = stageDryMass[i];
				double totalWetMass = stageWetMass[i];
				
				for (int j = i - 1; j >= 0; j--)
				{
					totalDryMass += stageWetMass[j];
					totalWetMass += stageWetMass[j];
				}
				stageDV[i] = Math.Log(totalWetMass / totalDryMass) * stageISP[i] * g;

				if (curStage == false && !double.IsNaN(stageDV[i]))
				{
					stageDeltas += Helper.prtlen(Helper.toFixed(stageDV[i], 2), 8);
					curStage = true;
				}
			}

			screenLabels[81].Text = "  STAGE: " + stageDeltas;

			// SUM TOTAL
			double totalDV = 0;
			foreach (double dv in stageDV)
			{
				if (!double.IsNaN(dv)) totalDV += dv; 
			}
			screenLabels[82].Text = "  TOTAL: " + Helper.prtlen(Helper.toFixed(totalDV, 2), 8);

			// CALCULATE BURN TIME
			double sDv = 0;
			double sTh = 0;
			for (int i = stageDV.Length - 1; i >= 0; i--)
			{
				if (!double.IsNaN(stageDV[i]))
				{
					sDv = stageDV[i];
					sTh = stageThrust[i];
					break;
				}
			}

			double deltaV;
			try
			{
				deltaV = double.Parse(screenInputs[1].Text);
			}
			catch (Exception)
			{
				deltaV = 0;
			}
			double burnSecs = burnTime(deltaV, sDv, sTh);
			screenLabels[84].Text = "  BURN TIME: " + Helper.timeString(burnSecs, false);
		}

		private double burnTime(double dV, double ISP, double thrust)
		{
			double massInit = vessel.Mass;
			double massFine = massInit * Math.Exp(-dV / (ISP * g));
			double massProp = massInit - massFine;
			double massDot = thrust / (ISP * g);
			return massProp / massDot;
		}
	}
}
