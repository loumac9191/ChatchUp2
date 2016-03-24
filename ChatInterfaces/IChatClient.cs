using System;
// ----------------------------------------------------------------------------
// Copyright 2010 Wyle
// ----------------------------------------------------------------------------
using System.ServiceModel;

namespace ChatInterfaces
{
    public interface IChatClient
    {
        [OperationContract(IsOneWay = true)]
        void ReceiveMessage(DateTime timeOfPost,string userName, string message);

        [OperationContract(IsOneWay = true)]
        void UpdateUserList(string userName);

    }
}