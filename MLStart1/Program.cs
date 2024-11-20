using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Serilog;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace MLStart1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var filename = "stage.json";

            if (!File.Exists(filename))
            {
                var content =
                @"{
                    ""FirstName"": ""Артём"",
                    ""LastName"": ""Урыкин""
                }";
                File.WriteAllText(filename, content);
            }

            IConfiguration configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile(filename).Build();
            string firstName = configuration["FirstName"];
            string lastName = configuration["LastName"];
            int N = firstName.Length;
            int L = lastName.Length;

            int[] oddNumbers = new int[] { 5, 7, 9, 11, 13, 15, 17, 19 };
            double[,] k = new double[8, 13];
            double[] randomValues = new double[13];
            Random random = new Random();

            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("Logs/logs.txt", rollingInterval: RollingInterval.Day)
                .WriteTo.File("Logs/trace.log", restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Verbose)
                .WriteTo.File("Logs/debug.log", restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Debug)
                .WriteTo.File("Logs/info.log", restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information)
                .WriteTo.File("Logs/warning.log", restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Warning)
                .WriteTo.File("Logs/error.log", restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error)
                .WriteTo.File("Logs/critical.log", restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Fatal)
                .CreateLogger();

            var logger = Log.ForContext<Program>();

            logger.Information("Программа запущена.");

            for (int i = 0; i < randomValues.Length; i++)
            {
                randomValues[i] = Math.Round(random.NextDouble() * 27.0 - 12.0, 2);
            }

            for (int i = 0; i < oddNumbers.Length; i++)
            {
                for (int j = 0; j < randomValues.Length; j++)
                {
                    double x = randomValues[j];
                    if (oddNumbers[i] == 9)
                    {
                        k[i, j] = Math.Sin(Math.Sin(Math.Pow((x / (x + 0.5)), x)));
                        logger.Verbose($"Verbose: k[{i}, {j}] = {k[i, j]}");
                    }
                    else if (oddNumbers[i] == 5 || oddNumbers[i] == 7 || oddNumbers[i] == 11 || oddNumbers[i] == 15)
                    {
                        k[i, j] = Math.Pow((0.5 / Math.Tan(2 * x) + (2 / 3)), Math.Pow(x, 1 / 9));
                        logger.Debug($"Debug: k[{i}, {j}] = {k[i, j]}");
                    }
                    else
                    {
                        k[i, j] = Math.Pow(Math.Tan(((Math.Exp(1 - x / Math.PI)) / 3) / 4), 3);
                        logger.Information($"Info: k[{i}, {j}] = {k[i, j]}");
                    }
                }
            }

            int I = N % 6;
            int J = L % 13;

            double minValue = k[I, 0];
            logger.Information($"Minimal element at k[{I}, 0] = {minValue}");

            for (int col = 0; col < oddNumbers.Length; col++)
            {
                if (k[I, col] < minValue)
                {
                    minValue = k[I, col];
                }
            }

            double sum = 0;
            for (int row = 0; row < k.GetLength(0); row++)
            {
                sum += k[row, J];
            }
            double avgValue = sum / k.GetLength(0);
            double result = avgValue + minValue;
            Console.WriteLine($"Результат: {result}");
            logger.Information($"Результат вычислений: {result}");

            logger.Information("Программа завершена.");

            Log.CloseAndFlush();
        }
    }
}