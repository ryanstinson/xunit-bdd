using Xunit;
using Xunit.Extensions;

[assembly: TestFramework("Xunit.Extensions.ObservationTestFramework", "Xunit.Bdd")]

namespace Xunit.Bdd.Test
{
	public class behaves_like_a_specification : Specification
	{
		protected static int constructionCount = 0;

		public behaves_like_a_specification()
		{
			constructionCount++;
		}
		
		private bool observedInBase = false;
		
		public override void Observe()
		{
			observedInBase = true;
		}

		[Observation]
		public void should_call_base_observe()
		{
			observedInBase.ShouldBeTrue("Observe should be called in the base class");
		}

		[Observation]
		public void should_be_constructed_either_once_or_twice()
		{
			Assert.InRange(constructionCount, 1, 2);
		}

		[Observation(Skip = "Skipped this observation")]
		public void should_skip_this_observation()
		{
			Assert.True(false);
		}

		[Observation]
		public void should_have_no_exception()
		{
			ThrownException.ShouldBeNull();
		}
	}

	public class behaves_like_a_polymorphic_specification : behaves_like_a_specification
	{
		protected bool observedInDerived = false;

		public override void Observe()
		{
			base.Observe();
			observedInDerived = true;
		}

		[Observation]
		public void should_call_derived_observe()
		{
			observedInDerived.ShouldBeTrue("Observe should be called in the derived class");
		}
	}

	[HandleExceptions]
	public class behaves_like_a_specification_that_throws_during_setup : Specification
	{
		public override void Observe()
		{
			throw new TestException();
		}

		[Observation]
		public void should_handle_exception()
		{
			ThrownException.ShouldNotBeNull();
			ThrownException.ShouldBeType<TestException>();
		}
	}

	public class behaves_like_a_specification_that_unexpectedly_throws_during_setup : Specification
	{
		public override void Observe()
		{
			throw new TestException();
		}

		[Observation]
		public void should_fail()
		{ }

		[Observation(Skip = "YOU SHOULD NEVER SEE THIS MESSAGE")] // The runner can't reach the point where it skips a test if its setup can't be run first.
		public void should_still_fail_even_if_skipped()
		{ }
	}

	public class behaves_like_a_base_specification_that_doesnt_throw : Specification
	{
		public override void Observe()
		{ }

		[Observation]
		public void should_run_test()
		{ }
	}

	[HandleExceptions]
	public class behaves_like_a_derived_specification_that_throws : behaves_like_a_base_specification_that_doesnt_throw
	{
		public override void Observe()
		{
			throw new TestException();
		}
		
		[Observation]
		public void should_handle_exception()
		{
			ThrownException.ShouldNotBeNull();
			ThrownException.ShouldBeType<TestException>();
		}
	}
}
