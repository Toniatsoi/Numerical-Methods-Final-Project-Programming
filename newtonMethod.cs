using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using org.mariuszgromada.math.mxparser;
using static MathNet.Symbolics.VisualExpression;

namespace CafeteriaGUI
{
    public partial class newtonMethod : Form
    {
        public newtonMethod()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(equation.Text) || string.IsNullOrWhiteSpace(xlower.Text))
                {
                    MessageBox.Show("Enter the data needed!");
                    return;
                }

                string format = "#.#########";
                List<object[]> dataList = new List<object[]>();

                double marginE = 0.001;
                double x0 = double.Parse(xlower.Text); // Initial guess
                double x1 = 0;
                int iterations = 0;
                double error = double.MaxValue;

                while (error > marginE)
                {
                    iterations++;
                    double fx0 = EvaluateFunction(equation.Text, x0);
                    double fDashX0 = DifferentiateFunction(equation.Text, x0);

                    // Newton-Raphson formula for finding next approximation
                    x1 = x0 - (fx0 / fDashX0);
                    double fx1 = EvaluateFunction(equation.Text, x1);

                    string decimalnumX0 = x0.ToString(format);
                    string decimalnumX1 = x1.ToString(format);
                    string decimalnumFX0 = fx0.ToString(format);
                    string decimalnumFX1 = fx1.ToString(format);
                    string decimalnumError = iterations == 1 ? " " : (Math.Abs(x1 - x0) / Math.Abs(x1) * 100).ToString(format);

                    object[] rowData = { iterations, decimalnumX0, decimalnumFX0, decimalnumX1, decimalnumFX1, decimalnumError };
                    dataList.Add(rowData);

                    UpdateDataGrid(dataList);

                    // Debug message
                    Console.WriteLine($"Iteration {iterations}: x0 = {decimalnumX0}, f(x0) = {decimalnumFX0}, f'(x0) = {fDashX0}, x1 = {decimalnumX1}, Error = {decimalnumError}");

                    if (Math.Abs(fx1) < marginE)
                    {
                        break;
                    }

                    error = Math.Abs(x1 - x0) / Math.Abs(x1) * 100;
                    x0 = x1; // Move to the next approximation
                }

                roottt.Text = x1.ToString(format);
                PlotGraph(equation.Text, x1);
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
                dataGridView2.Columns.Add("x0", "x0");
                dataGridView2.Columns.Add("f(x0)", "f(x0)");
                dataGridView2.Columns.Add("x1", "x1");
                dataGridView2.Columns.Add("f(x1)", "f(x1)");
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

        private double DifferentiateFunction(string equation, double x)
        {
            try
            {
                Argument xArg = new Argument("x", x);
                Expression exp = new Expression($"der({equation}, x)", xArg);
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

        private void PlotGraph(string equation, double root)
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
            for (double x = -5.0; x <= 5.0; x += 0.01)
            {
                xArg.setArgumentValue(x);
                double y = exp.calculate();
                functionSeries.Points.AddXY(x, y);
            }

            // Add the root point to the root series
            rootSeries.Points.AddXY(root, EvaluateFunction(equation, root));

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
            chartArea.AxisX.Minimum = -5;
            chartArea.AxisX.Maximum = 5;
            chartArea.AxisY.MajorGrid.LineColor = Color.Black;
            chartArea.AxisX.MajorGrid.LineColor = Color.Black;

            // Refresh the chart to display updates
            chart1.Invalidate();
        }
    }
}
