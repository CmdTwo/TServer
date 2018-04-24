using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using TServer.Generic;
using TServer.Common;

namespace TServer
{
    public class TClient
    {
        private int ProfileID;
        private DBManager DBManager;
        private Server Server;
        public TcpClient TcpClient { get; private set; }

        public TClient(TcpClient tcpClient, DBManager dbManager, Server server)
        {
            TcpClient = tcpClient;
            DBManager = dbManager;
            Server = server;
        }

        public void ReadCallback(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState;
            int bytesRead = TcpClient.Client.EndReceive(ar);
            if (bytesRead > 0)
            {
                MessageManager messageManager = new MessageManager();
                Dictionary<ReceiveMessageParam, object> message = messageManager.DeserializeMessage<ReceiveMessageParam>(state.Buffer, bytesRead);

                if ((bool)(message[ReceiveMessageParam.IsRequest]))
                {
                    switch ((byte)message[ReceiveMessageParam.CommandType])
                    {
                        case ((byte)RequestCommand.Authorization):
                            Request_Authorization(message[ReceiveMessageParam.Params] as Dictionary<ParameterType, object>);
                            break;
                        case ((byte)RequestCommand.GetPartOfProfile):
                            Request_GetPartOfProfile(message[ReceiveMessageParam.Params] as Dictionary<ParameterType, object>);
                            break;
                        case ((byte)RequestCommand.GetGroupList):
                            Request_GetGroupList(message[ReceiveMessageParam.Params] as Dictionary<ParameterType, object>);
                            break;
                        case ((byte)RequestCommand.GetColorList):
                            Request_GetColorList(message[ReceiveMessageParam.Params] as Dictionary<ParameterType, object>);
                            break;
                    }
                }
                else
                {
                    //switch ((byte)message[ReceiveMessageParam.CommandType])
                    //{
                    //    case ((byte)ResponseCommand.AuthorizationResponse):
                    //        AuthorizationResponse(message[ReceiveMessageParam.Params] as Dictionary<ParameterType, object>);
                    //        break;
                    //}
                }
                Server.BeginRead(this, ar);
            }
        }

        #region RequestMethods

        private void Request_Authorization(Dictionary<ParameterType, object> args)
        {
            object[] returnArgs = DBManager.Authorized((string)args[ParameterType.login], (string)args[ParameterType.password]);
            Dictionary<ReceiveMessageParam, object> response = new Dictionary<ReceiveMessageParam, object>() {
                            { ReceiveMessageParam.CommandType, ResponseCommand.AuthorizationResponse },
                            { ReceiveMessageParam.IsRequest, false },
                            { ReceiveMessageParam.Params, new Dictionary<ParameterType, object> {
                                { ParameterType.authorized, returnArgs[0] } } } };

            if (returnArgs[1] != null)
                ProfileID = (int)returnArgs[1];

            MessageManager messageManager = new MessageManager();
            byte[] bytes = messageManager.SerializeMessage(response);
            TcpClient.GetStream().Write(bytes, 0, bytes.Length);
        }

        private void Request_GetPartOfProfile(Dictionary<ParameterType, object> args)
        {
            object[] returnArgs = DBManager.GetPartOfProfile(ProfileID);
            if(returnArgs != null)
            {
                Dictionary<ReceiveMessageParam, object> response = new Dictionary<ReceiveMessageParam, object>() {
                            { ReceiveMessageParam.CommandType, ResponseCommand.GetPartOfProfileResponse },
                            { ReceiveMessageParam.IsRequest, false },
                            { ReceiveMessageParam.Params, new Dictionary<ParameterType, object> {
                                { ParameterType.name, returnArgs[0] } } } };
                MessageManager messageManager = new MessageManager();
                byte[] bytes = messageManager.SerializeMessage(response);
                TcpClient.GetStream().Write(bytes, 0, bytes.Length);
            }
            ////// ELSE - FIX IN FUTURE
        }

        private void Request_GetGroupList(Dictionary<ParameterType, object> args)
        {
            Dictionary<ReceiveMessageParam, object> response = new Dictionary<ReceiveMessageParam, object>() {
                            { ReceiveMessageParam.CommandType, ResponseCommand.GetGroupListResponse },
                            { ReceiveMessageParam.IsRequest, false },
                            { ReceiveMessageParam.Params, new Dictionary<ParameterType, object> {
                                { ParameterType.groupsList, DBManager.GetGroupsList(ProfileID) } } } };

            MessageManager messageManager = new MessageManager();
            byte[] bytes = messageManager.SerializeMessage(response);
            TcpClient.GetStream().Write(bytes, 0, bytes.Length);
        }

        private void Request_GetColorList(Dictionary<ParameterType, object> args)
        {
            Dictionary<ReceiveMessageParam, object> response = new Dictionary<ReceiveMessageParam, object>() {
                            { ReceiveMessageParam.CommandType, ResponseCommand.GetColorListResponse },
                            { ReceiveMessageParam.IsRequest, false },
                            { ReceiveMessageParam.Params, new Dictionary<ParameterType, object> {
                                { ParameterType.groupsList, DBManager.GetColorList() } } } };

            MessageManager messageManager = new MessageManager();
            byte[] bytes = messageManager.SerializeMessage(response);
            TcpClient.GetStream().Write(bytes, 0, bytes.Length);
        }

        private void Request_GetSubGroupList(Dictionary<ParameterType, object> args)
        {
            Dictionary<ReceiveMessageParam, object> response = new Dictionary<ReceiveMessageParam, object>() {
                            { ReceiveMessageParam.CommandType, ResponseCommand.GetSubgroupListResponse },
                            { ReceiveMessageParam.IsRequest, false },
                            { ReceiveMessageParam.Params, new Dictionary<ParameterType, object> {
                                { ParameterType.subgroupsList, DBManager.GetSubgroupList((int)args[ParameterType.groupID]) } } } };

            MessageManager messageManager = new MessageManager();
            byte[] bytes = messageManager.SerializeMessage(response);
            TcpClient.GetStream().Write(bytes, 0, bytes.Length);
        }
        #endregion

        #region ResponseMethods

        private void Response_Authorization()
        {

        }

        #endregion
    }
}
