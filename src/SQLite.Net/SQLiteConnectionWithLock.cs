//
// Copyright (c) 2012 Krueger Systems, Inc.
// Copyright (c) 2013 �ystein Krog (oystein.krog@gmail.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Threading;
using JetBrains.Annotations;
using SQLite.Net.Interop;

namespace SQLite.Net
{
    public class SQLiteConnectionWithLock : SQLiteConnection
    {
        private readonly object _lockPoint = new object();

        [PublicAPI]
        public SQLiteConnectionWithLock(ISQLitePlatform sqlitePlatform, SQLiteConnectionString connectionString)
            : base(sqlitePlatform, connectionString.DatabasePath, connectionString.StoreDateTimeAsTicks, connectionString.Serializer, null, null, connectionString.Resolver) { }

        [PublicAPI]
        public IDisposable Lock()
        {
            return new LockWrapper(_lockPoint);
        }

        private class LockWrapper : IDisposable
        {
            private readonly object _lockPoint;

            public LockWrapper(object lockPoint)
            {
                _lockPoint = lockPoint;
                Monitor.Enter(_lockPoint);
            }

            public void Dispose()
            {
                Monitor.Exit(_lockPoint);
            }
        }
    }
}