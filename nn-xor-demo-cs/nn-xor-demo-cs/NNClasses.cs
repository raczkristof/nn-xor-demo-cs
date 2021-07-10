using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nn_xor_demo_cs
{
    public class NNModel
    {
        private FCLayer[] layers;
        private int nInputs;

        public NNModel(int nInputs, int[] layerSizes, ActivationFunctions[] layerActivations)
        {
            if (layerSizes.Length != layerActivations.Length)
            {
                throw new ArgumentException("Number of layers does not match number of activations specified.");
            }
            else
            {
                this.layers = new FCLayer[layerSizes.Length];
                this.nInputs = nInputs;

                this.layers[0] = new FCLayer(layerSizes[0], nInputs, layerActivations[0]);
                for (int i = 1; i < layerSizes.Length; i++) this.layers[i] = new FCLayer(layerSizes[i], layerSizes[i - 1], layerActivations[i]);
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
                this.layers = new FCLayer[layerSizes.Length];
                this.nInputs = nInputs;

                this.layers[0] = new FCLayer(layerSizes[0], nInputs, layerActivations[0]);
                for (int i = 1; i < layerSizes.Length; i++) this.layers[i] = new FCLayer(layerSizes[i], layerSizes[i - 1], layerActivations[i]);
                this.ResetWeights(seed);
            }
        }

        // Predict the label based on the inputs
        public double[,] Predict(double[,] inputs)
        {
            double[,] output = layers[0].Propagate(inputs);
            for (int i = 1; i < layers.Length; i++)
            {
                output = layers[i].Propagate(output);
            }

            return output;
        }

        public void BackProp(double[,] trueLabels, double lr)
        {
            double[,] predLabels = layers.Last().GetOutput();
            double cost = Extensions.BinaryCrossEntropy(trueLabels, predLabels);

            double[,] err = new double[trueLabels.GetLength(0), trueLabels.GetLength(1)];
            for (int i = 0; i < err.GetLength(0); i++)
                for (int j = 0; j < err.GetLength(1); j++)
                    err[i, j] = (trueLabels[i, j] / predLabels[i, j] - (1 - trueLabels[i, j]) / (1 - predLabels[i, j])) * (-1);

            for (int i = layers.Length - 1; i >= 0; i--)
            {
                err = layers[i].BackPropagate(err, lr);
            }
        }

        // Randomize weights (between 1 and 0)
        public void ResetWeights()
        {
            Random rnd = new Random();
            foreach (FCLayer layer in layers) layer.RandomizeWeights(rnd);
        }

        public void ResetWeights(int seed)
        {
            Random rnd = new Random(seed);
            foreach (FCLayer layer in layers) layer.RandomizeWeights(rnd);
        }

    }

    public class FCLayer
    {
        public int nInputs;
        public double[,] weights;
        public double[,] inputs;

        public ActivationFunctions activationType;
        public bool bias;

        public FCLayer(int nNeurons, int nInputs, ActivationFunctions activationType, bool bias=true)
        {
            this.nInputs = nInputs;
            this.activationType = activationType;
            this.bias = bias;

            this.weights = new double[nInputs + Convert.ToInt32(bias), nNeurons];
        }

        public double[,] Propagate(double[,] inputs)
        {
            // See if we recieved the correct dimension of input.
            // if bias = true, it gets converted to 1, else it gets converted to 0.
            if (inputs.GetLength(1) != nInputs) throw new ArgumentException("Number of inputs does not equal the specified input size of the layer.");
            else
            {
                this.inputs = inputs;
                if (bias) this.inputs = inputs.AppendColOnes();
                else this.inputs = inputs;

                return Extensions.DotProduct(this.inputs, weights).Activate(activationType);
            }
        }

        public double[,] BackPropagate(double[,] layerErr, double lr)
        {
            double[,] dactivation = Extensions.DotProduct(inputs, weights).Derivative(activationType);
            double[,] delta = Extensions.Multiply(layerErr, dactivation);

            double[,] deltaW = Extensions.DotProduct(inputs.Transpose(), delta);

            layerErr = Extensions.DotProduct(delta, weights.Transpose());
            if (bias) // If there's bias, the term going to the bias neuron needs to be removed
            {
                double[,] tmp2 = layerErr;
                layerErr = new double[tmp2.GetLength(0), tmp2.GetLength(1) - 1];
                for (int i = 0; i < layerErr.GetLength(0); i++)
                    for (int j = 0; j < layerErr.GetLength(1); j++)
                        layerErr[i, j] = tmp2[i, j];
            }

            for (int i = 0; i < weights.GetLength(0); i++)
                for (int j = 0; j < weights.GetLength(1); j++)
                    weights[i, j] -= lr * deltaW[i, j];

            return layerErr;
        }

        public double[,] GetOutput()
        {
                return Extensions.DotProduct(inputs, weights).Activate(activationType); ;
        }

        public void RandomizeWeights(Random rnd)
        {
            for (int i = 0; i < weights.Length; i++) weights[i % weights.GetLength(0), i / weights.GetLength(0)] = rnd.NextDouble() * 2 -1;
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
        //the nature of an activation function better then "return input".
        public static double[,] Identity(this double[,] input, bool deriv = false)
        {
            double[,] output = new double[input.GetLength(0), input.GetLength(1)];

            for (int i = 0; i < input.GetLength(0); i++)
                for (int j = 0; j < input.GetLength(1); j++)
                {
                    if (deriv)
                        output[i, j] = 1.0;
                    else
                        output[i, j] = input[i, j];
                }

            return output;
        }

        public static double[,] Sigmoid(this double[,] input, bool deriv = false)
        {
            double[,] output = new double[input.GetLength(0), input.GetLength(1)];

            for (int i = 0; i < input.GetLength(0); i++)
                for (int j = 0; j < input.GetLength(1); j++)
                {
                    if (deriv)
                        output[i, j] = Math.Exp(-input[i, j]) / Math.Pow(1 + Math.Exp(-input[i, j]), 2);
                    else
                        output[i, j] = 1 / (1 + Math.Exp(-input[i, j]));
                }

            return output;
        }

        public static double[,] TanH(this double[,] input, bool deriv = false)
        {
            double[,] output = new double[input.GetLength(0), input.GetLength(1)];

            for (int i = 0; i < input.GetLength(0); i++)
                for (int j = 0; j < input.GetLength(1); j++)
                {
                    if (deriv)
                        output[i, j] = 1 - Math.Tanh(input[i, j]) * Math.Tanh(input[i, j]);
                    else
                        output[i, j] = Math.Tanh(input[i, j]);
                }

            return output;
        }

        public static double[,] ReLU(this double[,] input, bool deriv = false)
        {
            double[,] output = new double[input.GetLength(0), input.GetLength(1)];

            for (int i = 0; i < input.GetLength(0); i++)
                for (int j = 0; j < input.GetLength(1); j++)
                {
                    if (input[i, j] < 0) output[i, j] = 0;
                    else
                    {
                        if (deriv)
                            output[i, j] = 1.0;
                        else
                            output[i, j] = input[i, j];
                    }
                }

            return output;
        }

        public static double[,] LReLU(this double[,] input, double alpha, bool deriv = false)
        {
            double[,] output = new double[input.GetLength(0), input.GetLength(1)];

            for (int i = 0; i < input.GetLength(0); i++)
                for (int j = 0; j < input.GetLength(1); j++)
                {
                    if (input[i, j] < 0)
                    {
                        if (deriv)
                            output[i, j] = alpha;
                        else
                            output[i, j] = alpha * input[i, j];
                    }
                    else
                    {
                        if (deriv)
                            output[i, j] = 1.0;
                        else
                            output[i, j] = input[i, j];
                    }
                }

            return output;
        }

        public static double[,] Activate(this double[,] input, ActivationFunctions activationType)
        {
            double[,] output;

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

        public static double[,] Derivative(this double[,] input, ActivationFunctions activationType)
        {
            double[,] output;

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
