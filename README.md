# nn-xor-demo-cs
Implementation of a simple feed-forward neural network for solving the XOR problem in C#. It is purely for the purpose of refreshing my basic knowledge about neural networks and back propagation. It is not meant to be a fast/optimal implementation. The program uses a console and plotting window as the front end. It is based on the .Net Framework (mainly because newer versions does not seem to support the chart control anymore).

## Implementation
Having a console and a forms window at the same time can be achieved by creating a `Windows Forms App` in VS, and setting **Project** (right click on the project in the solution explorer) **-> Properties -> Application -> Output type** to `Console Application`

The project contains 3 main files:
- **Main.cs**: front end of the project, handling console inputs and general run of the program
- **Extensions.cs**: contains utility functions  such as dot product of 2D arrays
- **NNClasses.cs**: 'back end', contains code related to the NN, such as forward and back propagation and activation functions

### Console commands
The program can be interracted with via the console, with the following available commands:
- `plot(*)`: plots the desired information in the plot window, where * can be:
	-`ident`/`ident.deriv`: identity activation function or it\'s derivative
	-`sigmoid`/`sigmoid.deriv`: sigmoid activation function or it\'s derivative
	-`tanh`/`tanh.deriv`: TanH activation function or it\'s derivative
	-`relu`/`relu.deriv`: ReLU activation function or it\'s derivative
	-`lrelu`/`lrelu.deriv`: Leaky ReLU activation function or it\'s derivative (with α = 0.5)
	
	-`trainData`: the generated training data with it\'s labels (also generates the training data if it has not been generated yet)
	-`predictData`: the predictions of the Neural Net for the training Data (also generates the training data if it has not been generated yet)

- `testDotProduct`: test the implementation of the dot product extenstions
- `resetWeights`: reinitialize the weights of the current network
- `trainOne`: train the current network for one epoch (also generates the training data if it has not been generated yet)
- `train`: train the current network on the training data (also generates the training data if it has not been generated yet)
- `exit`: closes the program

### NN and training configuration
The training parameters can be changed via the constants at the beginning (lines 27-30) of `Main.cs`:
- `TRAININGDATANUM`: number of datapoint to generate for the training data
- `TRAININGEPOCHS`: number of epochs to train for
- `FEEDBACKEPOCHS`: number of epochs after feedback is given about the current state of training
- `LR`: learning rate

The configuration of the NN can be changed by giving an array of ints containing the number of neurons in each hidden layer, and an array of activation function names for each hidden layer. The last layer needs to be sigmoid or TanH for correct operation (the demo uses the Binary Crossentropy cost function). A random seed can be added here for debugging purposes.

### NN implementation
The implemented NN is a fully connected feed-forward neural network. It might be possible to extend the implementation to be able to use other types of layers. Layers automatically use bias, you'll need to edit the constructor of the `NNModel` class. Aside from that, everything should still work with bias turned off. The available activation functions for the layers are:
- Identity
- Sigmoid
- TanH
- ReLU
- Leaky ReLU
