using System;

namespace Application.Core;

// This is primary constructor for the AppException class.
// It allows you to create an instance of AppException with a status code, message, and detals
// details is error's stack trace.. we are not goinh to send full stack trace if we are in production
// but in development mode, its convenient for developers to get as much information as possible
// so we will send stack trace to client while in development mode
// This class can be used to represent exceptions that occur in the application, providing a way to
public class AppException(int statusCode, string message, string? details)
{
    // Properties to hold the status code, message, and details of the exception
    // you need to create properties for these fields to make them accessible outside the class
    public int StatusCode { get; } = statusCode;
    public string Message { get; } = message;
    public string? Details { get; } = details;


}
