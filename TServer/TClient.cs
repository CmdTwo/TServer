using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using TServer.Generic;
using TServer.Common;
using TServer.Common.Content;

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
                            Log.Write("New request: " + RequestCommand.Authorization);
                            Request_Authorization(message[ReceiveMessageParam.Params] as Dictionary<ParameterType, object>);
                            break;
                        case ((byte)RequestCommand.GetPartOfProfile):
                            Log.Write("New request: " + RequestCommand.GetPartOfProfile);
                            Request_GetPartOfProfile(message[ReceiveMessageParam.Params] as Dictionary<ParameterType, object>);
                            break;
                        case ((byte)RequestCommand.GetAvailableAccessList):
                            Log.Write("New request: " + RequestCommand.GetAvailableAccessList);
                            Request_GetAvailableAccessList(message[ReceiveMessageParam.Params] as Dictionary<ParameterType, object>);
                            break;
                        case ((byte)RequestCommand.GetAvailableSubgroupList):
                            Log.Write("New request: " + RequestCommand.GetAvailableSubgroupList);
                            Request_GetAvailableSubgroupList(message[ReceiveMessageParam.Params] as Dictionary<ParameterType, object>);
                            break;
                        case ((byte)RequestCommand.GetUserList):
                            Log.Write("New request: " + RequestCommand.GetUserList);
                            Request_GetUserList(message[ReceiveMessageParam.Params] as Dictionary<ParameterType, object>);
                            break;
                        case ((byte)RequestCommand.GetTagList):
                            Log.Write("New request: " + RequestCommand.GetTagList);
                            Request_GetTagList(message[ReceiveMessageParam.Params] as Dictionary<ParameterType, object>);
                            break;
                        case ((byte)RequestCommand.AddNewTest):
                            Log.Write("New request: " + RequestCommand.AddNewTest);
                            Request_AddNewTest(message[ReceiveMessageParam.Params] as Dictionary<ParameterType, object>);
                            break;
                        case ((byte)RequestCommand.GetAvailableTests):
                            Log.Write("New request: " + RequestCommand.GetAvailableTests);
                            Request_GetAvailableTests(message[ReceiveMessageParam.Params] as Dictionary<ParameterType, object>);
                            break;
                        case ((byte)RequestCommand.GetFailedTests):
                            Log.Write("New request: " + RequestCommand.GetFailedTests);
                            Request_GetFailedTests(message[ReceiveMessageParam.Params] as Dictionary<ParameterType, object>);
                            break;
                        case ((byte)RequestCommand.GetComplitedTests):
                            Log.Write("New request: " + RequestCommand.GetComplitedTests);
                            Request_GetComplitedTests(message[ReceiveMessageParam.Params] as Dictionary<ParameterType, object>);
                            break;
                        case ((byte)RequestCommand.GetTest):
                            Log.Write("New request: " + RequestCommand.GetTest);
                            Request_GetTest(message[ReceiveMessageParam.Params] as Dictionary<ParameterType, object>);
                            break;
                        case ((byte)RequestCommand.SaveProgress):
                            Log.Write("New request: " + RequestCommand.SaveProgress);
                            Request_SaveProgress(message[ReceiveMessageParam.Params] as Dictionary<ParameterType, object>);
                            break;
                        case ((byte)RequestCommand.UserCompletedTest):
                            Log.Write("New request: " + RequestCommand.UserCompletedTest);
                            Request_UserCompletedTest(message[ReceiveMessageParam.Params] as Dictionary<ParameterType, object>);
                            break;
                    }
                }
                else
                {
                    //switch ((byte)message[ReceiveMessageParam.CommandType])
                    //{
                    //    case ((byte)ResponseCommand.AutorizationResponse):
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
                ProfileID = Convert.ToInt32(returnArgs[1]);

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

        private void Request_GetAvailableAccessList(Dictionary<ParameterType, object> args)
        {
            Dictionary<ReceiveMessageParam, object> response = new Dictionary<ReceiveMessageParam, object>() {
                            { ReceiveMessageParam.CommandType, ResponseCommand.GetAvailableAccessListResponse },
                            { ReceiveMessageParam.IsRequest, false },
                            { ReceiveMessageParam.Params, new Dictionary<ParameterType, object> {
                                { ParameterType.availableAccessList, DBManager.GetAvailableAccessList(ProfileID) } } } };

            MessageManager messageManager = new MessageManager();
            byte[] bytes = messageManager.SerializeMessage(response);
            TcpClient.GetStream().Write(bytes, 0, bytes.Length);
        }

        private void Request_GetColorList(Dictionary<ParameterType, object> args)
        {
            //Dictionary<ReceiveMessageParam, object> response = new Dictionary<ReceiveMessageParam, object>() {
            //                { ReceiveMessageParam.CommandType, ResponseCommand.GetColorListResponse },
            //                { ReceiveMessageParam.IsRequest, false },
            //                { ReceiveMessageParam.Params, new Dictionary<ParameterType, object> {
            //                    { ParameterType.availableAccessList, DBManager.GetColorList() } } } };

            //MessageManager messageManager = new MessageManager();
            //byte[] bytes = messageManager.SerializeMessage(response);
            //TcpClient.GetStream().Write(bytes, 0, bytes.Length);
        }

        private void Request_GetAvailableSubgroupList(Dictionary<ParameterType, object> args)
        {
            Dictionary<ReceiveMessageParam, object> response = new Dictionary<ReceiveMessageParam, object>() {
                            { ReceiveMessageParam.CommandType, ResponseCommand.GetAvailableSubgroupListResponse },
                            { ReceiveMessageParam.IsRequest, false },
                            { ReceiveMessageParam.Params, new Dictionary<ParameterType, object> {
                                { ParameterType.availableSubAccessList, DBManager.GetAvailableSubgroupList((int)args[ParameterType.groupID]) } } } };

            MessageManager messageManager = new MessageManager();
            byte[] bytes = messageManager.SerializeMessage(response);
            TcpClient.GetStream().Write(bytes, 0, bytes.Length);
        }

        private void Request_GetUserList(Dictionary<ParameterType, object> args)
        {
            Dictionary<ReceiveMessageParam, object> response = new Dictionary<ReceiveMessageParam, object>() {
                            { ReceiveMessageParam.CommandType, ResponseCommand.GetUserListResponse },
                            { ReceiveMessageParam.IsRequest, false },
                            { ReceiveMessageParam.Params, new Dictionary<ParameterType, object> {
                                { ParameterType.usersList, DBManager.GetUsersList(ProfileID) } } } };

            MessageManager messageManager = new MessageManager();
            byte[] bytes = messageManager.SerializeMessage(response);
            TcpClient.GetStream().Write(bytes, 0, bytes.Length);
        }

        private void Request_GetTagList(Dictionary<ParameterType, object> args)
        {
            Dictionary<ReceiveMessageParam, object> response = new Dictionary<ReceiveMessageParam, object>() {
                            { ReceiveMessageParam.CommandType, ResponseCommand.GetTagListResponse },
                            { ReceiveMessageParam.IsRequest, false },
                            { ReceiveMessageParam.Params, new Dictionary<ParameterType, object> {
                                { ParameterType.tagList, DBManager.GetTagList() } } } };

            MessageManager messageManager = new MessageManager();
            byte[] bytes = messageManager.SerializeMessage(response);
            TcpClient.GetStream().Write(bytes, 0, bytes.Length);
        }

        private void Request_AddNewTest(Dictionary<ParameterType, object> args)
        {
            Dictionary<ReceiveMessageParam, object> response = new Dictionary<ReceiveMessageParam, object>() {
                            { ReceiveMessageParam.CommandType, ResponseCommand.AddNewTestResponse },
                            { ReceiveMessageParam.IsRequest, false },
                            { ReceiveMessageParam.Params, new Dictionary<ParameterType, object> {
                                { ParameterType.responseStatus, DBManager.AddNewTest((Test)args[ParameterType.newTest], ProfileID) } } } };

            MessageManager messageManager = new MessageManager();
            byte[] bytes = messageManager.SerializeMessage(response);
            TcpClient.GetStream().Write(bytes, 0, bytes.Length);
        }

        private void Request_GetAvailableTests(Dictionary<ParameterType, object> args)
        {
            Dictionary<ReceiveMessageParam, object> response = new Dictionary<ReceiveMessageParam, object>() {
                            { ReceiveMessageParam.CommandType, ResponseCommand.GetAvailableTestsResponse },
                            { ReceiveMessageParam.IsRequest, false },
                            { ReceiveMessageParam.Params, new Dictionary<ParameterType, object> {
                                { ParameterType.responseStatus, DBManager.GetAvailableTests(ProfileID) } } } };

            MessageManager messageManager = new MessageManager();
            byte[] bytes = messageManager.SerializeMessage(response);
            TcpClient.GetStream().Write(bytes, 0, bytes.Length);
        }

        private void Request_GetFailedTests(Dictionary<ParameterType, object> args)
        {
            Dictionary<ReceiveMessageParam, object> response = new Dictionary<ReceiveMessageParam, object>() {
                            { ReceiveMessageParam.CommandType, ResponseCommand.GetFailedTestsResponse },
                            { ReceiveMessageParam.IsRequest, false },
                            { ReceiveMessageParam.Params, new Dictionary<ParameterType, object> {
                                { ParameterType.responseStatus, DBManager.GetFailedTests(ProfileID) } } } };

            MessageManager messageManager = new MessageManager();
            byte[] bytes = messageManager.SerializeMessage(response);
            TcpClient.GetStream().Write(bytes, 0, bytes.Length);
        }

        private void Request_GetComplitedTests(Dictionary<ParameterType, object> args)
        {
            Dictionary<ReceiveMessageParam, object> response = new Dictionary<ReceiveMessageParam, object>() {
                            { ReceiveMessageParam.CommandType, ResponseCommand.GetComplitedTestsResponse },
                            { ReceiveMessageParam.IsRequest, false },
                            { ReceiveMessageParam.Params, new Dictionary<ParameterType, object> {
                                { ParameterType.responseStatus, DBManager.GetComplitedTests(ProfileID) } } } };

            MessageManager messageManager = new MessageManager();
            byte[] bytes = messageManager.SerializeMessage(response);
            TcpClient.GetStream().Write(bytes, 0, bytes.Length);
        }

        private void Request_GetTest(Dictionary<ParameterType, object> args)
        {
            Dictionary<ReceiveMessageParam, object> response = new Dictionary<ReceiveMessageParam, object>() {
                            { ReceiveMessageParam.CommandType, ResponseCommand.GetTestResponse },
                            { ReceiveMessageParam.IsRequest, false },
                            { ReceiveMessageParam.Params, new Dictionary<ParameterType, object> {
                                { ParameterType.test, DBManager.GetTest((int)args[ParameterType.testID]) } } } };

            MessageManager messageManager = new MessageManager();
            byte[] bytes = messageManager.SerializeMessage(response);
            TcpClient.GetStream().Write(bytes, 0, bytes.Length);
        }

        private void Request_SaveProgress(Dictionary<ParameterType, object> args)
        {
            Dictionary<ReceiveMessageParam, object> response = new Dictionary<ReceiveMessageParam, object>() {
                            { ReceiveMessageParam.CommandType, ResponseCommand.SaveProgressResponse },
                            { ReceiveMessageParam.IsRequest, false },
                            { ReceiveMessageParam.Params, new Dictionary<ParameterType, object> {
                                { ParameterType.responseStatus, DBManager.SaveProgress((int)args[ParameterType.testID], (double)args[ParameterType.progress_score], (int)args[ParameterType.progress_skip], ProfileID) } } } };

            MessageManager messageManager = new MessageManager();
            byte[] bytes = messageManager.SerializeMessage(response);
            TcpClient.GetStream().Write(bytes, 0, bytes.Length);
        }

        private void Request_UserCompletedTest(Dictionary<ParameterType, object> args)
        {
            Dictionary<ReceiveMessageParam, object> response = new Dictionary<ReceiveMessageParam, object>() {
                            { ReceiveMessageParam.CommandType, ResponseCommand.UserCompletedTestResponse },
                            { ReceiveMessageParam.IsRequest, false },
                            { ReceiveMessageParam.Params, new Dictionary<ParameterType, object> {
                                { ParameterType.responseStatus, DBManager.UserCompletedTest((int)args[ParameterType.testID], (int)args[ParameterType.progress_score], ProfileID, (bool)args[ParameterType.isCompleted]) } } } };

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
