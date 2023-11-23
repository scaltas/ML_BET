using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootballMatchPrediction.Core.Services.DataScience.Prediction
{
    public interface IPredictorServiceFactory
    {
        IPredictorService CreatePredictorService(string algorithm);
    }

    public class PredictorServiceFactory : IPredictorServiceFactory
    {
        public IPredictorService CreatePredictorService(string algorithm)
        {
            switch (algorithm)
            {
                case "LinearRegression":
                    return new LinearRegressionPredictorService();
                default:
                    throw new ArgumentException($"Unsupported algorithm: {algorithm}");
            }
        }
    }
}
