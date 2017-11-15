using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkObservability
{
	namespace Core
	{
		class ExtendableArc : Arc
		{
			Dictionary<string, IProperty> properties;
			private IProperty property;

			public ExtendableArc() : base()
			{
				properties = new Dictionary<string, IProperty>();
			}

			public void AddProperty(string key, IProperty property)
			{
				properties[key] = property;
			}

			public bool ContainsProperty(string key)
			{
				return properties.ContainsKey(key);
			}

			public int CompareProperty(string key, IProperty property)
			{
				if (ContainsProperty(key))
					return properties[key].CompareTo(property);
				else
					throw new PropertyNotFOundException();
			}

			public int CompareProperty(string key, IProperty property, IComparer<IProperty> comparer)
			{
				if (ContainsProperty(key))
					return comparer.Compare(properties[key], property);
				else
					throw new PropertyNotFOundException();
			}

		}

	}
}
