using System;

namespace Application.Core;

public class Result<T>
{
    public bool IsSuceess { get; set; }

    public T? Value { get; set; }

    public string? Error { get; set; }

    public int Code { get; set; }

    public static Result<T> Success(T value) => new()
    {
        IsSuceess = true,
        Value = value
    };
    
    public static Result<T> Failure(string error, int code) => new()
    {
        IsSuceess = false,
        Error = error,
        Code = code
    };
}
