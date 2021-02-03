using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


/// <summary>
/// Part 1 of the NEAT ACW by 514380
/// Outputs 3494 values from a dataset of 3500
/// This decrease is to level off the number of inputs vs outputs
/// </summary>

namespace NEATPerceptron
{
    class Program
    {
        /// <summary>
        /// Inputs 
        /// 
        ///  Input(1)        Input(2)        Output      Error
        ///  0               X(1)            X_p(2)      X(2)-X_p(2)
        ///  X(1)            X(2)            X_p(3)      X(3)-X_p(3)
        ///  X(2)            X(3)            X_p(4)      x(4)-X_p(4)
        ///  
        /// </summary>

        //Holds the data from the txt file
        static double[] data = new double[3498];

        //Bias input
        static int bias = 1;

        //2D array for the inputs 
        static double[,] inputs = new double[data.Length, 2];

        //3D array for the inputs
        static double[,] inputs3I = new double[data.Length, 3];

        //1D array for the outputs
        static double[] outputs = new double[3496];

        //Array of weights
        static double[] weights = new double[4];

        //Learning rate
        static double learningRate = 0.1;

        //Error
        static double error;

        //Total count of errors from 1 epoch
        static double epochError;

        //Iteration
        static double epoch;

        //Total number of inputs
        static int dataCount = inputs.GetUpperBound(0) + 1;

        //Give the weights a random value
        static public void InitialiseWeights()
        {
            Random r = new Random();
            weights[0] = r.NextDouble();
            weights[1] = r.NextDouble();
            weights[2] = r.NextDouble();
            weights[3] = r.NextDouble();
        }

        //The expected output sign 
        static public double Output(double[] weights, double x, double y)
        {
            double sum = (x * weights[0]) + (y * weights[1]) + (bias * weights[2]);
            return (sum >= 0) ? 1 : -1;
        }

        //The expected output sigmoid for 1 input
        static public double SigOutput1I(double[] weights, double x)
        {
            double sum = (x * weights[0]) + (bias * weights[1]);
            return 1 / (1 + Math.Exp(-sum));
        }

        //The expected output sigmoid for 2 input
        static public double SigOutput2I(double[] weights, double x, double y)
        {
            double sum = (x * weights[0]) + (y * weights[1]) + (bias * weights[2]);
            return 1 / (1 + Math.Exp(-sum));
        }

        //The expected output sigmoid for 3 input
        static public double SigOutput3I(double[] weights, double x, double y, double z)
        {
            double sum = (x * weights[0]) + (y * weights[1]) + (z * weights[2]) + (bias * weights[3]);
            return 1 / (1 + Math.Exp(-sum));
        }

        //Data input from the text file into array
        static public void DataInput()
        {
            //Setup the input reader
            StreamReader input = File.OpenText("dataset.txt");

            //Set a variable to store the current vaule at X line in the file
            string line;

            //Counter for the element in the data array
            int i = 0;

            //Main while loop 
            while ((line = input.ReadLine()) != null)
            {
                //Level off the data
                if (data[3496] != 0)
                {
                    break;
                }
                else
                {
                    //Put the data into the array
                    data[i] = double.Parse(line);
                    i++;
                }
            }
        }

        //Sort the data into the 2D array
        static void SortData()
        {
            for (int i = 0; i < data.Length - 2; i++)
            {
                //Sets the first column
                inputs[i, 0] = data[i];

                //Sets the second column
                inputs[i, 1] = data[i + 1];

                //We dont want the first element to be an output so we
                //Start from the second one
                if (i != 0)
                {
                    //Sets the output array
                    outputs[i - 1] = data[i];
                }
            }
        }

        //Sort the data into the 2D array with 3 columns
        static void SortData3I()
        {
            for (int i = 0; i < data.Length - 2; i++)
            {
                if (i == 0)
                {
                    inputs3I[i, 0] = 0;
                    inputs3I[i, 1] = data[i];
                    inputs3I[i, 2] = data[i + 1];
                    i = 1;

                }
                inputs3I[i, 0] = data[i - 1];
                inputs3I[i, 1] = data[i];
                inputs3I[i, 2] = data[i + 1];

                if (i != 0 && i != 1)
                {
                    outputs[i - 2] = data[i];
                }

            }
        }

