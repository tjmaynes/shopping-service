using System;
using LanguageExt;
using static LanguageExt.Prelude;

namespace ShoppingService.Core.Common
{
    public enum ServiceErrorCode
    {
        ItemNotFound,
        InvalidItem,
        UnknownException
    }

    public class ServiceError : Record<ServiceError>
    {
        public string Message;
        public ServiceErrorCode ErrorCode;

        public ServiceError(string message, ServiceErrorCode errorCode)
        {
            Message = message;
            ErrorCode = errorCode;
        }
    }
}
