using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Prowingen
{
	public unsafe class ProwingenRequestStream : Stream
	{
		readonly Request _parent;
		readonly IoBufInfo* _buffers;
		readonly int _bufferCount;
		readonly int _length;

		int _currentPosition;
		int _currentBuffer;
		int _currentBufferPosition;

		internal ProwingenRequestStream (Request parent, RequestInfo* info)
		{
			_parent = parent;
			_buffers = info->Buffers;
			_bufferCount = (int)info->BufferCount;
			for (var c = 0; c < _bufferCount; c++)
				_length += (int)_buffers [c].Size;
		}

		public override int Read (byte[] buffer, int offset, int count)
		{
			if (count == 0)
				return 0;
			if (buffer == null)
				throw new ArgumentNullException ("buffer");
			if (offset < 0 || offset + count > buffer.Length)
				throw new IndexOutOfRangeException ();
			if (count < 0)
				throw new ArgumentOutOfRangeException ("count");
			_parent.CheckDisposed ();

			var remainingStreamDataLen = _length - _currentPosition;
			if (remainingStreamDataLen == 0)
				return 0;

			var plannedToReadTotal = count <= remainingStreamDataLen ? count : remainingStreamDataLen;
			int toReadTotal = plannedToReadTotal;


			while(toReadTotal > 0)
			{
				if ((ulong)_currentBufferPosition >= _buffers [_currentBuffer].Size)
				{
					//Set next buffer
					_currentBufferPosition = 0;
					_currentBuffer++;
					Debug.Assert (_currentBuffer < _bufferCount);
				}
				else
				{
					//Calculate length of the data to read from current IOBuf
					var currentBuffer = _buffers [_currentBuffer];
					var remainingBufferDataLen = currentBuffer.Size - (ulong)_currentBufferPosition;
					var toReadFromBuffer = (ulong)toReadTotal <= remainingBufferDataLen ? (int)toReadTotal : (int)remainingBufferDataLen;

					Marshal.Copy (new IntPtr (currentBuffer.Data + _currentBufferPosition), buffer, offset, toReadFromBuffer);

					//Update offsets
					offset += toReadFromBuffer;
					_currentBufferPosition += toReadFromBuffer;
					toReadTotal -= toReadFromBuffer;
				}
			}

			_currentPosition += plannedToReadTotal;
			return plannedToReadTotal;
		}

		public override System.Threading.Tasks.Task<int> ReadAsync (byte[] buffer, int offset, int count, System.Threading.CancellationToken cancellationToken)
		{
			return Task.FromResult (Read (buffer, offset, count));
		}

		protected override void Dispose(bool disposing)
		{
			_parent.Dispose ();
		}


		#region Info
		public override long Length{get{return _length;}}

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
				return _currentPosition;
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

