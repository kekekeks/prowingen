using System;

namespace Prowingen
{
	public static unsafe class ProwingenExtensions
	{

		public static void AppendBody(this IResponse response, byte[] data, int offset, int size, bool flush = false)
		{
			fixed(byte* sptr = data)
			{
				byte* ptr = sptr;
				ptr += offset;
				response.AppendBody (new IntPtr((void*)ptr), size, flush);
			}
		}

		public static void AppendBody(this IResponse response, byte[] data, bool flush = false)
		{
			response.AppendBody (data, 0, data.Length, flush);
		}
	}
}

