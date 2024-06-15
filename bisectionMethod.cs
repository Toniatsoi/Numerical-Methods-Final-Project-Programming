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
    public partial class bisectionMethod : Form
    {
        public bisectionMethod()
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
                double xM = (xL + xU) / 2.0;
                int iterations = 0;
                double prevXM = 0;

                double error = Math.Abs(xM - prevXM) / Math.Abs(xM) * 100;

                while (error > marginE)
                {
                    iterations++;
                    double fxL = EvaluateFunction(equation.Text, xL);
                    double fxU = EvaluateFunction(equation.Text, xU);
                    double fxM = EvaluateFunction(equation.Text, xM);

                    string decimalnumA = xL.ToString(format);
                    string decimalnumB = xU.ToString(format);
                    string decimalnumC = xM.ToString(format);
                    string decimalnumFA = fxL.ToString(format);
                    string decimalnumFB = fxM.ToString(format);
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
                    }
                    else
                    {
                        xL = xM;
                    }

                    prevXM = xM;
                    xM = (xL + xU) / 2;
                    error = Math.Abs(xM - prevXM); // / Math.Abs(xM) * 100;
                }

                if (roott != 0)
                {
                    roottt.Text = roott.ToString(format);
                    PlotGraph(equation.Text, xL, roott);
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
                dataGridView2.Columns.Add("xR", "xR");
                dataGridView2.Columns.Add("xU", "xU");
                dataGridView2.Columns.Add("f(xL)", "f(xL)");
                dataGridView2.Columns.Add("f(xR)", "f(xR)");
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
