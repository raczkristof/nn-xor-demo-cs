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
        public static double[] Unity(this double[] input, bool deriv = false)
        {
            double[] output;

            if (deriv)
                output = (from value in input
                          select 1.0).ToArray();
            else
                output = (from value in input
                          select value).ToArray();

            return output;
        }
    }
}
