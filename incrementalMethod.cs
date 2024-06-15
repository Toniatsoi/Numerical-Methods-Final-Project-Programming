using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using org.mariuszgromada.math.mxparser;

namespace CafeteriaGUI
{
    public partial class incrementalMethod : Form
    {
        public incrementalMethod()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(equation.Text) || string.IsNullOrWhiteSpace(xlower.Text) || string.IsNullOrWhiteSpace(deltax.Text))
                {
                    MessageBox.Show("Enter the data needed!");
                    return;
                }

                string format = "#.########";
                List<object[]> dataList = new List<object[]>();

                double marginE = 0.00001; // Decreased margin of error
                double valL = double.Parse(xlower.Text);
                double delta = double.Parse(deltax.Text) / 10; // Decrease initial step size
                double root = 0;
                double valU = valL + delta;
                int iterCount = 0;
                double prevU = 0;

                double error = Math.Abs(valU - prevU) / Math.Abs(valU) * 100;
                bool signChange = false;
                double currX = valL;
                double currY = EvaluateFunction(equation.Text, currX);
                double prevY = currY;

                // Check if the initial lower bound is already a root
                if (EvaluateFunction(equation.Text, valL) == 0)
                {
                    root = valL;
                    roott.Text = root.ToString("F4");
                    PlotGraph(equation.Text, valL, root);
                    return;
                }

                while (error > marginE && !signChange)
                {
                    iterCount++;
                    double fL = EvaluateFunction(equation.Text, valL);
                    double fU = EvaluateFunction(equation.Text, valU);
                    double product = fL * fU;

                    string decA = valL.ToString("F4");
                    string decC = delta.ToString("F4");
                    string decB = valU.ToString("F4");
                    string decFA = fL.ToString("F4");
                    string decFB = fU.ToString("F4");
                    string decError = iterCount == 1 ? " " : ((Math.Abs(valU - prevU) / Math.Abs(valU)) * 100).ToString(format);

                    string sign = product < 0 ? "<0" : product > 0 ? ">0" : "0";

                    object[] rowData = { iterCount, decA, decC, decB, decFA, decFB, error, sign };
                    dataList.Add(rowData);

                    UpdateDataGrid(dataList);

                    if (prevY * currY < 0)
                    {
                        double tolerance = 0.00001; // Decreased tolerance
                        double lowerBound = currX - delta;
                        double upperBound = currX;

                        do
                        {
                            root = (lowerBound * EvaluateFunction(equation.Text, upperBound) - upperBound * EvaluateFunction(equation.Text, lowerBound))
                                   / (EvaluateFunction(equation.Text, upperBound) - EvaluateFunction(equation.Text, lowerBound));

                            double fA = EvaluateFunction(equation.Text, lowerBound);
                            double fC = EvaluateFunction(equation.Text, root);

                            if (Math.Abs(fC) < tolerance)
                            {
                                break;
                            }
                            else if (fA * fC < 0)
                            {
                                upperBound = root;
                            }
                            else
                            {
                                lowerBound = root;
                            }

                        } while (Math.Abs(upperBound - lowerBound) > tolerance);

                        roott.Text = root.ToString("F4");
                        PlotGraph(equation.Text, valL, root);
                        signChange = true; // Exit the loop
                    }
                    else
                    {
                        prevY = currY;
                        currX += delta;
                        currY = EvaluateFunction(equation.Text, currX);
                    }

                    if (error < marginE && fL * fU == 0)
                    {
                        root = valU;
                        break;
                    }

                    if (fL * fU < 0)
                    {
                        delta /= 10.0; // Decrease step size more aggressively
                    }
                    else
                    {
                        valL = valU;
                    }

                    prevU = valU;
                    valU = valL + delta;
                    error = Math.Abs(valU - prevU);//    / Math.Abs(valU) * 100;

                    // Recalculate error and check if we need to update marginE
                    if (Math.Abs(valU - prevU) < marginE)
                    {
                        error = 0;
                    }

                    // Update the loop termination condition to avoid premature exits
                    if (Math.Abs(fU) < marginE)
                    {
                        root = valU;
                        signChange = true;
                    }
                }

                if (!signChange)
                {
                    roott.Text = root.ToString("F4");
                    PlotGraph(equation.Text, valL, root);
                }

                if (root == 0)
                {
                    MessageBox.Show("No root found within specific range.");
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
                dataGridView2.Columns.Add("Val L", "Val L");
                dataGridView2.Columns.Add("Delta", "Delta");
                dataGridView2.Columns.Add("Val U", "Val U");
                dataGridView2.Columns.Add("f(L)", "f(L)");
                dataGridView2.Columns.Add("f(U)", "f(U)");
                dataGridView2.Columns.Add("Error", "Error");
                dataGridView2.Columns.Add("Sign", "Sign");
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

        private void PlotGraph(string equation, double initialX, double root)
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

        private void dashboardButton_Click(object sender, EventArgs e)
        {

        }
    }
}
