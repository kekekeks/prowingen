using System;

namespace Prowingen
{
	public unsafe class Response
	{
		static readonly IResponseWrapper Wrapper = Prowingen.Factory.Native.Value.CreateResponseWrapper();
		IntPtr _native;
		internal Response(IntPtr native)
		{
			_native = native;
		}

		public void SetCode(int code, string status)
		{
			Check ();
			Wrapper.SetCode (_native, (ushort)code, status);
		}

		public void AppendHeader(string key, string value)
		{
			Check ();
			Wrapper.AppendHeader (_native, key, value);
		}

		public void AppendBody(IntPtr data, int size, bool flush)
		{
			Check ();
			Wrapper.AppendBody (_native, data, size, flush);
		}

		public void AppendBody(byte[] data, int offset, int size, bool flush = false)
		{
			fixed(byte* sptr = data)
			{
				byte* ptr = sptr;
				ptr += offset;
				AppendBody (new IntPtr((void*)ptr), size, flush);
			}
		}

		public void AppendBody(byte[] data, bool flush = false)
		{
			AppendBody (data, 0, data.Length, flush);
		}

		void Check()
		{
			if (_native == IntPtr.Zero)
				throw new InvalidOperationException ("Request is already completed");
		}

		public void Complete()
		{
			Wrapper.Complete (_native);
			_native = IntPtr.Zero;
		}


	}
}

