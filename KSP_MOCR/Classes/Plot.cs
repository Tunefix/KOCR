﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace KSP_MOCR
{
	public class Plot : Control
	{
		public Color gridColor { get; set; }
		public Color labelColor { get; set; }
		public int minX { get; set; }
		public int maxX { get; set; }
		public int minY { get; set; }
		public int maxY { get; set; }

		private bool autoXmin = false;
		private bool autoXmax = false;
		private bool autoYmin = false;
		private bool autoYmax = false;

		private int plotTop;
		private int plotRight;
		private int plotBottom;
		private int plotLeft;
		private int plotWidth;
		private int plotHeight;

		private int XAxisHeight = 20;
		private int YAxisWidth = 40;

		private double xScaler;
		private double yScaler;

		private List<KeyValuePair<double, double?>>[] series;
		private bool multiAxis = false;
		double[] axisData;
		Type[] seriesType;

		private Pen axisPen;
		private Pen gridPen;
		private Brush labelBrush;
		PointF[] line = new PointF[2];
		Pen linePen;

		public enum Type { LINE, CROSS }

		private StringFormat stringFormat;

		readonly List<Color> chartLineColors = new List<Color>();

		public int fixedXwidth = -1; // set fixed nubmer of items on x-axis. Pads to the left if items are less. Pads to the right if minX < 0.


		public Plot()
		{
			chartLineColors.Add(Color.FromArgb(255, 204, 51, 0));
			chartLineColors.Add(Color.FromArgb(255, 0, 51, 204));
			chartLineColors.Add(Color.FromArgb(255, 0, 169, 51));
			chartLineColors.Add(Color.FromArgb(100, 251, 251, 251));
			chartLineColors.Add(Color.FromArgb(100, 251, 251, 251));
			chartLineColors.Add(Color.FromArgb(100, 251, 251, 251));
			chartLineColors.Add(Color.FromArgb(100, 251, 251, 251));
			chartLineColors.Add(Color.FromArgb(100, 251, 251, 251));
			chartLineColors.Add(Color.FromArgb(100, 251, 251, 251));
			chartLineColors.Add(Color.FromArgb(100, 251, 251, 251));
			chartLineColors.Add(Color.FromArgb(100, 251, 251, 251));
			chartLineColors.Add(Color.FromArgb(100, 251, 251, 251));

			// Turn on double buffering
			this.DoubleBuffered = true;
		}

		public void setSeriesColor(int id, Color c)
		{
			while (chartLineColors.Count <= id)
			{
				chartLineColors.Add(c);
			}
			chartLineColors[id] = c;
		}

		public void setData(List<Dictionary<double, double?>> data, List<Type> types, bool multipleYaxis)
		{
			List<List<KeyValuePair<double, double?>>> tmpData = new List<List<KeyValuePair<double, double?>>>();
			
			foreach (Dictionary<double, double?> graph in data)
			{
				tmpData.Add(graph.ToList());
			}
			setData(tmpData, types, multipleYaxis);
		}

		public void setData(List<List<KeyValuePair<double, double?>>> data, List<Type> types, bool multipleYaxis)
		{
			series = data.ToArray();
			seriesType = types.ToArray();
			multiAxis = multipleYaxis;
			this.Invalidate();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			bool invX = false;
			bool invY = false;

			if (series != null)
			{
				axisPen = new Pen(gridColor, 2.0f);
				gridPen = new Pen(gridColor, 1.0f);
				labelBrush = new SolidBrush(labelColor);

				Graphics g = e.Graphics;
				g.SmoothingMode = SmoothingMode.AntiAlias;
				g.InterpolationMode = InterpolationMode.HighQualityBilinear;
				g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

				if (maxX == -1 || autoXmax == true) { maxX = findMaxX(); autoXmax = true; }
				if (minX == -1 || autoXmin == true) { minX = findMinX(); autoXmin = true; }

				// Adjust minX if fixedXwidth is set
				if (fixedXwidth != -1)
				{
					int diff = maxX - minX;
					if (diff < fixedXwidth)
					{
						int pad = fixedXwidth - diff;
						minX -= pad;

						if (minX < 0)
						{
							pad = minX * -1;
							minX = 0;
							maxX += pad;
						}
					}
				}

				plotTop = Margin.Top;
				plotRight = Size.Width - Margin.Right;
				plotBottom = Size.Height - Margin.Bottom - XAxisHeight;

				if (!multiAxis)
				{
					if (maxY == -1 || autoYmax == true) { maxY = findMaxY(); autoYmax = true; }
					if (minY == -1 || autoYmin == true) { minY = findMinY(); autoYmin = true; }
					plotLeft = YAxisWidth;
					plotWidth = plotRight - plotLeft;
					plotHeight = plotBottom - plotTop;

					
					YAxisWidth = maxY.ToString().Length * 9;
					if (minY > maxY) YAxisWidth = minY.ToString().Length * 9;

					// Draw YAxis With Labels
					// Determine best grid-size
					axisData = getAxisData("Y");
					yScaler = axisData[3];

					// Draw YAxis With Labels
					g.DrawLine(axisPen, plotLeft, plotBottom, plotLeft, plotTop);

					for (int i = 0; i <= axisData[2]; i++)
					{
						int y = plotBottom - (int)Math.Round((axisData[1] * i));
						g.DrawLine(gridPen, plotLeft, y, plotRight, y);

						stringFormat = new StringFormat();
						stringFormat.Alignment = StringAlignment.Far;
						stringFormat.LineAlignment = StringAlignment.Center;
						if (axisData[4] == 1)
						{
							g.DrawString((minY - (i * axisData[0])).ToString(), Font, labelBrush, plotLeft, y, stringFormat);
							invY = true;
						}
						else
						{
							g.DrawString(((i * axisData[0]) + minY).ToString(), Font, labelBrush, plotLeft, y, stringFormat);
						}
					}
				}
				else
				{
					plotLeft = YAxisWidth * series.Length;
					plotWidth = plotRight - plotLeft;
					plotHeight = plotBottom - plotTop;
				}



				// Determine best grid-size
				axisData = getAxisData("X");
				xScaler = axisData[3];


				// Draw XAxis With Labels
				g.DrawLine(axisPen, plotLeft, plotBottom, plotRight, plotBottom);

				for (int i = 0; i <= axisData[2]; i++)
				{
					int x = (int)Math.Round(plotLeft + (axisData[1] * i));
					g.DrawLine(gridPen, x, plotBottom, x, plotTop);

					stringFormat = new StringFormat();
					stringFormat.Alignment = StringAlignment.Center;
					stringFormat.LineAlignment = StringAlignment.Near;
					if (axisData[4] == 1)
					{
						g.DrawString((minX - (i * axisData[0])).ToString(), Font, labelBrush, x, plotBottom, stringFormat);
						invX = true;
					}
					else
					{
						g.DrawString(((i * axisData[0]) + minX).ToString(), Font, labelBrush, x, plotBottom, stringFormat);
					}
				}


				int n = 0;
				foreach (List<KeyValuePair<double, double?>> serie in series)
				{
					if (multiAxis)
					{
						if (maxY == -1 || autoYmax == true) { maxY = findMaxY(serie); autoYmax = true; }
						if (minY == -1 || autoYmin == true) { minY = findMinY(serie); autoYmin = true; }

						// Draw THIS YAxis with Labels
						// Determine best grid-size
						axisData = getAxisData("Y");
						yScaler = axisData[3];


						// Draw YAxis With Labels
						g.DrawLine(axisPen, plotLeft - (n * YAxisWidth), plotBottom, plotLeft - (n * YAxisWidth), plotTop);

						for (int i = 0; i <= axisData[2]; i++)
						{
							int y = plotBottom - (int)Math.Round((axisData[1] * i));
							g.DrawLine(gridPen, plotLeft, y, plotRight, y);

							stringFormat = new StringFormat();
							stringFormat.Alignment = StringAlignment.Far;
							stringFormat.LineAlignment = StringAlignment.Center;
							g.DrawString(((i * axisData[0]) + minY).ToString(), Font, labelBrush, plotLeft, y, stringFormat);
						}
					}

					if (seriesType[n] == Type.CROSS)
					{
						drawCross(g, n, serie, invX, invY);
					}
					else
					{
						drawLine(g, n, serie, invX, invY);
					}
					n++;
				}
			}
		}

		private void drawCross(Graphics g, int n, List<KeyValuePair<double, double?>> serie) { drawLine(g, n, serie, false, false); }
		private void drawCross(Graphics g, int n, List<KeyValuePair<double, double?>> serie, bool invX, bool invY)
		{
			/**
			 * DRAW CROSSES
			 */
			linePen = new Pen(chartLineColors[n], 2.0f);
			double? value;
			double i;
			float x, y;

			foreach (KeyValuePair<double, double?> p in serie)
			{
				if (p.Value != null && !double.IsInfinity((double)p.Value) && !double.IsNaN(p.Key) && xScaler != 0 && yScaler != 0)
				{

					i = p.Key;
					value = p.Value;

					if (invX)
					{
						x = (float)((minX / xScaler) - (i / xScaler) + plotLeft);
					}
					else
					{
						x = (float)((i / xScaler) + plotLeft - (minX / xScaler));
					}

					if (invY)
					{
						y = (float)((value / yScaler) - (maxY / yScaler) + Margin.Top);
					}
					else
					{
						y = (float)(plotBottom - (value / yScaler) + (minY / yScaler));
					}

					line[0] = new PointF(x - 4, y - 4);
					line[1] = new PointF(x + 4, y + 4);
					g.DrawLines(linePen, line);
					line[0] = new PointF(x - 4, y + 4);
					line[1] = new PointF(x + 4, y - 4);
					g.DrawLines(linePen, line);
				}
			}
		}

		private void drawLine(Graphics g, int n, List<KeyValuePair<double, double?>> serie) { drawLine(g, n, serie, false, false); }
		private void drawLine(Graphics g, int n, List<KeyValuePair<double, double?>> serie, bool invX, bool invY)
		{
			/*
			 * DRAW THE LINE
			 */
			linePen = new Pen(chartLineColors[n], 2.0f);
			bool started = false;

			double? value;
			double i;
			float x, y;

			foreach (KeyValuePair<double, double?> p in serie)
			{
				i = p.Key;
				value = p.Value;
				if (value != null && yScaler != 0 && xScaler != 0)
				{
					if (invX)
					{
						x = (float)((minX / xScaler) - (i / xScaler) + plotLeft);
					}
					else
					{
						x = (float)((i / xScaler) + plotLeft - (minX / xScaler));
					}

					if (invY)
					{
						y = (float)((value / yScaler) - (maxY / yScaler) + Margin.Top);
					}
					else
					{
						y = (float)(plotBottom - (value / yScaler) + (minY / yScaler));
					}


					if (!started)
					{
						started = true;

						line[0] = new PointF(x, y);
					}
					else
					{

						line[1] = new PointF(x, y);
						g.DrawLines(linePen, line);
						line[0] = line[1];
					}
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="a"></param>
		/// <returns>double[4]
		/// [0] - GridStep
		/// [1] - GridStepPx
		/// [2] - GridLines
		/// [3] - xSclaer</returns>
		private double[] getAxisData(String a)
		{
			double[] ret = new double[5];
			if (a == "X")
			{
				int split = Math.Abs(maxX - minX);
				double xPrPx = split / (double)plotWidth;
				int xLabelMaxWidth = maxX.ToString().Length * 12;
				if(minX > maxX) xLabelMaxWidth = minX.ToString().Length * 12;
				int maxLabels = (int)Math.Floor((double)(plotWidth / xLabelMaxWidth));
				double minStep = (split / (double)maxLabels);
				int minStepInt = (int)minStep;
				double gridStep = Math.Ceiling(minStep / Math.Pow(10, minStepInt.ToString().Length - 1)) * Math.Pow(10, minStepInt.ToString().Length - 1);
				int gridLines = (int)Math.Floor(split / gridStep);
				double gridStepPx = gridStep / xPrPx;

				// CHECK FOR INVERTED AXIS (highest value towards origin)
				double inv = 0;
				if (minX > maxX) inv = 1; 

				ret[0] = gridStep;
				ret[1] = gridStepPx;
				ret[2] = gridLines;
				ret[3] = xPrPx;
				ret[4] = inv;
			}
			else
			{
				int split = Math.Abs(maxY - minY);
				double xPrPx = split / (double)plotHeight;
				int xLabelMaxWidth = 16;
				int maxLabels = (int)Math.Floor((double)(plotHeight / xLabelMaxWidth));
				double minStep = (split / (double)maxLabels);
				int minStepInt = (int)minStep;
				double gridStep = Math.Ceiling(minStep / Math.Pow(10, minStepInt.ToString().Length - 1)) * Math.Pow(10, minStepInt.ToString().Length - 1);
				int gridLines = (int)Math.Floor(split / gridStep);
				double gridStepPx = gridStep / xPrPx;

				// CHECK FOR INVERTED AXIS (highest value towards origin)
				double inv = 0;
				if (minY > maxY) inv = 1;

				ret[0] = gridStep;
				ret[1] = gridStepPx;
				ret[2] = gridLines;
				ret[3] = xPrPx;
				ret[4] = inv;
			}
			return ret;
		}

		public int findMaxX()
		{
			double? tMax = null;
			if (series == null) { return 0; }
			lock (series)
			{
				foreach (List<KeyValuePair<double, double?>> serie in series)
				{
					lock(serie)
					{
						foreach (KeyValuePair<double, double?> point in serie)
						{
							if (point.Value != null)
							{
								if (tMax == null)
								{
									tMax = point.Key;
								}
								else if (point.Key > tMax)
								{
									tMax = point.Key;
								}
							}
						}
					}
				}
			}
			return tMax == null ? 1: (int)tMax;
		}

		public int findMinX()
		{
			double? tMin = null;
			if(series == null) { return 0; }
			lock(series)
			{
				foreach (List<KeyValuePair<double, double?>> serie in series)
				{
					lock(serie)
					{
						foreach (KeyValuePair<double, double?> point in serie)
						{
							if (point.Value != null)
							{
								if (tMin == null)
								{
									tMin = point.Key;
								}
								else if (point.Key < tMin)
								{
									tMin = point.Key;
								}
							}
						}
					}
				}
			}
			return tMin == null ? 0: (int)tMin;
		}

		private int findMaxY()
		{
			double? tMax = null;
			lock(series)
			{
				foreach (List<KeyValuePair<double, double?>> serie in series)
				{
					lock(serie)
					{
						foreach (KeyValuePair<double, double?> point in serie)
						{
							if (point.Value != null)
							{
								if (tMax == null)
								{
									tMax = point.Value;
								}
								else if (point.Value > tMax)
								{
									tMax = point.Value;
								}
							}
						}
					}
				}
			}

			if (tMax == null)
			{
				return 0;
			}
			return (int)Math.Ceiling((double)tMax);
		}

		private int findMinY()
		{
			double? tMin = null;
			lock(series)
			{
				foreach (List<KeyValuePair<double, double?>> serie in series)
				{
					lock(serie)
					{
						foreach (KeyValuePair<double, double?> point in serie)
						{
							if (point.Value != null)
							{
								if (tMin == null)
								{
									tMin = point.Value;
								}
								else if (point.Value < tMin)
								{
									tMin = point.Value;
								}
							}
						}
					}
				}
			}
			return tMin == null ? 0 : (int)Math.Floor((double)tMin);
		}

		private int findMaxY(List<KeyValuePair<double, double?>> serie)
		{
			double? tMax = null;
			lock(serie)
			{
				foreach (KeyValuePair<double, double?> point in serie)
				{
					if (point.Value != null)
					{
						if (tMax == null)
						{
							tMax = point.Value;
						}
						else if (point.Value > tMax)
						{
							tMax = point.Value;
						}
					}
				}
			}
			
			if (tMax != null)
			{
				return (int)Math.Ceiling((double)tMax);
			}
			else
			{
				return 1;
			}
		}

		private int findMinY(List<KeyValuePair<double, double?>> serie)
		{
			double? tMin = null;
			lock(serie)
			{
				foreach (KeyValuePair<double, double?> point in serie)
				{
					if (point.Value != null)
					{
						if (tMin == null)
						{
							tMin = point.Value;
						}
						else if (point.Value < tMin)
						{
							tMin = point.Value;
						}
					}
				}
			}
			return tMin == null ? 0 : (int)Math.Floor((double)tMin);
		}

		public int findMinX(List<KeyValuePair<double, double?>> serie)
		{
			double? tMin = null;
			lock (serie)
			{
				foreach (KeyValuePair<double, double?> point in serie)
				{
					if (point.Value != null)
					{
						if (tMin == null)
						{
							tMin = point.Key;
						}
						else if (point.Key < tMin)
						{
							tMin = point.Key;
						}
					}
				}
			}
			return tMin == null ? 0 : (int)Math.Floor((double)tMin);
		}

		public int findMaxX(List<KeyValuePair<double, double?>> serie)
		{
			double? tMin = null;
			lock (serie)
			{
				foreach (KeyValuePair<double, double?> point in serie)
				{
					if (point.Value != null)
					{
						if (tMin == null)
						{
							tMin = point.Key;
						}
						else if (point.Key > tMin)
						{
							tMin = point.Key;
						}
					}
				}
			}
			return tMin == null ? 0 : (int)Math.Floor((double)tMin);
		}
	}
}
