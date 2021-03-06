﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit.Extensions
{
    public class ObservationTestClassRunner : TestClassRunner<ObservationTestCase>
    {
	    private bool exceptionDuringInitialization { get; }
	    readonly ISpecification specification;

        public ObservationTestClassRunner(ISpecification specification, ITestClass testClass, IReflectionTypeInfo @class, IEnumerable<ObservationTestCase> testCases, IMessageSink diagnosticMessageSink, IMessageBus messageBus, ITestCaseOrderer testCaseOrderer, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource)
            : base(testClass, @class, testCases, diagnosticMessageSink, messageBus, testCaseOrderer, aggregator, cancellationTokenSource)
        {
	        this.specification = specification;
        }

	    protected override Task<RunSummary> RunTestMethodAsync(ITestMethod testMethod,
                                                               IReflectionMethodInfo method,
                                                               IEnumerable<ObservationTestCase> testCases,
                                                               object[] constructorArguments)
        {
            return new ObservationTestMethodRunner(specification, testMethod, Class, method, testCases, MessageBus, new ExceptionAggregator(Aggregator), CancellationTokenSource).RunAsync();
        }
    }
}
