﻿using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit.Extensions
{
	public class ObservationTestRunner : TestRunner<ObservationTestCase>
    {
        readonly ISpecification specification;
        readonly ExecutionTimer timer;
        
        public ObservationTestRunner(ISpecification specification, ITest test, IMessageBus messageBus, ExecutionTimer timer, Type testClass, MethodInfo testMethod, string skipReason, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource)
            : base(test, messageBus, testClass, null, testMethod, null, skipReason, aggregator, cancellationTokenSource)
        {
            this.specification = specification;
            this.timer = timer;
        }

        protected override async Task<Tuple<decimal, string>> InvokeTestAsync(ExceptionAggregator aggregator)
        {
            var duration = await new ObservationTestInvoker(specification, Test, MessageBus, TestClass, TestMethod, aggregator, CancellationTokenSource).RunAsync();
            return Tuple.Create(duration, String.Empty);
        }
    }
}

