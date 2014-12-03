using System;

namespace Prowingen
{
	public class Request : IDisposable
	{
		static readonly IRequestWrapper Wrapper = Prowingen.Factory.Native.Value.CreateRequestWrapper();
		IntPtr _native;

		public Request (IntPtr native)
		{
			_native = native;
		}

		public void Dispose ()
		{
			if (_native != IntPtr.Zero)
			{
				Wrapper.Dispose (_native);
				_native = IntPtr.Zero;
			}
		}
	}
}

