using System;

namespace BLToolkit.Mapping
{
	public class MapValue
	{
		public MapValue(object origValue, params object[] mapValues)
		{
			_origValue = origValue;
			_mapValues = mapValues;
		}

		private object _origValue;
		public  object  OrigValue
		{
			get { return _origValue;  }
		}

		private object[] _mapValues;
		public  object[]  MapValues
		{
			get { return _mapValues;  }
		}
	}
}
