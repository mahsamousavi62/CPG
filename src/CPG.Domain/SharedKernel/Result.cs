using System;

namespace CPG.Domain.SharedKernel;

public class Result<T>
{
    private Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None ||
            !isSuccess && error == Error.None)
        {
            throw new ArgumentException("Invalid error", nameof(error));
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    private Result(bool isSuccess, Error error, T data)
    {
        if (isSuccess && error != Error.None ||
            !isSuccess && error == Error.None)
        {
            throw new ArgumentException("Invalid error", nameof(error));
        }

        IsSuccess = isSuccess;
        Error = error;
        Data = data;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public T Data {get; set;}

    public Error Error { get; }

    public static Result<T> SuccessResult(T data) => new(true, Error.None, data);

    public static Result<T> FailureResult(T data, Error error) => new(false, error, data);

    public static Result<T> Success() => new(true, Error.None);

    public static Result<T> Failure(Error error) => new(false, error);
}
