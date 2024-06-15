using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using org.mariuszgromada.math.mxparser;

namespace CafeteriaGUI
{
    public partial class regual_falsiMethod : Form
    {
        public regual_falsiMethod()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(equation.Text) || string.IsNullOrWhiteSpace(xlower.Text) || string.IsNullOrWhiteSpace(xupper.Text))
                {
                    MessageBox.Show("Enter the data needed!");
                    return;
                }

                string format = "#.####";
                List<object[]> dataList = new List<object[]>();

                double marginE = 0.0001;
                double xL = double.Parse(xlower.Text);
                double xU = double.Parse(xupper.Text);
                double roott = 0;
                double xM = 0;
                int iterations = 0;
                double prevXM = 0;

                double error = double.MaxValue;

                // Initial check to ensure the interval is valid
                double fxL = EvaluateFunction(equation.Text, xL);
                double fxU = EvaluateFunction(equation.Text, xU);

                if (fxL * fxU > 0)
                {
                    MessageBox.Show("The function values at the interval boundaries must have opposite signs.");
                    return;
                }

                while (error > marginE && iterations < 100) // Adding a maximum iteration check
                {
                    iterations++;

                    xM = (xL * fxU - xU * fxL) / (fxU - fxL);
                    double fxM = EvaluateFunction(equation.Text, xM);

                    string decimalnumA = xL.ToString(format);
                    string decimalnumB = xU.ToString(format);
                    string decimalnumC = xM.ToString(format);
                    string decimalnumFA = fxL.ToString(format);
                    string decimalnumFB = fxU.ToString(format);
                    string decimalnumERROR = iterations == 1 ? " " : (Math.Abs(xM - prevXM) / Math.Abs(xM) * 100).ToString(format);

                    object[] rowData = { iterations, decimalnumA, decimalnumC, decimalnumB, decimalnumFA, decimalnumFB, decimalnumERROR };
                    dataList.Add(rowData);

                    UpdateDataGrid(dataList);

                    if (Math.Abs(fxM) < marginE)
                    {
                        roott = xM;
                        break;
                    }

                    if (fxL * fxM < 0)
                    {
                        xU = xM;
                        fxU = fxM; // Update fxU for the next iteration
                    }
                    else
                    {
                        xL = xM;
                        fxL = fxM; // Update fxL for the next iteration
                    }

                    error = Math.Abs(xM - prevXM); /// Math.Abs(xM) * 100;
                    prevXM = xM;
                }

                if (Math.Abs(EvaluateFunction(equation.Text, xM)) < marginE)
                {
                    roottt.Text = xM.ToString(format);
                    PlotGraph(equation.Text, xL, xU, xM);
                }
                else
                {
                    MessageBox.Show("No root found within the specified range.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void UpdateDataGrid(List<object[]> dataList)
        {
            // Clear existing columns and rows
            dataGridView2.Columns.Clear();
            dataGridView2.Rows.Clear();

            // Add columns if not already added
            if (dataGridView2.Columns.Count == 0)
            {
                dataGridView2.Columns.Add("Iteration", "Iteration");
                dataGridView2.Columns.Add("xL", "xL");
                dataGridView2.Columns.Add("xM", "xM");
                dataGridView2.Columns.Add("xU", "xU");
                dataGridView2.Columns.Add("f(xL)", "f(xL)");
                dataGridView2.Columns.Add("f(xU)", "f(xU)");
                dataGridView2.Columns.Add("Error", "Error");
            }

            // Add rows from the dataList
            foreach (var row in dataList)
            {
                dataGridView2.Rows.Add(row);
            }

            // Adjust the column widths
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView2.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private double EvaluateFunction(string equation, double x)
        {
            try
            {
                Argument xArg = new Argument("x", x);
                Expression exp = new Expression(equation, xArg);
                double result = exp.calculate();
                if (double.IsNaN(result))
                {
                    throw new Exception("Invalid expression");
                }
                return result;
            }
            catch (Exception e)
            {
                MessageBox.Show($"Error: {e.Message}");
                return double.NaN;
            }
        }

        private void PlotGraph(string equation, double xL, double xU, double root)
        {
            // Create series for the function
            var functionSeries = new Series
            {
                Name = "f(x)",
                ChartType = SeriesChartType.Line,
                Color = Color.Blue
            };

            // Create series for the root
            var rootSeries = new Series
            {
                Name = "Root",
                ChartType = SeriesChartType.Point,
                Color = Color.Red,
                MarkerSize = 10,
                MarkerStyle = MarkerStyle.Circle
            };

            Argument xArg = new Argument("x");
            Expression exp = new Expression(equation, xArg);

            // Define the range and step for plotting the function
            for (double x = xL - 1.0; x <= xU + 1.0; x += 0.01)
            {
                xArg.setArgumentValue(x);
                double y = exp.calculate();
                functionSeries.Points.AddXY(x, y);
            }

            // Add the root point to the root series
            double yRoot = EvaluateFunction(equation, root);
            rootSeries.Points.AddXY(root, yRoot);
            Console.WriteLine($"Root: ({root}, {yRoot})");

            // Clear any existing series and add the new series to the chart
            chart1.Series.Clear();
            chart1.Series.Add(functionSeries);
            chart1.Series.Add(rootSeries);

            // Customize chart area appearance
            var chartArea = chart1.ChartAreas[0];
            chartArea.BackColor = Color.White;
            chartArea.AxisX.Title = "X";
            chartArea.AxisX.TitleForeColor = Color.Black;
            chartArea.AxisX.LabelStyle.ForeColor = Color.Black;
            chartArea.AxisY.Title = "f(X)";
            chartArea.AxisY.TitleForeColor = Color.Black;
            chartArea.AxisY.LabelStyle.ForeColor = Color.Black;
            chartArea.AxisX.Minimum = xL - 1;
            chartArea.AxisX.Maximum = xU + 1;
            chartArea.AxisY.MajorGrid.LineColor = Color.Black;
            chartArea.AxisX.MajorGrid.LineColor = Color.Black;

            // Ensure the Y-axis includes the root point
            double minY = functionSeries.Points.Min(point => point.YValues[0]);
            double maxY = functionSeries.Points.Max(point => point.YValues[0]);
            chartArea.AxisY.Minimum = minY - 1;
            chartArea.AxisY.Maximum = maxY + 1;

            // Refresh the chart to display updates
            chart1.Invalidate();
        }
    }
}
