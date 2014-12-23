using System;
using System.IO;
using System.Threading.Tasks;

namespace Prowingen
{
	class ProwingenResponseStream : Stream
	{
		Response _parent;

		public ProwingenResponseStream (Response parent)
		{
			_parent = parent;
		}
			
		public override void Flush ()
		{
			_parent.AppendBody (null, 0, 0, true);
		}

		public override void Write (byte[] buffer, int offset, int count)
		{
			if (offset < 0 || offset + count > buffer.Length)
				throw new IndexOutOfRangeException ();
			if (count < 0)
				throw new ArgumentOutOfRangeException ("count");
			if (count == 0)
				return;
			_parent.AppendBody (buffer, offset, count);
		}

		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);
			_parent.Complete ();
		}

		#region Async
		public override IAsyncResult BeginWrite (byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			Write (buffer, offset, count);
			var tcs = new TaskCompletionSource<int> (state);
			tcs.SetResult (0);

			callback (tcs.Task);
			return tcs.Task;
		}

		public override System.Threading.Tasks.Task FlushAsync (System.Threading.CancellationToken cancellationToken)
		{
			if(cancellationToken.IsCancellationRequested)
			{
				var tcs = new TaskCompletionSource<int> ();
				tcs.SetCanceled ();
				return tcs.Task;
			}
			Flush();
			return Task.FromResult (0);
		}

		public override Task WriteAsync (byte[] buffer, int offset, int count, System.Threading.CancellationToken cancellationToken)
		{
			if(cancellationToken.IsCancellationRequested)
			{
				var tcs = new TaskCompletionSource<int> ();
				tcs.SetCanceled ();
				return tcs.Task;
			}
			Write (buffer, offset, count);
			return Task.FromResult (0);
		}

		#endregion


		#region Info
		public override bool CanRead
		{
			get
			{
				return false;
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
				return true;
			}
		}
		#endregion
		#region NotSupported
		public override int Read (byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException ();
		}

		public override long Seek (long offset, SeekOrigin origin)
		{
			throw new NotSupportedException ();
		}

		public override void SetLength (long value)
		{
			throw new NotSupportedException ();
		}

		public override long Length
		{
			get
			{
				throw new NotSupportedException ();
			}
		}

		public override long Position
		{
			get
			{
				throw new NotSupportedException ();
			}
			set
			{
				throw new NotSupportedException ();
			}
		}

		#endregion


	}

}

