using System;
using System.Runtime.Serialization;

namespace NetworkObservability.core
{
    [Serializable]
    internal class OutOFNodeInterfaces : Exception
    {
        public OutOFNodeInterfaces()
        {
        }

        public OutOFNodeInterfaces(string message) : base(message)
        {
        }

        public OutOFNodeInterfaces(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected OutOFNodeInterfaces(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}