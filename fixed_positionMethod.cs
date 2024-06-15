using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using org.mariuszgromada.math.mxparser;

namespace CafeteriaGUI
{
    public partial class fixed_positionMethod : Form
    {
        public fixed_positionMethod()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(equation.Text) || string.IsNullOrWhiteSpace(ex.Text))
                {
                    MessageBox.Show("Enter the data needed!");
                    return;
                }

                string format = "#.####";
                List<object[]> dataList = new List<object[]>();

                double marginE = 0.001;
                double x0 = double.Parse(ex.Text); // Initial guess
                double x1 = 0;
                int iterations = 0;
                double error = double.MaxValue;
                int maxIterations = 100; // Add a maximum iteration limit

                while (error > marginE && iterations < maxIterations)
                {
                    iterations++;
                    x1 = GFunction(x0); // Fixed-point iteration: x1 = g(x0)

                    if (double.IsNaN(x1) || double.IsInfinity(x1))
                    {
                        MessageBox.Show("Function evaluation resulted in an invalid number. Please check the function or initial guess.");
                        return;
                    }

                    string decimalnumX0 = x0.ToString(format);
                    string decimalnumX1 = x1.ToString(format);
                    string decimalnumError;

                    if (iterations == 1)
                    {
                        decimalnumError = " ";
                    }
                    else
                    {
                        error = Math.Abs(x1 - x0);
                        decimalnumError = error.ToString(format);
                    }

                    object[] rowData = { iterations, decimalnumX0, decimalnumX1, decimalnumError };
                    dataList.Add(rowData);

                    UpdateDataGrid(dataList);

                    x0 = x1;
                }

                roottt.Text = x1.ToString(format); // Display the root
                PlotGraph(equation.Text, x1); // Plot the graph

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
                dataGridView2.Columns.Add("f(xL)", "f(xL)");
                dataGridView2.Columns.Add("xL", "xL");
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

        private double GFunction(double x)
        {
            // Example: Let's choose g(x) = x - f(x) / f'(x), similar to the Newton-Raphson method
            double fx = EvaluateFunction(equation.Text, x);
            double fDashX = DifferentiateFunction(equation.Text, x);

            // Avoid division by zero
            if (fDashX == 0)
            {
                throw new DivideByZeroException("Derivative is zero. Division by zero error.");
            }

            return x - (fx / fDashX);
        }

        private double DifferentiateFunction(string equationStr, double xValue)
        {
            double h = 0.000001; // a small value
            return (EvaluateFunction(equationStr, xValue + h) - EvaluateFunction(equationStr, xValue)) / h;
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
