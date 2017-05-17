using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkObservability.Core.Comparers
{
	class BidirectionalComparer : Comparer<IProperty>
	{
		public bool IsLeftFlowBlocked
		{
			get; set;
		}
		public bool IsRightFlowBlocked
		{
			get; set;
		}

		public int Compare(IProperty lhs, IProperty rhs)
		{
			lhs as 
		}
	}
}
