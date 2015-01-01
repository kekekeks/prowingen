using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Prowingen
{
	public unsafe class DummyInputStream : Stream
	{
		internal DummyInputStream ()
		{
		}

		public override int Read (byte[] buffer, int offset, int count)
		{
			System.Threading.Thread.Sleep (-1);
			return 0;
		}

		public override System.Threading.Tasks.Task<int> ReadAsync (byte[] buffer, int offset, int count, System.Threading.CancellationToken cancellationToken)
		{
			return new TaskCompletionSource<int> ().Task;
		}

		protected override void Dispose(bool disposing)
		{
		
		}


		#region Info
		public override long Length{get{return 100500;}}

		public override bool CanRead
		{
			get
			{
				return true;
			}
		}
		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}
		public override bool CanWrite
		{
			get
			{
				return false;
			}
		}


		#endregion

		#region TODO
		public override long Seek (long offset, SeekOrigin origin)
		{
			throw new NotSupportedException ();
		}


		public override long Position
		{
			get
			{
				return 0;
			}
			set
			{
				throw new NotSupportedException ();
			}
		}
		#endregion


		#region not supported
		public override void Flush ()
		{
			throw new NotSupportedException ();
		}

		public override void SetLength (long value)
		{
			throw new NotSupportedException ();
		}
		public override void Write (byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException ();
		}

		#endregion
	}
}

