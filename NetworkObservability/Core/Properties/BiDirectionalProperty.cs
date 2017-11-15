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
				var temp = other as BiDirectionalProperty;
				if (LeftFlowBlocked == temp.LeftFlowBlocked && RightFLowBlocked == temp.RightFLowBlocked)
					return 0;
				else if (LeftFlowBlocked == temp.LeftFlowBlocked && RightFLowBlocked != temp.RightFLowBlocked)
					return 1;
				else
					return -1;
			}
			else
			{
				throw new NotSupportedException();
			}
		}
	}
}
