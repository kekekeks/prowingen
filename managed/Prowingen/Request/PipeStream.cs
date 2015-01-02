using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Prowingen
{
	public class PipeStream
	{

		object _lock = new object ();
		Queue<byte[]> _buffers = new Queue<byte[]> ();
		TaskCompletionSource<int> _nextBuffer = new TaskCompletionSource<int> ();

		byte[] _currentBuffer;
		int _currentBufferPosition;

		public WriteableStream Input { get; private set; }

		public Stream Output { get; private set; }

		public PipeStream ()
		{
			Input = new WriteableStream (this);
			Output = new ReadableStream (this);
		}

		public class WriteableStream : Stream
		{
			PipeStream _parent;

			public WriteableStream (PipeStream parent)
			{
				_parent = parent;
			}

			void Append (byte[] data)
			{
				lock (_parent._lock)
				{
					_parent._buffers.Enqueue (data);
					_parent._nextBuffer.SetResult (0);
					_parent._nextBuffer = new TaskCompletionSource<int> ();
				}
			}

			public void AppendOwnedBuffer (byte[] data)
			{
				if (data == null || data.Length == 0)
					return;
				Append (data);
			}

			public override void Write (byte[] buffer, int offset, int count)
			{
				if (count == 0)
					return;
				var owned = new byte[count];
				Buffer.BlockCopy (buffer, offset, owned, 0, count);
				AppendOwnedBuffer (owned);
			}

			public override Task WriteAsync (byte[] buffer, int offset, int count, System.Threading.CancellationToken cancellationToken)
			{
				cancellationToken.ThrowIfCancellationRequested ();
				Write (buffer, offset, count);
				return Task.FromResult (0);
			}

			public override void Flush ()
			{

			}

			public override Task FlushAsync (System.Threading.CancellationToken cancellationToken)
			{
				return Task.FromResult (0);
			}

			public override bool CanWrite
			{
				get
				{
					return true;
				}
			}

			protected override void Dispose (bool disposing)
			{
				Append (null);//EOF
				base.Dispose (disposing);
			}

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

		class ReadableStream : Stream
		{
			readonly PipeStream p;
			bool _gotEof;

			public ReadableStream (PipeStream parent)
			{
				p = parent;
			}

			public async override Task<int> ReadAsync (byte[] buffer, int offset, int count, System.Threading.CancellationToken cancellationToken)
			{
				cancellationToken.ThrowIfCancellationRequested ();
				if (_gotEof)
					return 0;
				if (count == 0)
					return 0;
				if (buffer.Length < offset + count)
					throw new IndexOutOfRangeException ();

				var readTotal = 0;
				while(true) //Two pass, 1) before await, 2) after await if needed
				{

					Task task = null;
					lock (p._lock)
					{
						while (count > 0)//Read what we can now, then break
						{
							//Ensure that we have a buffer to read from
							if (p._currentBuffer == null || p._currentBufferPosition >= p._currentBuffer.Length)
							{
								if (p._buffers.Count == 0)
									break; //Whoops, no data
								p._currentBufferPosition = 0;
								p._currentBuffer = p._buffers.Dequeue ();
							}
							if (p._currentBuffer == null)
							{
								_gotEof = true;
								break; //Whoops, EOF
							}
							var toRead = Math.Min (count, p._currentBuffer.Length - p._currentBufferPosition);
							Buffer.BlockCopy (p._currentBuffer, p._currentBufferPosition, buffer, offset, toRead);
							p._currentBufferPosition += toRead;
							offset += toRead;
							count -= toRead;
							readTotal += toRead;
						}
					
						//We are here in 3 cases
						//1) Successfully read at least some data (readTotal != 0)
						//2) Were unable to read anything because of EOF or buffer underrun
						if (readTotal != 0)
							return readTotal;
						if (_gotEof)
							return 0;
						//OK, we have to wait, save task from current _tcs
						task = p._nextBuffer.Task;
					}
					await task.ConfigureAwait(false); //Wait for some data for the next pass
					if (readTotal == 0)
						cancellationToken.ThrowIfCancellationRequested ();//Since we haven't read anything yet, it's ok to throw
				}
			}

		

			public override int Read (byte[] buffer, int offset, int count)
			{
				//This stream is asynchronious by its nature, so synchronious version is just for compatibility
				return ReadAsync (buffer, offset, count).Result;
			}

			public override bool CanRead
			{
				get
				{
					return true;
				}
			}


			#region NotSupported

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
}

