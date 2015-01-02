using System;
using System.Runtime.InteropServices;
using System.IO;

namespace Prowingen
{
	public unsafe class ProwingenOpaqueInputStream : Stream
	{
		static readonly Wrappers Wrappers = Prowingen.Factory.Wrappers.Value;
		#pragma warning disable 0414
		private GCHandle _cbHandle;
		#pragma warning restore 0414
		private readonly PipeStream _pipeStream = new PipeStream ();
		bool _disposed;
		IntPtr _native;

		unsafe void OnBuffer (int count, IoBufInfo* buffers)
		{
			ulong size = 0;
			for (var c = 0; c < count; c++)
				size += buffers [c].Size;
			var buffer = new byte[size];
			int position = 0;
			for (var c = 0; c < count; c++)
				Marshal.Copy (new IntPtr ((void*)buffers [c].Data), buffer, position, (int)buffers [c].Size);
			_pipeStream.Input.AppendOwnedBuffer (buffer);
		}


		public ProwingenOpaqueInputStream (IntPtr request)
		{
			var handler = new OpaqueInputStreamHandlerDelegate (OnBuffer);
			_cbHandle = GCHandle.Alloc (handler);
			var pFunc = Marshal.GetFunctionPointerForDelegate (handler);
			_native = Wrappers.UpgradeToOpaqueInputStream (request, pFunc);
		}

		protected override void Dispose (bool disposing)
		{
			if (!_disposed)
			{
				_pipeStream.Input.Dispose ();
				Wrappers.DisposeOpaqueInputStream (_native);
				_disposed = true;
			}
			base.Dispose (disposing);
		}

		#region implemented abstract members of Stream

		public override int Read (byte[] buffer, int offset, int count)
		{
			return _pipeStream.Output.Read (buffer, offset, count);
		}

		public override System.Threading.Tasks.Task<int> ReadAsync (byte[] buffer, int offset, int count, System.Threading.CancellationToken cancellationToken)
		{
			return _pipeStream.Output.ReadAsync (buffer, offset, count, cancellationToken);
		}

		public override void Flush ()
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

		public override void Write (byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException ();
		}

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

