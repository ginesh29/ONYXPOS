using System.Net;
using System.Net.WebSockets;
using System.Text;

namespace Onyx_POS.Middleware
{
    public class WebsocketMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context.Request.Path == "/ws" && context.WebSockets.IsWebSocketRequest)
            {
                await HandleWebSocketRequestAsync(context);
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await context.Response.WriteAsync("WebSocket connection expected.");
            }
        }

        private async Task HandleWebSocketRequestAsync(HttpContext context)
        {
            var cancellationToken = context.RequestAborted;
            WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();

            try
            {
                while (webSocket.State == WebSocketState.Open)
                {
                    var isConnected = CheckRemoteConnection(); // Replace with your logic
                    var bytes = Encoding.UTF8.GetBytes(isConnected.ToString());
                    var arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);

                    await webSocket.SendAsync(arraySegment, WebSocketMessageType.Text, true, cancellationToken);

                    // Optionally await a delay to avoid busy waiting
                    await Task.Delay(1000, cancellationToken);
                }
            }
            catch (WebSocketException)
            {
                // Handle exceptions if necessary
            }
            finally
            {
                if (webSocket.State == WebSocketState.Open)
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing WebSocket", cancellationToken);

                webSocket.Dispose();
            }
        }

        private bool CheckRemoteConnection()
        {
            // Implement your logic to check remote connection status
            return true; // Example: Always returning true for demonstration
        }
    }
}