﻿using System;

namespace Cizeta.TraMaAuth
{
    public class DbException : Exception
    {
        public DbException() : base() { }

        public DbException(string message) : base(message) { }

        public DbException(string message, Exception innerException) : base(message, innerException) { }
    }
}
