using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkObservabilityCore;

namespace NetworkObservability
{
	public class CanvasGraphXML : GraphXML
	{
		#region Constructors
		public CanvasGraphXML() 
			: base()
		{
		}

		public CanvasGraphXML(String version, String encoding, String standalone) 
			: base(version, encoding, standalone)
		{
		}
		#endregion



	}
}
