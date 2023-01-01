using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perimeter
{
    class CLI
    {
        bool active;
        Server server;
        public CLI(Server server)
        {
            this.server = server;
        }

        public void Start()
        {
            string input;
            active = true;
            Console.WriteLine("Welcome to Sensory. Please enter your input:\n\tsensor - show all sensors data\n\tsensor <id> - show data for specific sensor\n\texit - exit the program");
            while (active)
            {
                input = Console.ReadLine();
                ParseCommand(input);
            }
        }

        private void ParseCommand(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                return;
            }
            string[] cmdArgs = command.Split(' ');

            switch (cmdArgs[0])
            {
                case "sensor":
                    int id;
                    if (cmdArgs.Length > 1 && int.TryParse(cmdArgs[1], out id))
                    {
                        if (cmdArgs.Length > 2)
                        {
                            Console.WriteLine("Invalid input");
                            break;
                        }
                        Console.WriteLine(ShowSensorData(id));
                    }
                    else
                    {
                        Console.WriteLine(ShowAllSensorsData());
                    }
                    break;
                case "exit":
                    if (cmdArgs.Length > 1)
                    {
                        Console.WriteLine("Invalid input");
                    }
                    else
                    {
                        Exit();
                    }
                    break;
                default:
                    Console.WriteLine("Invalid input");
                    break;
            }
            
        }

        public string ShowSensorData(int id)
        {
            float max = server.GetMaxTemperature(id);
            float min = server.GetMinTemperature(id);
            float average = server.GetAverageTemperature(id);

            if (float.IsNaN(max) || float.IsNaN(min) || float.IsNaN(average))
            {
                return "No available data for sensor " + id;
            }

            return string.Format("Data for sensor {0}\nMax: {1}, Min: {2}, Average: {3}", id, max, min, average);
        }

        public string ShowAllSensorsData()
        {
            float max = server.GetMaxTemperature();
            float min = server.GetMinTemperature();
            float average = server.GetAverageTemperature();

            if (float.IsNaN(max) || float.IsNaN(min) || float.IsNaN(average))
            {
                return "No available data for any of the sensors";
            }

            return string.Format("Max: {0}, Min: {1}, Average: {2}", max, min, average);
        }

        public void Exit()
        {
            active = false;
        }
    }
}
