using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace l4neyro
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<Function> functionList = new List<Function>();
            double step = Convert.ToDouble(textBox1.Text);
            double sigma = Convert.ToDouble(textBox2.Text);
            int trainingsCount = Convert.ToInt32(textBox3.Text);

            List<double> pr = new List<double>();
            List<double> er = new List<double>();
            double firstXValueForPrediction = TeachNeuralNetwork(functionList, step, trainingsCount);

            int countOfPrediction = Convert.ToInt32(textBox4.Text); ;

            for (int i = 0; i < countOfPrediction; ++i)
            {
                functionList = ProcessData(functionList, sigma, out double y);
                pr.Add(y);
            }

            int k = 0;
            for (double x = firstXValueForPrediction; k < countOfPrediction; x += step, ++k)
            {
                er.Add(GetExactFunctionResult(x));
            }

            foreach (var item in er)
            {
                dataGridView1.Rows.Add(item);
            }

            foreach (var item in pr)
            {
                dataGridView2.Rows.Add(item);
            }
        }
                
        private double GetExactFunctionResult(double x)
        {
            return Math.Sin(2 * Math.PI * x);
        }

        public List<double> CalculateR(List<Function> functionList, Function function)
        {
            List<Double> listR = new List<double>();
            foreach (var currentFunction in functionList)
            {
                double sum = Math.Pow((currentFunction.X1 - function.X1), 2) + Math.Pow((currentFunction.X2 - function.X2), 2);
                listR.Add(Math.Sqrt(sum));
            }
            return listR;
        }

        private List<double> CalculateD(List<Double> listR, double sigma)
        {
            List<Double> listD = new List<double>();
            foreach (var item in listR)
            {
                listD.Add(Math.Exp(-Math.Pow(item, 2.0) / Math.Pow(sigma, 2.0)));
            }
            return listD;
        }

        private double TeachNeuralNetwork(List<Function> functionList, double step, int trainingsCount)
        {
            double x = 0;
            for (; x < Math.Ceiling(step * trainingsCount); x += step * 4)
            {
                functionList.Add(new Function
                {
                    X1 = GetExactFunctionResult(x),
                    X2 = GetExactFunctionResult(x + step),
                    X3 = GetExactFunctionResult(x + step * 2),
                    FunctionResult = GetExactFunctionResult(x + step * 3)
                });
            }

            return x;
        }

        private List<Function> ProcessData(List<Function> functionList, double sigma, out double y)
        {
            List<Double> listR = new List<double>();
            List<Double> listD = new List<double>();
            Function function = new Function
            {
                X1 = functionList.Last().X2,
                X2 = functionList.Last().X3,
                X3 = functionList.Last().FunctionResult,
            };

            listR = CalculateR(functionList, function);
            listD = CalculateD(listR, sigma);

            y = PredictResult(listD, functionList);
            function.FunctionResult = y;
             
            functionList.Add(function);

            return functionList;
        }

        private double PredictResult(List<Double> listD, List<Function> functionList)
        {
            double sumDY = 0;
            double sumD = 0;
            int i = 0;
            for(; i < listD.Count(); i++)
            {
                sumDY += listD[i] * functionList[i].FunctionResult;
                sumD += listD[i];
            }        
            return sumDY / sumD;
        }

    }
}
