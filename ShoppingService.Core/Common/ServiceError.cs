using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;

namespace ShoppingService.Core.Common
{
    public class ServiceError : IEquatable<ServiceError>
    {
        public IEnumerable<string> Messages;
        public int StatusCode;

        public ServiceError(IEnumerable<string> _Messages, int _StatusCode)
        {
            Messages = _Messages;
            StatusCode = _StatusCode;
        }

        public static ServiceError CreateWithSingleMessage(string Message, int StatusCode) {
            return new ServiceError(new List<string> {Message}, StatusCode);
        }

        public override int GetHashCode() => (Messages.GetHashCode() ^ StatusCode.GetHashCode());

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            return Equals((ServiceError)obj);
        }

        public bool Equals(ServiceError other) => (
            Messages.All(item => other.Messages.Contains(item)) && StatusCode == other.StatusCode
        );

        public static bool operator ==(ServiceError error1, ServiceError error2) => error1.Equals(error2);
        public static bool operator !=(ServiceError error1, ServiceError error2) => !error1.Equals(error2);
    }
}
