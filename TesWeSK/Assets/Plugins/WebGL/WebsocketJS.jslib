var WebsocketJS = 
{
	$webSocket:{},
	ConnectJS : function(url)
	{
		var s_url = Pointer_stringify(url);
		webSocket = new WebSocket(s_url);
		webSocket.onmessage = function (e) 
		{
			if (e.data instanceof Blob)
			{
				var reader = new FileReader();
				reader.addEventListener("loadend", function() 
				{
					var array = new Uint8Array(reader.result);
					var msg = "";
					for(var i = 0;i<array.length;i++)
					{
						if(i == array.length - 1)
							msg += array[i];
						else
							msg += array[i]+" ";
					}
					SendMessage("WebSocket","OnMessage_2",msg);
				});
				reader.readAsArrayBuffer(e.data);
			}
			else
			{
				alert("msg not a blob instance");
			}
		};
		webSocket.onopen = function(e)
		{
			SendMessage("WebSocket","OnOpen_2");
		};
		webSocket.onclose = function(e)
		{
			SendMessage("WebSocket","OnClose_2");
		};
	},

	SendMsgJS: function (msg,length)
	{
		webSocket.send(HEAPU8.buffer.slice(msg, msg+length));
	},
	
	CloseJS: function ()
	{
		webSocket.close();
	},
	
	AlertJS:function (msg)
	{
		var s_msg = Pointer_stringify(msg);
		alert(s_msg);
	}
};

autoAddDeps(WebsocketJS, '$webSocket');
mergeInto(LibraryManager.library, WebsocketJS);