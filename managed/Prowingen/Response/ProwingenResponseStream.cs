using System;
using System.IO;
using System.Threading.Tasks;

namespace Prowingen
{
	class ProwingenResponseStream : Stream
	{
		Response _parent;
		byte[] _buffer = BufferManager.GetBuffer();
		int _bufferPosition;

		public ProwingenResponseStream (Response parent)
		{
			_parent = parent;
		}
			
		public override void Flush ()
		{
			if(_bufferPosition!=0)
			{
				WriteInternal (_buffer, 0, _bufferPosition);
				_bufferPosition = 0;
			}
		}

		public override void Write (byte[] buffer, int offset, int count)
		{
			if (count > _buffer.Length)
			{
				Flush ();
				WriteInternal (buffer, offset, count);
			}
			else
			{
				if (_bufferPosition + count > _buffer.Length)
				{
					var toWrite = _bufferPosition + count - _buffer.Length;
					Buffer.BlockCopy (buffer, offset, _buffer, _bufferPosition, toWrite);
					Flush ();
					offset += toWrite;
					count -= toWrite;
				}


				Buffer.BlockCopy (buffer, offset, _buffer, _bufferPosition, count);
				_bufferPosition += count;


			}
		}


		void WriteInternal(byte[] buffer, int offset, int count)
		{
			if (offset < 0 || offset + count > buffer.Length)
				throw new IndexOutOfRangeException ();
			if (count < 0)
				throw new ArgumentOutOfRangeException ("count");
			if (count == 0)
				return;
			_parent.AppendBody (buffer, offset, count, true);
		}

		protected override void Dispose (bool disposing)
		{
			if (_buffer != null)
			{
				if (_bufferPosition == 0)
					_parent.Complete (null, 0, 0);
				else
					_parent.Complete (_buffer, 0, _bufferPosition);
				BufferManager.ReleaseBuffer (_buffer);
			}

			base.Dispose (disposing);
		}

		#region Async

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

