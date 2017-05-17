using System;
using System.Runtime.Serialization;

namespace NetworkObservability.Core
{
	[Serializable]
	internal class PropertyNotFOundException : Exception
	{
		public PropertyNotFOundException()
		{
		}

		public PropertyNotFOundException(string message) : base(message)
		{
		}

		public PropertyNotFOundException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected PropertyNotFOundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}