        //Main method for perceptron
        static void Main(string[] args)
        {
            //Input the data
            DataInput();

            //Sort the data into the 2D array
            SortData();

            //Sort data into 2D array with 3 columns
            SortData3I();

            //Give the weights a random value
            InitialiseWeights();

            //The number of selected inputs
            int response = 0;

            do
            {
                //Ask the user how many inputs they would like 1 - 3 
                Console.WriteLine("How many inputs would you like? 1, 2 or 3? Or press 0 to exit");

                //Check if response it the right type
                try
                {
                    //Read in the response
                    response = int.Parse(Console.ReadLine());
                }
                //Catch errors
                catch
                {
                    //Write the error to the user
                    Console.WriteLine("Please enter a number from 1 - 3");
                    Console.ReadLine();
                }

                if (response == 0)
                {
                    Environment.Exit(0);
                }

                //Switch case for the number of inputs
                switch (response)
                {
                    #region 1 Input
                    //If 1 input is selected
                    case 1:
                        //Let the user know the amount of inputs
                        Console.WriteLine("1 input selected");
                        Console.WriteLine("...Working...");
                        //Run the perceptron
                        do
                        {
                            //Set the epoch total error back to 0 after each loop through the dataset
                            epochError = 0;
                            //For each element in the dataset
                            for (int p = 0; p < dataCount - 4; p++)
                            {
                                //calculate the output
                                double output = SigOutput1I(weights, data[p]);

                                //calculate error which is the predicted result minus the actual output
                                error = outputs[p] - output;
                                //If this number is not 0, i.e is not error free, update the weights
                                if (error != 0)
                                {
                                    //Loop twice for the weights
                                    for (int i = 0; i < 2; i++)
                                    {
                                        //If the counter = 1 then we need to update the bias weight
                                        if (i == 1)
                                        {
                                            //Update the bias weight
                                            weights[i] += learningRate * error * bias;
                                        }
                                        else
                                        {
                                            //Update the weights
                                            weights[i] += learningRate * error * data[p];
                                        }

                                    }
                                }
                                //Calculate all of the errors for 1 run of the dataset 
                                epochError += error;
                            }
                            //Increment for every loop of the dataset
                            epoch++;
                        }

                        //Check to see if the epocherror ^ 2 is greater than 10-^11
                        //If it is then we need to run through again
                        while (Math.Pow(epochError, 2) > 0.00000000001);

                        //Set up writing to a txt file
                        StreamWriter write = new StreamWriter("ouputdata.txt");

                        //Output the data once training has finished
                        for (int i = 0; i < dataCount - 4; i++)
                        {
                            //Calculate the outputs again
                            double output = SigOutput1I(weights, data[i]);

                            //Write the outputs to the console
                            Console.WriteLine(outputs[i] + "    " + output);

                            //Write the outputsto the txt file
                            write.WriteLine(output);
                        }

                        //Close the writer
                        write.Close();

                        //Display the results
                        Console.WriteLine("Epoch: " + epoch + "  " + "Inputs: " + inputs.Length / 2);
                        //    Console.ReadLine();
                        response = 4;
                        break;
                    #endregion

                    #region 2 Inputs
                    case 2:
                        Console.WriteLine("2 inputs selected");
                        Console.WriteLine("...Working...");
                        do
                        {
                            //Set the epoch total error back to 0 after each loop through the dataset
                            epochError = 0;
                            //For each element in the dataset
                            for (int p = 0; p < dataCount - 4; p++)
                            {
                                //calculate the output
                                double output2I = SigOutput2I(weights, inputs[p, 0], inputs[p, 1]);

                                //calculate error which is the predicted result minus the actual output
                                error = outputs[p] - output2I;
                                //If this number is not 0, i.e is not error free, update the weights
                                if (error != 0)
                                {
                                    //Loop three times for the weights
                                    for (int i = 0; i < 3; i++)
                                    {
                                        //If the counter = 2 then we need to update the bias weight
                                        if (i == 2)
                                        {
                                            //Update the bias weight
                                            weights[i] += learningRate * error * bias;
                                        }
                                        else
                                        {
                                            //Update the weights
                                            weights[i] += learningRate * error * inputs[p, i];
                                        }

                                    }
                                }
                                //Calculate all of the errors for 1 run of the dataset 
                                epochError += error;
                            }
                            //Increment for every loop of the dataset
                            epoch++;
                        }

                        //Check to see if the epocherror ^ 2 is greater than 10-^10
                        //If it is then we need to run through again
                        while (Math.Pow(epochError, 2) > 0.00000000001);

                        //Set up writing to a txt file
                        StreamWriter write2 = new StreamWriter("ouputdata.txt");

                        //Output the data once training has finished
                        for (int i = 0; i < dataCount - 4; i++)
                        {
                            //Calculate the outputs again
                            double output = SigOutput2I(weights, inputs[i, 0], inputs[i, 1]);

                            //Write the outputs to the console
                            Console.WriteLine(outputs[i] + "    " + output);

                            //Write the outputsto the txt file
                            write2.WriteLine(output);
                        }

                        //Close the writer
                        write2.Close();

                        //Display the results
                        Console.WriteLine("Epoch: " + epoch + "  " + "Inputs: " + inputs.Length / 2);
                        //   Console.ReadLine();
                        response = 4;
                        break;
                    #endregion

                    #region 3 Inputs
                    case 3:
                        Console.WriteLine("3 inputs selected");
                        Console.WriteLine("...Working...");
                        do
                        {
                            //Set the epoch total error back to 0 after each loop through the dataset
                            epochError = 0;
                            //For each element in the dataset
                            for (int p = 0; p < dataCount - 4; p++)
                            {
                                //calculate the output
                                double output3I = SigOutput3I(weights, inputs3I[p, 0], inputs3I[p, 1], inputs3I[p, 2]);

                                //calculate error which is the predicted result minus the actual output
                                error = outputs[p] - output3I;
                                //If this number is not 0, i.e is not error free, update the weights
                                if (error != 0)
                                {
                                    //Loop three four for the weights
                                    for (int i = 0; i < 4; i++)
                                    {
                                        //If the counter = 3 then we need to update the bias weight
                                        if (i == 3)
                                        {
                                            //Update the bias weight
                                            weights[i] += learningRate * error * bias;
                                        }
                                        else
                                        {
                                            //Update the weights
                                            weights[i] += learningRate * error * inputs3I[p, i];
                                        }

                                    }
                                }
                                //Calculate all of the errors for 1 run of the dataset 
                                epochError += error;
                            }
                            //Increment for every loop of the dataset
                            epoch++;
                            //Console.Write(epoch + "               " + Math.Pow(epochError, 2));
                        }

                        //Check to see if the epocherror ^ 2 is greater than 10-^10
                        //If it is then we need to run through again
                        while (Math.Pow(epochError, 2) > 0);

                        //Set up writing to a txt file
                        StreamWriter write3 = new StreamWriter("ouputdata.txt");
                        
                        //Output the data once training has finished
                        for (int i = 0; i < dataCount - 4; i++)
                        {
                            //Calculate the outputs again
                            double output = SigOutput3I(weights, inputs3I[i, 0], inputs3I[i, 1], inputs3I[i, 2]);

                            //Write the outputs to the console
                            Console.WriteLine(outputs[i] + "    " + output);

                            //Write the outputsto the txt file
                            write3.WriteLine(output);
                        }

                        //Close the writer
                        write3.Close();

                        //Display the results
                        Console.WriteLine("Epoch: " + epoch + "  " + "Inputs: " + inputs.Length / 2);
                        // Console.ReadLine();
                        response = 4;
                        break;
                    #endregion

                    default:
                        break;
                }
            }
            while (true);
        }
    }
}

