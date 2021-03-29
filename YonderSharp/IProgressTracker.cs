using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YonderSharp
{
    public interface IProgressTracker
    {
        /// <summary>
        /// Notify about some progress. i.e. 5/100
        /// </summary>
        /// <param name="currentStep">Which step was just finish?</param>
        /// <param name="totalStepCount">Total steps to finish</param>
        void Progress(int currentStep, int totalStepCount);
    }
}
