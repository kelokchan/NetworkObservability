using System;
using System.Runtime.Serialization;

namespace NetworkObservability
{
    namespace Core
    {
        [Serializable]
        public class OutOFNodeInterfacesException : Exception
        {
            public OutOFNodeInterfacesException()
            {
            }

            public OutOFNodeInterfacesException(string message) : base(message)
            {
            }

            public OutOFNodeInterfacesException(string message, Exception innerException) : base(message, innerException)
            {
            }

            protected OutOFNodeInterfacesException(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }
        }
    }
}