using System;

namespace Prowingen
{
	public unsafe class Response
	{
		static readonly IResponseWrapper Wrapper = Prowingen.Factory.Native.Value.CreateResponseWrapper();
		readonly IntPtr _native;
		internal Response(IntPtr native)
		{
			_native = native;
		}

		public void SetCode(int code, string status)
		{
			Wrapper.SetCode (_native, code, status);
		}

		public void AppendHeader(string key, string value)
		{
			Wrapper.AppendHeader (_native, key, value);
		}

		public void AppendBody(IntPtr data, int size, bool flush)
		{
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

		public void Complete()
		{
			Wrapper.Complete (_native);
		}


	}
}

