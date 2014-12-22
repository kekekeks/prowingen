using System;
using System.Collections.Generic;

namespace Prowingen
{
	public class ProwingenResponseHeaders : IEnumerable<KeyValuePair<string, IEnumerable<string>>>
	{
		internal Dictionary<string, List<string>> Dictionary = new Dictionary<string, List<string>>();

		Response _parent;

		public ProwingenResponseHeaders (Response parent)
		{
			_parent = parent;
		}

		List<string> GetList(string header)
		{
			List<string> rv;
			if (Dictionary.TryGetValue (header, out rv))
				return rv;
			return Dictionary [header] = new List<string> ();
		}

		public void Add(string header, string value)
		{
			_parent.OnHeader ();
			GetList (header).Add (value);
		}

		public void Add(string header, IEnumerable<string> values)
		{
			_parent.OnHeader ();
			GetList (header).AddRange (values);
		}

		public void Clear()
		{
			_parent.OnHeader ();
			Dictionary.Clear ();
		}

		public void Remove(string header)
		{
			_parent.OnHeader ();
			Dictionary.Remove (header);
		}


		#region IEnumerable implementation
		public IEnumerator<KeyValuePair<string, IEnumerable<string>>> GetEnumerator ()
		{
			foreach (var kp in Dictionary)
				yield return new KeyValuePair<string, IEnumerable<string>> (kp.Key, kp.Value);
		}
		#endregion
		#region IEnumerable implementation
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
		{
			return GetEnumerator ();
		}
		#endregion
	}
}

