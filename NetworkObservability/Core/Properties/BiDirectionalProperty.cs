using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkObservability.Core.Properties
{
	class BiDirectionalProperty : IProperty
	{
		public bool LeftFlowBlocked
		{
			get; set;
		}

		public bool RightFLowBlocked
		{
			get; set;
		}

		int IComparable<IProperty>.CompareTo(IProperty other)
		{
			if (other is BiDirectionalProperty)
			{
				if 
			}
		}
	}
}
