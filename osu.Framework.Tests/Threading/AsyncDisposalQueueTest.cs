// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Logging;

namespace osu.Framework.Tests.Threading
{
    [TestFixture]
    public class AsyncDisposalQueueTest
    {
        [Test]
        public void TestManyAsyncDisposal()
        {
            var objects = new List<DisposableObject>();
            for (int i = 0; i < 10000; i++)
                objects.Add(new DisposableObject());

            objects.ForEach(AsyncDisposalQueue.Enqueue);

            int attempts = 1000;

            while (!objects.All(o => o.IsDisposed))
            {
                if (attempts-- == 0)
                    Assert.Fail("Expected all objects to dispose.");
                Thread.Sleep(10);
            }

            Logger.Log(objects.Select(d => d.TaskId).Distinct().Count().ToString());

            // It's very unlikely for this to fail by chance due to the number of objects being disposed and computational time requirement of task scheduling
            Assert.That(objects.Select(d => d.TaskId).Distinct().Count(), Is.LessThan(objects.Count));
        }

        private class DisposableObject : IDisposable
        {
            public int? TaskId { get; private set; }

            public bool IsDisposed { get; private set; }

            public void Dispose()
            {
                TaskId = Task.CurrentId;
                IsDisposed = true;
            }
        }
    }
}
