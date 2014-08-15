using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebSocketSharp;

namespace DoshiiDotNetIntegration.CommunicationLogic
{
    /// <summary>
    /// faciliates the web sockets communication with the doshii application
    /// </summary>
    internal class DoshiiWebSocketsCommunication : LoggingBase
    {

        #region properties

        /// <summary>
        /// web socket object that will handle all the communications with doshii
        /// </summary>
        WebSocket ws = null;

        #endregion

        #region internal methods and events

        #region events

        internal event EventHandler<MessageEventArgs> webSocketMessage;

        #endregion

        #region methods

        internal DoshiiWebSocketsCommunication(string webSocketURL)
        {
            ws = new WebSocket(webSocketURL);
            ws.OnOpen += new EventHandler(ws_OnOpen);
            ws.OnClose += new EventHandler<CloseEventArgs>(ws_OnClose);
            ws.OnMessage += new EventHandler<MessageEventArgs>(ws_OnMessage);
            ws.OnError += new EventHandler<ErrorEventArgs>(ws_OnError);
            
        }

        internal void Connect()
        {
            if (ws != null)
            {
                ws.Connect();
            }
            else
            {
                log.Error(string.Format("Attempted to open a web socket connection before initializing the ws object"));
            }
        }

        internal void SendMessage(string message)
        {
            log.Debug(string.Format("sending websockets message {0} to {1}", message, ws.Url.ToString()));
            ws.Send(message);
        }

        #endregion

        #endregion

        #region event handlers

        private void ws_OnError(object sender, ErrorEventArgs e)
        {
            log.Error(string.Format("there was an error with the websockets connection to {0} the error was", ws.Url.ToString(), e.Message));
        }

        private void ws_OnMessage(object sender, MessageEventArgs e)
        {
            OnMessageReceived(e);
        }

        private void OnMessageReceived(MessageEventArgs e)
        {
            webSocketMessage(this, e);
        }

        private void ws_OnClose(object sender, CloseEventArgs e)
        {
            log.Debug(string.Format("webScokets connection to {0} closed", ws.Url.ToString()));
        }

        private void ws_OnOpen(object sender, EventArgs e)
        {
            log.Debug(string.Format("webScokets connection open to {0}", ws.Url.ToString()));
        }

        #endregion

    }
}
