﻿using System;
using System.Collections.Generic;
using KRPC.Client;
using KRPC.Client.Services.SpaceCenter;

namespace KSP_MOCR
{
	public class StreamCollection
	{
		readonly KRPC.Client.Connection connection;
		private Dictionary<DataType, Kstream> streams = new Dictionary<DataType, Kstream>();

		public StreamCollection(Connection con)
		{
			connection = con;
		}

		public dynamic GetData(DataType type)
		{
			if (!streams.ContainsKey(type))
			{
				try
				{
					addStream(type);
				}
				catch (Exception e)
				{
					Console.WriteLine(e.Message);
					return new object();
				}
			}
			Kstream stream = streams[type];
			dynamic output = stream.Get();
			return output;
		}

		public void CloseStreams()
		{
			foreach (KeyValuePair<DataType, Kstream> stream in streams)
			{
				stream.Value.Remove();
			}
		}

		private void addStream(DataType type)
		{
			// Some much used variables
			Flight flight = connection.SpaceCenter().ActiveVessel.Flight();
			Vessel vessel = connection.SpaceCenter().ActiveVessel;
			Control control = connection.SpaceCenter().ActiveVessel.Control;
			Resources resources = connection.SpaceCenter().ActiveVessel.Resources;
			int stage = control.CurrentStage;
			Resources resources_stage =  connection.SpaceCenter().ActiveVessel.ResourcesInDecoupleStage(stage, false);

			Kstream stream;

			switch (type)
			{
				///// CONTROL DATA /////

				case DataType.control_SAS:
					stream = new boolStream(connection.AddStream(() => control.SAS));
					break;

				case DataType.control_SASmode:
					stream = new sasModeStream(connection.AddStream(() => control.SASMode));
					break;

				case DataType.control_RCS:
					stream = new boolStream(connection.AddStream(() => control.RCS));
					break;

				case DataType.control_gear:
					stream = new boolStream(connection.AddStream(() => control.Gear));
					break;

				case DataType.control_brakes:
					stream = new boolStream(connection.AddStream(() => control.Brakes));
					break;

				case DataType.control_lights:
					stream = new boolStream(connection.AddStream(() => control.Lights));
					break;

				case DataType.control_abort:
					stream = new boolStream(connection.AddStream(() => control.Abort));
					break;

				case DataType.control_actionGroup0:
					stream = new boolStream(connection.AddStream(() => control.GetActionGroup(0)));
					break;

				case DataType.control_actionGroup1:
					stream = new boolStream(connection.AddStream(() => control.GetActionGroup(1)));
					break;

				case DataType.control_actionGroup2:
					stream = new boolStream(connection.AddStream(() => control.GetActionGroup(2)));
					break;

				case DataType.control_actionGroup3:
					stream = new boolStream(connection.AddStream(() => control.GetActionGroup(3)));
					break;

				case DataType.control_actionGroup4:
					stream = new boolStream(connection.AddStream(() => control.GetActionGroup(4)));
					break;

				case DataType.control_actionGroup5:
					stream = new boolStream(connection.AddStream(() => control.GetActionGroup(5)));
					break;

				case DataType.control_actionGroup6:
					stream = new boolStream(connection.AddStream(() => control.GetActionGroup(6)));
					break;

				case DataType.control_actionGroup7:
					stream = new boolStream(connection.AddStream(() => control.GetActionGroup(7)));
					break;

				case DataType.control_actionGroup8:
					stream = new boolStream(connection.AddStream(() => control.GetActionGroup(8)));
					break;

				case DataType.control_actionGroup9:
					stream = new boolStream(connection.AddStream(() => control.GetActionGroup(9)));
					break;

				case DataType.control_throttle:
					stream = new floatStream(connection.AddStream(() => control.Throttle));
					break;

				case DataType.control_currentStage:
					stream = new intStream(connection.AddStream(() => control.CurrentStage));
					break;

				///// FLIGHT DATA /////

				case DataType.flight_gForce:
					stream = new floatStream(connection.AddStream(() => flight.GForce));
					break;

				case DataType.flight_meanAltitude:
					stream = new doubleStream(connection.AddStream(() => flight.MeanAltitude));
					break;

				case DataType.flight_surfaceAltitude:
					stream = new doubleStream(connection.AddStream(() => flight.SurfaceAltitude));
					break;

				case DataType.flight_bedrockAltitude:
					stream = new doubleStream(connection.AddStream(() => flight.BedrockAltitude));
					break;

				case DataType.flight_elevation:
					stream = new doubleStream(connection.AddStream(() => flight.Elevation));
					break;

				case DataType.flight_latitude:
					stream = new doubleStream(connection.AddStream(() => flight.Latitude));
					break;

				case DataType.flight_longitude:
					stream = new doubleStream(connection.AddStream(() => flight.Longitude));
					break;

				case DataType.flight_velocity:
					stream = new tuple3Stream(connection.AddStream(() => flight.Velocity));
					break;

				case DataType.flight_speed:
					stream = new doubleStream(connection.AddStream(() => flight.Speed));
					break;

				case DataType.flight_horizontalSpeed:
					stream = new doubleStream(connection.AddStream(() => flight.HorizontalSpeed));
					break;

				case DataType.flight_verticalSpeed:
					stream = new doubleStream(connection.AddStream(() => flight.VerticalSpeed));
					break;

				case DataType.flight_centerOfMass:
					stream = new tuple3Stream(connection.AddStream(() => flight.CenterOfMass));
					break;

				case DataType.flight_rotation:
					stream = new tuple4Stream(connection.AddStream(() => flight.Rotation));
					break;

				case DataType.flight_direction:
					stream = new tuple3Stream(connection.AddStream(() => flight.Direction));
					break;

				case DataType.flight_pitch:
					stream = new floatStream(connection.AddStream(() => flight.Pitch));
					break;

				case DataType.flight_heading:
					stream = new floatStream(connection.AddStream(() => flight.Heading));
					break;

				case DataType.flight_roll:
					stream = new floatStream(connection.AddStream(() => flight.Roll));
					break;

				case DataType.flight_atmosphereDensity:
					stream = new floatStream(connection.AddStream(() => flight.AtmosphereDensity));
					break;

				case DataType.flight_dynamicPressure:
					stream = new floatStream(connection.AddStream(() => flight.DynamicPressure));
					break;

				case DataType.flight_staticPressure:
					stream = new floatStream(connection.AddStream(() => flight.StaticPressure));
					break;


				///// RESOURCE DATA /////

				case DataType.resource_total_max_electricCharge:
					stream = new floatStream(connection.AddStream(() => resources.Max("ElectricCharge")));
					break;

				case DataType.resource_total_amount_electricCharge:
					stream = new floatStream(connection.AddStream(() => resources.Amount("ElectricCharge")));
					break;

				case DataType.resource_total_max_monoPropellant:
					stream = new floatStream(connection.AddStream(() => resources.Max("MonoPropellant")));
					break;

				case DataType.resource_total_amount_monoPropellant:
					stream = new floatStream(connection.AddStream(() => resources.Amount("MonoPropellant")));
					break;

				case DataType.resource_stage_max_liquidFuel:
					stream = new floatStream(connection.AddStream(() => resources_stage.Max("LiquidFuel")));
					break;

				case DataType.resource_stage_amount_liquidFuel:
					stream = new floatStream(connection.AddStream(() => resources_stage.Amount("LiquidFuel")));
					break;

				case DataType.resource_stage_max_oxidizer:
					stream = new floatStream(connection.AddStream(() => resources_stage.Max("Oxidizer")));
					break;

				case DataType.resource_stage_amount_oxidizer:
					stream = new floatStream(connection.AddStream(() => resources_stage.Amount("Oxidizer")));
					break;



				///// VESSEL DATA /////
				
				case DataType.vessel_MET:
					stream = new doubleStream(connection.AddStream(() => vessel.MET));
					break;
				
				default:
					throw (new Exception("DataType: " + type.ToString() + " not supported"));
			}

			streams.Add(type, stream);
		}
	}
}
