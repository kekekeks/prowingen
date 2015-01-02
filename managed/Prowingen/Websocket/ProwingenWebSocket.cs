using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using WebSocketAccept = System.Action
	<System.Collections.Generic.IDictionary<string, object>, // WebSocket Accept parameters
System.Func // WebSocketFunc callback
<System.Collections.Generic.IDictionary<string, object>, // WebSocket environment
System.Threading.Tasks.Task // Complete
>
>;

using WebSocketFunc = System.Func
	<System.Collections.Generic.IDictionary<string, object>, // WebSocket Environment
System.Threading.Tasks.Task // Complete
>;

using WebSocketSendAsync = System.Func
	<System.ArraySegment<byte> /* data */,
int /* messageType */,
bool /* endOfMessage */,
System.Threading.CancellationToken /* cancel */,
System.Threading.Tasks.Task
>;

using WebSocketReceiveAsync = System.Func
	<System.ArraySegment<byte> /* data */,
System.Threading.CancellationToken /* cancel */, System.Threading.Tasks.Task
<System.Tuple
<
int /* messageType */,
bool /* endOfMessage */,
int /* count */
>
>
>;

using WebSocketReceiveTuple = System.Tuple
	<
	int /* messageType */,
bool /* endOfMessage */,
int /* count */
>;

using WebSocketCloseAsync = System.Func
	<
	int /* closeStatus */,
string /* closeDescription */,
System.Threading.CancellationToken /* cancel */,
System.Threading.Tasks.Task
>;
using System.IO;
using System.Net.WebSockets;



namespace Prowingen
{
	public class ProwingenWebSocket : WebSocket
	{
		Dictionary<string, object> _wsEnv;
		Stream _output;
		Stream _input;

		public ProwingenWebSocket (System.Collections.Generic.IDictionary<string, object> opaqueEnv, System.Collections.Generic.IDictionary<string, object> env)
		{
			_output = (Stream)opaqueEnv ["opaque.Output"];
			_input = (Stream)opaqueEnv ["opaque.Input"];
			_wsEnv = new Dictionary<string, object>
			{
				{ "websocket.SendAsync", new WebSocketSendAsync (SendAsync) },
				{ "websocket.ReceiveAsync", new WebSocketReceiveAsync (RecieveAsync) },
				{ "websocket.CloseAsync", new WebSocketCloseAsync (CloseAsync) },
				{ "websocket.Version", "1.0" },
				{ "websocket.CallCancelled", opaqueEnv ["opaque.CallCancelled"] },
				//Hack for those lazy assholes, who aren't using OWIN websocket spec
				{ typeof(WebSocketContext).FullName, new ProwingenWebSocketContext(env, this)}
			};
		}


		public System.Threading.Tasks.Task Handle (WebSocketFunc callback)
		{
			return callback (_wsEnv);
		}

		Task SendAsync (ArraySegment<byte> data, int type, bool eom, System.Threading.CancellationToken cancel)
		{

			cancel.ThrowIfCancellationRequested ();
			//TODO: Optimize
			//Copy/Paste from http://stackoverflow.com/questions/8125507/how-can-i-send-and-receive-websocket-messages-on-the-server-side
			//It is ineffective, but I will use it for now

			Byte[] response;

			Byte[] frame = new Byte[10];

			Int32 indexStartRawData = -1;
			Int32 length = data.Count;

			frame[0] = (Byte)129;
			if (length <= 125)
			{
				frame[1] = (Byte)length;
				indexStartRawData = 2;
			}
			else if (length >= 126 && length <= 65535)
			{
				frame[1] = (Byte)126;
				frame[2] = (Byte)((length >> 8) & 255);
				frame[3] = (Byte)(length & 255);
				indexStartRawData = 4;
			}
			else
			{
				frame[1] = (Byte)127;
				frame[2] = (Byte)((length >> 56) & 255);
				frame[3] = (Byte)((length >> 48) & 255);
				frame[4] = (Byte)((length >> 40) & 255);
				frame[5] = (Byte)((length >> 32) & 255);
				frame[6] = (Byte)((length >> 24) & 255);
				frame[7] = (Byte)((length >> 16) & 255);
				frame[8] = (Byte)((length >> 8) & 255);
				frame[9] = (Byte)(length & 255);

				indexStartRawData = 10;
			}

			response = new Byte[indexStartRawData + length];

			Int32 i, reponseIdx = 0;

			for (i = 0; i < indexStartRawData; i++)
			{
				response[reponseIdx] = frame[i];
				reponseIdx++;
			}

			Buffer.BlockCopy (data.Array, data.Offset, response, reponseIdx, data.Count);
			return _output.WriteAsync (response, 0, response.Length, cancel);

		}

		Task<WebSocketReceiveTuple> RecieveAsync (ArraySegment<byte> arg1, System.Threading.CancellationToken arg2)
		{
			return new TaskCompletionSource<WebSocketReceiveTuple> ().Task;
		}

		Task CloseAsync (int status, string statusDesc, System.Threading.CancellationToken cancellationToken)
		{
			return new TaskCompletionSource<int> ().Task;
		}

		public override void Dispose ()
		{

		}

		#region implemented abstract members of WebSocket
		public override void Abort ()
		{
			Dispose ();
		}
		public override Task CloseAsync (WebSocketCloseStatus closeStatus, string statusDescription, System.Threading.CancellationToken cancellationToken)
		{
			return CloseAsync ((int)closeStatus, statusDescription, cancellationToken);
		}

		public override Task CloseOutputAsync (WebSocketCloseStatus closeStatus, string statusDescription, System.Threading.CancellationToken cancellationToken)
		{
			return 	CloseAsync (closeStatus, statusDescription, cancellationToken);
		}
		public override async Task<WebSocketReceiveResult> ReceiveAsync (ArraySegment<byte> buffer, System.Threading.CancellationToken cancellationToken)
		{
			var res = await RecieveAsync (buffer, cancellationToken);
			return new WebSocketReceiveResult (res.Item3, (WebSocketMessageType)res.Item1, res.Item2);
		}
		public override Task SendAsync (ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, System.Threading.CancellationToken cancellationToken)
		{
			return SendAsync (buffer, (int)messageType, endOfMessage, cancellationToken);
		}


		public override WebSocketCloseStatus? CloseStatus
		{
			get
			{
				return null;
			}
		}
		public override string CloseStatusDescription
		{
			get
			{
				return "";
			}
		}
		public override WebSocketState State
		{
			get
			{
				return WebSocketState.Open;
			}
		}
		public override string SubProtocol
		{
			get
			{
				return "";
			}
		}
		#endregion
	}
}

