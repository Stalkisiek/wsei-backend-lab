using System;

namespace CoreApp.Exceptions;

public class LecturerNotFoundException : Exception
{
    public LecturerNotFoundException(string msg) : base(msg)
    {
    }
}

