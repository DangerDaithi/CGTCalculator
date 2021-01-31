using System;

namespace CGTCalculator.Processor
{
    public class CgtCalculatorProcessorException : Exception
    {
        public CgtCalculatorProcessorException(string message, Exception e) : base(message, e){}
    }
}