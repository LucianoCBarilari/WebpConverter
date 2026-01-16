using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaProcessorLibrary.Application.Results
{
    public class Result
    {
        public bool IsSuccess { get; }
        public Operation? Operation { get; }
        public ErrorCode? Error { get; }

        private Result(bool success, Operation? op, ErrorCode? error)
        {
            IsSuccess = success;
            Operation = op;
            Error = error;
        }

        public static Result Ok(Operation op)
            => new(true, op, null);

        public static Result Fail(ErrorCode error)
            => new(false, null, error);
    }
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public T? Value { get; }
        public Operation? Operation { get; }
        public ErrorCode? Error { get; }

        private Result(bool success, T? value, Operation? operation, ErrorCode? error)
        {
            IsSuccess = success;
            Value = value;
            Operation = operation;
            Error = error;
        }

        public static Result<T> Ok(T value, Operation operation)
            => new(true, value, operation, null);

        public static Result<T> Fail(ErrorCode error)
            => new(false, default, null, error);
    }

}
