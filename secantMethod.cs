using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using org.mariuszgromada.math.mxparser;

namespace CafeteriaGUI
{
    public partial class secantMethod : Form
    {
        public secantMethod()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(equation.Text) || string.IsNullOrWhiteSpace(xlower.Text) || string.IsNullOrWhiteSpace(ex.Text))
                {
                    MessageBox.Show("Enter the data needed!");
                    return;
                }

                string format = "#.#####";
                List<object[]> dataList = new List<object[]>();

                double marginE = 0.001;
                double x0 = double.Parse(xlower.Text);
                double x1 = double.Parse(ex.Text);
                double roott = 0;
                double error = Math.Abs(x1 - x0);

                int iterations = 0;

                while (error > marginE)
                {
                    iterations++;
                    double fx0 = EvaluateFunction(equation.Text, x0);
                    double fx1 = EvaluateFunction(equation.Text, x1);

                    if (fx0 == fx1)
                    {
                        MessageBox.Show("Divide by zero error in the secant method.");
                        return;
                    }

                    double x2 = x1 - (fx1 * (x0 - x1)) / (fx0 - fx1);
                    double fx2 = EvaluateFunction(equation.Text, x2);

                    string decimalnumXO = x0.ToString(format);
                    string decimalnumXL = x1.ToString(format);
                    string decimalnumX2 = x2.ToString(format);
                    string decimalnumFx0 = fx0.ToString(format);
                    string decimalnumFx1 = fx1.ToString(format);
                    string decimalnumFx2 = fx2.ToString(format);
                    string decimalnumE = error.ToString(format);

                    object[] rowData = { iterations, decimalnumXO, decimalnumXL, decimalnumX2, decimalnumFx0, decimalnumFx1, decimalnumFx2, decimalnumE };
                    dataList.Add(rowData);

                    UpdateDataGrid(dataList);

                    error = Math.Abs(x2 - x1);
                    x0 = x1;
                    x1 = x2;
                    roott = x2;
                }

                roottt.Text = roott.ToString(format);
                PlotGraph(equation.Text, x0, roott);
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
                dataGridView2.Columns.Add("x1", "x1");
                dataGridView2.Columns.Add("x2", "x2");
                dataGridView2.Columns.Add("f(x0)", "f(x0)");
                dataGridView2.Columns.Add("f(x1)", "f(x1)");
                dataGridView2.Columns.Add("f(x2)", "f(x2)");
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
    }
}
