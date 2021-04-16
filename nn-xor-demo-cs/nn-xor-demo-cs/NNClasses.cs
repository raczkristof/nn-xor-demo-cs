using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nn_xor_demo_cs
{
    public static class Activations
    {
        // Unity activation function. The implementation is inefficient, but represents
        // the nature of an activation function better then "return input". Shows the
        // general implementation scheme for using LINQ to perform the calcualtion on
        // all elemnts of the array.
        public static double[] Identity(this double[] input, bool deriv = false)
        {
            double[] output;

            if (deriv)
                output = (from x in input
                          select 1.0).ToArray();
            else
                output = (from x in input
                          select x).ToArray();

            return output;
        }

        public static double[] Sigmoid(this double[] input, bool deriv = false)
        {
            double[] output;

            if (deriv)
                output = (from x in input
                          select Math.Exp(-x) / Math.Pow(1 + Math.Exp(-x), 2)).ToArray();
            else
                output = (from x in input
                          select 1 / (1 + Math.Exp(-x))).ToArray();

            return output;
        }

        public static double[] TanH(this double[] input, bool deriv = false)
        {
            double[] output;

            if (deriv)
                output = (from x in input
                          select 1 - Math.Tanh(x) * Math.Tanh(x)).ToArray();
            else
                output = (from x in input
                          select Math.Tanh(x)).ToArray();

            return output;
        }

        public static double[] ReLU(this double[] input, bool deriv = false)
        {
            double[] output = new double[input.Length];

            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] < 0) output[i] = 0;
                else
                {
                    if (deriv)
                        output[i] = 1.0;
                    else
                        output[i] = input[i];
                }
            }

            return output;
        }

        public static double[] LReLU(this double[] input, double alpha, bool deriv = false)
        {
            double[] output = new double[input.Length];

            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] < 0)
                {
                    if (deriv)
                        output[i] = alpha;
                    else
                        output[i] = alpha * input[i];
                }
                else
                {
                    if (deriv)
                        output[i] = 1.0;
                    else
                        output[i] = input[i];
                }
            }

            return output;
        }
    }
}
