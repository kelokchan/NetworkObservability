using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkObservability
{
	namespace Core
	{
		class BiDirectionalConnectionProperty : IProperty
		{
			public bool blockLeftFlow
			{
				get; set;
			}

			public bool blockRightFlow
			{
				get; set;
			}

			public int Weight
			{
				get; set;
			}

			public int CompareTo(IProperty property)
			{
				throw new NotImplementedException();
			}
		}
	}
}
