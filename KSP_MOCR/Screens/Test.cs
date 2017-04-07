﻿using KRPC.Client;
using KRPC.Client.Services;
using KRPC.Client.Services.KRPC;
using KRPC.Client.Services.SpaceCenter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSP_MOCR
{
	class TestScreen : MocrScreen
	{

		KRPC.Schema.KRPC.Status status;
		private KRPC.Client.Services.SpaceCenter.Flight flight;
		KRPC.Client.Stream<KRPC.Client.Services.SpaceCenter.Flight> flight_stream;

		public TestScreen(Form1 form)
		{
			this.form = form;

			this.width = 120;
			this.height = 30;
		}

		public override void updateLocalElements(object sender, EventArgs e)
		{
			if (form.connected && form.krpc.CurrentGameScene == GameScene.Flight) // krpc.CurrentGameScene is 1 RPC
			{

				if (flight_stream == null)
				{
					var vessel = this.form.spaceCenter.ActiveVessel;
					var refframe = vessel.Orbit.Body.ReferenceFrame;

					try
					{
						this.flight_stream = this.form.connection.AddStream(() => vessel.Flight(refframe));
					}
					catch (Exception) { }
				}


				// GET DATA
				flight = flight_stream.Get();

				screenLabels[1].Text = flight.MeanAltitude.ToString();

				//flight = GetData.getFlight(); // 7 RPC
				//screenLabels[1].Text = flight.MeanAltitude.ToString(); // 7 RPC

				//screenLabels[1].Text = connection.SpaceCenter().ActiveVessel.Flight().MeanAltitude.ToString(); // 3 RPC
			}

			screenLabels[9].Text = "CTRL STATUS: " + form.ctrlDown.ToString();

		}

		public override void makeElements()
		{
			for (int i = 0; i < 30; i++) screenLabels.Add(null); // Initialize Labels

			screenLabels[0] = Helper.CreateLabel(1, 4, 58, 1); // Connection Status
			screenLabels[0].Text = "NOT CONNECTED";
			screenLabels[0].TextAlign = ContentAlignment.TopLeft;

			screenLabels[1] = Helper.CreateLabel(61, 4, 58, 2);

			screenLabels[4] = Helper.CreateLabel(0, 0, 60, 2, "──────────────────── CONNECTION DETAILS ────────────────────"); // Connection Header}

			screenLabels[2] = Helper.CreateLabel(1, 6, 58, 2, "Label 2");

			screenLabels[3] = Helper.CreateLabel(0, 10, 120, 1, "┼─ ─ ─ ─ ─┬─ ─ ─ ─ ─┬─ ─ ─ ─ ─┬─ ─ ─ ─ ─┬─ ─ ─ ─ ─┬─ ─ ─ ─ ─┬─ ─ ─ ─ ─┬─ ─ ─ ─ ─┬─ ─ ─ ─ ─┬─ ─ ─ ─ ─┬─ ─ ─ ─ ─┬─ ─ ─ ─ ┼");
			screenLabels[5] = Helper.CreateLabel(0, 11, 1, 1, "│"); // TEST
			screenLabels[6] = Helper.CreateLabel(119, 11, 1, 1, "│"); // TEST
			screenLabels[7] = Helper.CreateLabel(0, 12, 1, 2, "│\n││"); // TEST
			screenLabels[8] = Helper.CreateLabel(1, 14, 1, 1, "│"); // TEST

			screenLabels[9] = Helper.CreateLabel(1, 5, 58, 2, "CTRL STATUS:");

			screenLabels[10] = Helper.CreateLabel(10, 11, 1, 1, "│"); // TEST
			screenLabels[11] = Helper.CreateLabel(20, 11, 1, 1, "│"); // TEST
			screenLabels[12] = Helper.CreateLabel(30, 11, 1, 1, "│"); // TEST
			screenLabels[13] = Helper.CreateLabel(40, 11, 1, 1, "│"); // TEST
			screenLabels[14] = Helper.CreateLabel(50, 11, 1, 1, "│"); // TEST
			screenLabels[15] = Helper.CreateLabel(60, 11, 1, 1, "│"); // TEST
			screenLabels[16] = Helper.CreateLabel(70, 11, 1, 1, "│"); // TEST

			screenLabels[17] = Helper.CreateLabel(20, 15, 6, 1, "HHgåfy"); // TEST
			screenLabels[18] = Helper.CreateLabel(20, 16, 6, 1, "HHåygf"); // TEST
			screenLabels[19] = Helper.CreateLabel(20, 17, 6, 1, "HHgåfy"); // TEST

			screenLabels[20] = Helper.CreateLabel(5, 15, 1, 1, "┼");
			screenLabels[21] = Helper.CreateLabel(6, 15, 1, 1, "┼");
			screenLabels[22] = Helper.CreateLabel(5, 16, 1, 1, "┼");
			screenLabels[23] = Helper.CreateLabel(6, 16, 1, 1, "┼");

			screenLabels[24] = Helper.CreateLabel(10, 14, 1, 1, "┼");

			screenLabels[25] = Helper.CreateLabel(0, 10, 120, 1);
			screenLabels[25].Location = new Point(4, 350);
			screenLabels[25].Size = new Size((int)(120 * form.pxPrChar), (int)form.pxPrLine);
			screenLabels[25].Text = "┼─ ─ ─ ─ ─┬─ ─ ─ ─ ─┬─ ─ ─ ─ ─┬─ ─ ─ ─ ─┬─ ─ ─ ─ ─┬─ ─ ─ ─ ─┬─ ─ ─ ─ ─┬─ ─ ─ ─ ─┬─ ─ ─ ─ ─┬─ ─ ─ ─ ─┬─ ─ ─ ─ ─┬─ ─ ─ ─ ┼";
			screenLabels[25].ForeColor = Color.FromArgb(255, 255, 255);
			screenLabels[25].BackColor = Color.Maroon;

		}

		public override void destroyStreams()
		{
			
		}
	}
}
