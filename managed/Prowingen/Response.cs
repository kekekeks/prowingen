using System;
using System.IO;
using System.Threading.Tasks;

namespace Prowingen
{
	public unsafe class Response
	{
		static readonly IResponseWrapper Wrapper = Prowingen.Factory.Native.Value.CreateResponseWrapper();
		IntPtr _native;

		public event EventHandler HeadersSent;
		public Stream OutputStream { get; private set;}
		public bool HeadersAreSent {get; private set;}


		internal Response(IntPtr native)
		{
			_native = native;
			OutputStream = new ProwingenResponseStream (this);
		}

		public void SetCode(int code, string status)
		{
			OnHeader ();
			Wrapper.SetCode (_native, (ushort)code, status);
		}

		void OnWrite()
		{
			Check ();
			bool headersWereSent = HeadersAreSent;
			HeadersAreSent = true;
			if (!headersWereSent && HeadersSent != null)
				HeadersSent (this, new EventArgs ());

		}

		void OnHeader()
		{
			Check ();
			if (HeadersAreSent)
				throw new InvalidOperationException ("Headers are already sent");
		}

		public void AppendHeader(string key, string value)
		{
			OnHeader ();
			Wrapper.AppendHeader (_native, key, value);
		}

		void AppendBody(byte[] data, int offset, int size, bool flush = false)
		{
			OnWrite ();
			fixed(byte* sptr = data)
			{
				byte* ptr = sptr;
				ptr += offset;
				Wrapper.AppendBody(_native, new IntPtr((void*)ptr), size, flush);
			}
		}

		void Check()
		{
			if (_native == IntPtr.Zero)
				throw new ObjectDisposedException ("Request is already completed");
		}

		void Complete()
		{
			if (_native != IntPtr.Zero)
				Wrapper.Complete (_native);
			_native = IntPtr.Zero;
		}



		class ProwingenResponseStream : Stream
		{
			Response _parent;

			public ProwingenResponseStream (Response parent)
			{
				_parent = parent;
			}

			static byte[] empty;
			public override void Flush ()
			{
				_parent.AppendBody (empty, 0, 0, true);
			}

			public override void Write (byte[] buffer, int offset, int count)
			{
				if (offset < 0 || offset + count >= buffer.Length)
					throw new IndexOutOfRangeException ();
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
}

