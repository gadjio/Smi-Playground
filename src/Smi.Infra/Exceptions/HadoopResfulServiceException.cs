namespace Smi.Infra.Exceptions
{
	using System;

	public class HadoopResfulServiceException : Exception
    {
        public HadoopResfulServiceException(string message) : base(message)
        {
        }
    }
}
