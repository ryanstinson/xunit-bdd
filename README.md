# xUnit.Net BDD Extensions

Extends xUnit.Net with Behavior Driven Development style fixtures.

Some of the design goals include:
 * Use natural C# constructs to control things such as BDD contexts and concerns. For example, the context of the specification is defined in the class constructor and the concern for the fixture is defined by its namespace.
 * Don't force me to inherit from any base class to get BDD style tests working. There is an interface `ISpecification` that one can implement to accomplish this. A `Specification` & `AsyncSpecification` base classes are also provided for convenience.

See here for a [full introduction](https://www.chadly.net/bdd-with-xunit-net/)

## How to Use

Install via [nuget](https://www.nuget.org/packages/xUnit.BDD/)

```
dotnet add package xUnit.BDD
```

* [Write a Scenario](#write-a-scenario)
* [Async Tests](#async-tests)
* [Handling Exceptions](#handling-exceptions)
* [Shared Context](#shared-context)

### Write a Scenario

```cs
using Xunit.Extensions;

[assembly: TestFramework("Xunit.Extensions.ObservationTestFramework", "Xunit.Bdd")]

public class Calculator
{
	public int Add(int x, int y) => x + y;
}

public class when_adding_two_numbers : Specification
{
	readonly Calculator calc;
	int result;

	public when_adding_two_numbers()
	{
		calc = new Calculator();
	}

	public override void Observe()
	{
		result = calc.Add(1, 2);
	}

	[Observation]
	public void should_return_correct_result()
	{
		result.ShouldEqual(3);
	}
}
```

This is a contrived example, but the idea is to run the code you're observing in the `Observe` method and have one or more `Observation`s on what you observed. This way, when a test (`Observation`) fails, the language of the class and method (the scenario) should be granular enough to let you know exactly what failed.

### Async Tests

If you are testing something that is async, you can use the `AsyncSpecification`:

```cs
using Xunit.Extensions;

public class Calculator
{
	public async Task<int> CalculateMagicNumber(int x, int y)
	{
		int result = await SomeAsyncCallToAnExternalService();
		return result + x + y;
	}
}

public class when_making_magic_numbers : AsyncSpecification
{
	readonly Calculator calc;
	int result;

	public when_making_magic_numbers()
	{
		calc = new Calculator();
	}

	public override async Task ObserveAsync()
	{
		result = await calc.CalculateMagicNumber(1, 2);
	}

	[Observation]
	public void should_return_correct_result()
	{
		result.ShouldEqual(69);
	}
}
```

If you have some async setup/teardown that also needs to run with the test, you can override `InitializeAsync` and/or `DisposeAsync`:

```cs
using Xunit.Extensions;

public class when_doing_more_stuff : AsyncSpecification, IAsyncLifetime
{
	public async Task InitializeAsync()
	{
		await DoSomething();
	}

	public override async Task ObserveAsync()
	{
		result = await calc.CalculateMagicNumber(1, 2);
	}

	public async Task DisposeAsync()
	{
		await DoSomethingElse();
	}

	[Observation]
	public void should_return_correct_result()
	{
		result.ShouldEqual(69);
	}
}
```

### Handling Exceptions

If you have a method under test that throws an exception, you can make use of the `HandleExceptionsAttribute` to make assertions on it:

```cs
using Xunit.Extensions;

public class Calculator
{
	public int Add(int x, int y)
	{
		if (x == 69)
			throw new ArgumentException("inappropriate");

		return x + y;
	}
}

[HandleExceptions]
public class when_adding_an_inappropriate_number : Specification
{
	readonly Calculator calc;
	int result;

	public when_adding_an_inappropriate_number()
	{
		calc = new Calculator();
	}

	public override void Observe()
	{
		result = calc.Add(69, 1);
	}

	[Observation]
	public void should_not_return_any_result()
	{
		result.ShouldEqual(0);
	}

	[Observation]
	public void should_throw()
	{
		ThrownException.ShouldBeType<ArgumentException>().Message.ShouldEqual("inappropriate");
	}
}
```

The `HandleExceptionsAttribute` will cause the test harness to handle any exceptions thrown by the `Observe` method. You can then make assertions on the thrown exception via the `ThrownException` property. If you leave off the `HandleExceptions` on the test class, it will not handle any exceptions from `Observe`. Therefore, you should only add the attribute if you are expecting an exception so as not to hide test failures.

### Shared Context

When writing test scenarios like this, you "observe" one thing in the `Observe` method and make one or more `Observation`s on the results. Due to this, the test framework overrides [Xunit's default handling of shared test context](https://xunit.github.io/docs/shared-context.html). Instead of creating a new instance of the class for each `Observation` and rerunning the test setup & `Observe` method, the test harness will create the class once, run the setup & `Observe` once, and then run all of the `Observation`s in sequence.

In other words, it treats all `ISpecification` tests as [class fixtures](https://xunit.github.io/docs/shared-context.html#class-fixture) (e.g. shared object instance across tests in a single class).

If you write BDD scenarios as prescribed, this should make no difference to you. It is simply a performance optimization that you should be aware of.

## Building Locally

After cloning, run:

```
dotnet restore
dotnet build
```

To run the test cases:

```
cd test
dotnet test
```
