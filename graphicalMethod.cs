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
    public partial class graphicalMethod : Form
    {
        public graphicalMethod()
        {
            InitializeComponent();
        }

        private void DashBoard_Load(object sender, EventArgs e)
        {
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void ConfirmInput()
        {
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void chart1_Click(object sender, EventArgs e)       
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView2.Rows.Clear();
            string eqStr = equation.Text.Replace(" ", "");
            if (!double.TryParse(value.Text, out double valX))
            {
                MessageBox.Show("Invalid x value. Please enter a valid number.");
                return;
            }

            List<object[]> dataList = new List<object[]>();

            double prevY = EvaluateFunction(eqStr, valX);
            dataList.Add(new object[] { valX.ToString("F4"), prevY.ToString("F4") });

            double inc = 0.2;
            double nextX = valX + inc;
            double nextY = EvaluateFunction(eqStr, nextX);
            bool signChange = false;

            double rootVal = double.NaN;

            try
            {
                while (!signChange)
                {
                    dataList.Add(new object[] { nextX.ToString("F4"), nextY.ToString("F4") });
                    UpdateDataGrid(dataList);

                    if (prevY * nextY < 0)
                    {
                        double tolerance = 0.0001;
                        double root = 0;
                        double lowerBound = nextX - inc;
                        double upperBound = nextX;

                        do
                        {
                            root = (lowerBound * EvaluateFunction(eqStr, upperBound) - upperBound * EvaluateFunction(eqStr, lowerBound))
                                   / (EvaluateFunction(eqStr, upperBound) - EvaluateFunction(eqStr, lowerBound));

                            double fA = EvaluateFunction(eqStr, lowerBound);
                            double fC = EvaluateFunction(eqStr, root);

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

                        rootVal = root;
                        roott.Text = rootVal.ToString("F4");

                        signChange = true; // Set signChange to true to exit the loop

                        PlotGraph(eqStr, valX, rootVal);
                    }
                    else
                    {
                        prevY = nextY;
                        nextX += inc;
                        nextY = EvaluateFunction(eqStr, nextX);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
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

        private void UpdateDataGrid(List<object[]> dataList)
        {
            // Clear the existing columns and rows
            dataGridView2.Columns.Clear();
            dataGridView2.Rows.Clear();

            // Add columns if not already added
            if (dataGridView2.Columns.Count == 0)
            {
                dataGridView2.Columns.Add("X", "X");
                dataGridView2.Columns.Add("f(X)", "f(X)");
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

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void roott_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
