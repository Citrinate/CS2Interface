using System;

namespace CS2Interface {
	internal class ClientException : Exception
	{
		internal EClientExceptionType Type;
		internal ClientException(EClientExceptionType type, string message) : base(message) { 
			Type = type;
		}
	}

	internal enum EClientExceptionType {
		Failed,
		BadRequest,
		Timeout
	}
}