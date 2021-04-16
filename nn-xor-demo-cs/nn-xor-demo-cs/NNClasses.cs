using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nn_xor_demo_cs
{
    public class NNModel
    {
        private NNLayer[] layers;
        private int nInputs;

        public NNModel(int nInputs, int[] layerSizes, ActivationFunctions[] layerActivations)
        {
            if (layerSizes.Length != layerActivations.Length)
            {
                throw new ArgumentException("Number of layers does not match number of activations specified.");
            }
            else
            {
                this.layers = new NNLayer[layerSizes.Length];
                this.nInputs = nInputs;

                this.layers[0] = new NNLayer(layerSizes[0], nInputs, layerActivations[0]);
                for (int i = 1; i < layerSizes.Length; i++) this.layers[i] = new NNLayer(layerSizes[i], layerSizes[i - 1], layerActivations[i]);
                this.ResetWeights();
            }
        }

        public NNModel(int nInputs, int[] layerSizes, ActivationFunctions[] layerActivations, int seed)
        {
            if (layerSizes.Length != layerActivations.Length)
            {
                throw new ArgumentException("Number of layers does not match number of activations specified.");
            }
            else
            {
                this.layers = new NNLayer[layerSizes.Length];
                this.nInputs = nInputs;

                this.layers[0] = new NNLayer(layerSizes[0], nInputs, layerActivations[0]);
                for (int i = 1; i < layerSizes.Length; i++) this.layers[i] = new NNLayer(layerSizes[i], layerSizes[i - 1], layerActivations[i]);
                this.ResetWeights(seed);
            }
        }

        // Randomize weights (between 1 and 0)
        public void ResetWeights()
        {
            Random rnd = new Random();
            foreach (NNLayer layer in layers) layer.RandomizeWeights(rnd);
        }

        public void ResetWeights(int seed)
        {
            Random rnd = new Random(seed);
            foreach (NNLayer layer in layers) layer.RandomizeWeights(rnd);
        }

        //public double[] Predict(double[,] inputs)
        //{
        //    layers[0].Propagate(inputs);
        //    for (int i = 1; i < layers.Length; i++)
        //    {
        //        layers[i].Propagate(layers[i-1].output);
        //    }
        //}
    }

    public class NNLayer
    {
        public int nInputs;
        public double[,] weights;
        public double[,] output;

        public ActivationFunctions activationType;
        public bool bias;

        public NNLayer(int nNeurons, int nInputs, ActivationFunctions activationType, bool bias=true)
        {
            this.nInputs = nInputs;
            this.activationType = activationType;
            this.bias = bias;

            this.weights = new double[nNeurons, nInputs + Convert.ToInt32(bias)]; // TODO weight initialization
        }

        public void Propagate(double[,] inputs)
        {
            // See if we recieved the correct dimension of input.
            // if bias = true, it gets converted to 1, else it gets converted to 0.
            if (inputs.GetLength(0) != nInputs + Convert.ToInt32(bias)) throw new ArgumentException("Number of inputs does not equal the specified input size of the layer.");
            else
            {
                //if (bias) output = Extensions.DotProduct(weights, inputs.Append(1.0).ToArray()).Activate(activationType);
                //else output = Extensions.DotProduct(weights, inputs).Activate(activationType);
            }

            //inputs.
        }

        public void RandomizeWeights(Random rnd)
        {
            for (int i = 0; i < weights.Length; i++) weights[i % weights.GetLength(0), i / weights.GetLength(1)] = rnd.NextDouble();
        }
    }

    public enum ActivationFunctions
    { 
        Identity,
        Sigmoid,
        TanH,
        ReLU,
        LeakyReLU,
    }

    public static class Activations
    {
        //Unity activation function.The implementation is inefficient, but represents
        //the nature of an activation function better then "return input". Shows the
        //general implementation scheme for using LINQ to perform the calcualtion on
        //all elemnts of the array.
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

        public static double[] Activate(this double[] input, ActivationFunctions activationType)
        {
            double[] output;

            switch (activationType)
            {
                case ActivationFunctions.Identity:
                    output = input.Identity();
                    break; 
                case ActivationFunctions.Sigmoid:
                    output = input.Sigmoid();
                    break;
                case ActivationFunctions.TanH:
                    output = input.TanH();
                    break;
                case ActivationFunctions.ReLU:
                    output = input.ReLU();
                    break;
                case ActivationFunctions.LeakyReLU:
                    output = input.LReLU(0.01);
                    break;
                default:
                    output = input.Identity();
                    break;
            }

            return output;
        }

        public static double[] Derivative(this double[] input, ActivationFunctions activationType)
        {
            double[] output;

            switch (activationType)
            {
                case ActivationFunctions.Identity:
                    output = input.Identity(deriv: true);
                    break;
                case ActivationFunctions.Sigmoid:
                    output = input.Sigmoid(deriv: true);
                    break;
                case ActivationFunctions.TanH:
                    output = input.TanH(deriv: true);
                    break;
                case ActivationFunctions.ReLU:
                    output = input.ReLU(deriv: true);
                    break;
                case ActivationFunctions.LeakyReLU:
                    output = input.LReLU(0.01, deriv: true);
                    break;
                default:
                    output = input.Identity(deriv: true);
                    break;
            }

            return output;
        }
    }
}
