﻿﻿using Moq;
using Peasy.Synchronous;
using Shouldly;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Peasy.Core.Tests.CommandTests
{
    public class SynchronousCommandTests
    {
        #region Synchronous

        [Fact]
        public void Successful_Execution_With_Expected_ExecutionResult_And_Method_Invocations()
        {
            var doerOfThings = new Mock<IDoThings>();
            var command = new SynchronousCommandStub(doerOfThings.Object);

            var result = command.Execute();

            result.Success.ShouldBeTrue();
            result.Errors.ShouldBeNull();

            doerOfThings.Verify(d => d.Log("OnInitialization"), Times.Once);
            doerOfThings.Verify(d => d.Log("OnValidate"), Times.Once);
            doerOfThings.Verify(d => d.Log("OnGetRules"), Times.Once);
            doerOfThings.Verify(d => d.Log("OnExecute"), Times.Once);
            doerOfThings.Verify(d => d.Log("OnFailedExecution"), Times.Never);
            doerOfThings.Verify(d => d.Log("OnPeasyExceptionHandled"), Times.Never);
            doerOfThings.Verify(d => d.Log("OnSuccessfulExecution"), Times.Once);
        }

        [Fact]
        public void Successful_Execution_With_Expected_ExecutionResult_And_Method_Invocations_When_All_Rules_Pass()
        {
            var doerOfThings = new Mock<IDoThings>();
            var command = new SynchronousCommandStub(doerOfThings.Object, new ISynchronousRule[] { new SynchronousTrueRule(), new SynchronousTrueRule() });

            var result = command.Execute();

            result.Success.ShouldBeTrue();
            result.Errors.ShouldBeNull();

            doerOfThings.Verify(d => d.Log("OnInitialization"), Times.Once);
            doerOfThings.Verify(d => d.Log("OnValidate"), Times.Once);
            doerOfThings.Verify(d => d.Log("OnGetRules"), Times.Once);
            doerOfThings.Verify(d => d.Log("OnExecute"), Times.Once);
            doerOfThings.Verify(d => d.Log("OnFailedExecution"), Times.Never);
            doerOfThings.Verify(d => d.Log("OnPeasyExceptionHandled"), Times.Never);
            doerOfThings.Verify(d => d.Log("OnSuccessfulExecution"), Times.Once);
        }

        [Fact]
        public void Fails_Execution_With_Expected_ExecutionResult_And_Method_Invocations_When_Any_Rules_Fail()
        {
            var doerOfThings = new Mock<IDoThings>();
            var rules = new ISynchronousRule[] { new SynchronousTrueRule(), new SynchronousFalseRule1() };
            var command = new SynchronousCommandStub(doerOfThings.Object, rules);

            var result = command.Execute();

            result.Success.ShouldBeFalse();
            result.Errors.Count().ShouldBe(1);
            result.Errors.First().ErrorMessage.ShouldBe("FalseRule1 failed validation");

            doerOfThings.Verify(d => d.Log("OnInitialization"), Times.Once);
            doerOfThings.Verify(d => d.Log("OnValidate"), Times.Once);
            doerOfThings.Verify(d => d.Log("OnGetRules"), Times.Once);
            doerOfThings.Verify(d => d.Log("OnExecute"), Times.Never);
            doerOfThings.Verify(d => d.Log("OnFailedExecution"), Times.Once);
            doerOfThings.Verify(d => d.Log("OnPeasyExceptionHandled"), Times.Never);
            doerOfThings.Verify(d => d.Log("OnSuccessfulExecution"), Times.Never);
        }

        [Fact]
        public void Fails_Execution_With_Expected_ExecutionResult_And_Method_Invocations_When_Any_Validation_Results_Exist()
        {
            var doerOfThings = new Mock<IDoThings>();
            var validationResult = new ValidationResult("You shall not pass");
            var command = new SynchronousCommandStub(doerOfThings.Object, new [] { validationResult });

            var result = command.Execute();

            result.Success.ShouldBeFalse();
            result.Errors.Count().ShouldBe(1);
            result.Errors.First().ErrorMessage.ShouldBe("You shall not pass");

            doerOfThings.Verify(d => d.Log("OnInitialization"), Times.Once);
            doerOfThings.Verify(d => d.Log("OnValidate"), Times.Once);
            doerOfThings.Verify(d => d.Log("OnGetRules"), Times.Never);
            doerOfThings.Verify(d => d.Log("OnExecute"), Times.Never);
            doerOfThings.Verify(d => d.Log("OnFailedExecution"), Times.Once);
            doerOfThings.Verify(d => d.Log("OnPeasyExceptionHandled"), Times.Never);
            doerOfThings.Verify(d => d.Log("OnSuccessfulExecution"), Times.Never);
        }

        [Fact]
        public void Fails_Execution_With_Expected_ExecutionResult_And_Method_Invocations_When_A_ServiceException_Is_Caught()
        {
            var doerOfThings = new Mock<IDoThings>();
            doerOfThings.Setup(d => d.DoSomething()).Throws(new PeasyException("You shall not pass"));
            var command = new SynchronousCommandStub(doerOfThings.Object);

            var result = command.Execute();

            result.Success.ShouldBeFalse();
            result.Errors.Count().ShouldBe(1);
            result.Errors.First().ErrorMessage.ShouldBe("You shall not pass");

            doerOfThings.Verify(d => d.Log("OnInitialization"), Times.Once);
            doerOfThings.Verify(d => d.Log("OnValidate"), Times.Once);
            doerOfThings.Verify(d => d.Log("OnGetRules"), Times.Once);
            doerOfThings.Verify(d => d.Log("OnExecute"), Times.Once);
            doerOfThings.Verify(d => d.Log("OnFailedExecution"), Times.Once);
            doerOfThings.Verify(d => d.Log("OnPeasyExceptionHandled"), Times.Once);
            doerOfThings.Verify(d => d.Log("OnSuccessfulExecution"), Times.Never);
        }

        #endregion

        #region Asynchronous

        [Fact]
        public async Task Successful_Execution_With_Expected_ExecutionResult_And_Method_Invocations_Async()
        {
            var doerOfThings = new Mock<IDoThings>();
            var command = new CommandStub(doerOfThings.Object);

            var result = await command.ExecuteAsync();

            result.Success.ShouldBeTrue();
            result.Errors.ShouldBeNull();

            doerOfThings.Verify(d => d.Log("OnInitializationAsync"), Times.Once);
            doerOfThings.Verify(d => d.Log("OnValidateAsync"), Times.Once);
            doerOfThings.Verify(d => d.Log("OnGetRulesAsync"), Times.Once);
            doerOfThings.Verify(d => d.Log("OnExecuteAsync"), Times.Once);
            doerOfThings.Verify(d => d.Log("OnFailedExecution"), Times.Never);
            doerOfThings.Verify(d => d.Log("OnPeasyExceptionHandled"), Times.Never);
            doerOfThings.Verify(d => d.Log("OnSuccessfulExecution"), Times.Once);
        }

        [Fact]
        public async Task Successful_Execution_With_Expected_ExecutionResult_And_Method_Invocations_When_All_Rules_Pass_Async()
        {
            var doerOfThings = new Mock<IDoThings>();
            var command = new CommandStub(doerOfThings.Object, new IRule[] { new TrueRule(), new TrueRule() });

            var result = await command.ExecuteAsync();

            result.Success.ShouldBeTrue();
            result.Errors.ShouldBeNull();

            doerOfThings.Verify(d => d.Log("OnInitializationAsync"), Times.Once);
            doerOfThings.Verify(d => d.Log("OnValidateAsync"), Times.Once);
            doerOfThings.Verify(d => d.Log("OnGetRulesAsync"), Times.Once);
            doerOfThings.Verify(d => d.Log("OnExecuteAsync"), Times.Once);
            doerOfThings.Verify(d => d.Log("OnFailedExecution"), Times.Never);
            doerOfThings.Verify(d => d.Log("OnPeasyExceptionHandled"), Times.Never);
            doerOfThings.Verify(d => d.Log("OnSuccessfulExecution"), Times.Once);
        }

        [Fact]
        public async Task Fails_Execution_With_Expected_ExecutionResult_And_Method_Invocations_When_Any_Rules_Fail_Async()
        {
            var doerOfThings = new Mock<IDoThings>();
            var rules = new IRule[] { new TrueRule(), new FalseRule1() };
            var command = new CommandStub(doerOfThings.Object, rules);

            var result = await command.ExecuteAsync();

            result.Success.ShouldBeFalse();
            result.Errors.Count().ShouldBe(1);
            result.Errors.First().ErrorMessage.ShouldBe("FalseRule1 failed validation");

            doerOfThings.Verify(d => d.Log("OnInitializationAsync"), Times.Once);
            doerOfThings.Verify(d => d.Log("OnValidateAsync"), Times.Once);
            doerOfThings.Verify(d => d.Log("OnGetRulesAsync"), Times.Once);
            doerOfThings.Verify(d => d.Log("OnExecuteAsync"), Times.Never);
            doerOfThings.Verify(d => d.Log("OnFailedExecution"), Times.Once);
            doerOfThings.Verify(d => d.Log("OnPeasyExceptionHandled"), Times.Never);
            doerOfThings.Verify(d => d.Log("OnSuccessfulExecution"), Times.Never);
        }

        [Fact]
        public async Task Fails_Execution_With_Expected_ExecutionResult_And_Method_Invocations_When_Any_Validation_Results_Exist_Async()
        {
            var doerOfThings = new Mock<IDoThings>();
            var validationResult = new ValidationResult("You shall not pass");
            var command = new CommandStub(doerOfThings.Object, new [] { validationResult });

            var result = await command.ExecuteAsync();

            result.Success.ShouldBeFalse();
            result.Errors.Count().ShouldBe(1);
            result.Errors.First().ErrorMessage.ShouldBe("You shall not pass");

            doerOfThings.Verify(d => d.Log("OnInitializationAsync"), Times.Once);
            doerOfThings.Verify(d => d.Log("OnValidateAsync"), Times.Once);
            doerOfThings.Verify(d => d.Log("OnGetRulesAsync"), Times.Never);
            doerOfThings.Verify(d => d.Log("OnExecuteAsync"), Times.Never);
            doerOfThings.Verify(d => d.Log("OnFailedExecution"), Times.Once);
            doerOfThings.Verify(d => d.Log("OnPeasyExceptionHandled"), Times.Never);
            doerOfThings.Verify(d => d.Log("OnSuccessfulExecution"), Times.Never);
        }

        [Fact]
        public async Task Fails_Execution_With_Expected_ExecutionResult_And_Method_Invocations_When_A_ServiceException_Is_Caught_Async()
        {
            var doerOfThings = new Mock<IDoThings>();
            doerOfThings.Setup(d => d.DoSomething()).Throws(new PeasyException("You shall not pass"));
            var command = new CommandStub(doerOfThings.Object);

            var result = await command.ExecuteAsync();

            result.Success.ShouldBeFalse();
            result.Errors.Count().ShouldBe(1);
            result.Errors.First().ErrorMessage.ShouldBe("You shall not pass");

            doerOfThings.Verify(d => d.Log("OnInitializationAsync"), Times.Once);
            doerOfThings.Verify(d => d.Log("OnValidateAsync"), Times.Once);
            doerOfThings.Verify(d => d.Log("OnGetRulesAsync"), Times.Once);
            doerOfThings.Verify(d => d.Log("OnExecuteAsync"), Times.Once);
            doerOfThings.Verify(d => d.Log("OnFailedExecution"), Times.Once);
            doerOfThings.Verify(d => d.Log("OnPeasyExceptionHandled"), Times.Once);
            doerOfThings.Verify(d => d.Log("OnSuccessfulExecution"), Times.Never);
        }

        #endregion

        #region IRulesContainer Support

        [Fact]
        public void Allows_Retrieval_Of_Configured_Rules()
        {
            var doerOfThings = new Mock<IDoThings>();
            var rules = new ISynchronousRule[] { new SynchronousTrueRule(), new SynchronousFalseRule1() };
            var command = new SynchronousCommandStub(doerOfThings.Object, rules);

            command.GetRules().ShouldBe(rules);
        }

        #endregion

        #region IValidationErrorsContainer Support

        [Fact]
        public void Allows_Validation_Of_Configured_Rules()
        {
            var doerOfThings = new Mock<IDoThings>();
            var rules = new ISynchronousRule[] { new SynchronousTrueRule(), new SynchronousFalseRule1() };
            var command = new SynchronousCommandStub(doerOfThings.Object, rules);

            var errors = command.Validate().ToArray();

            errors.Count().ShouldBe(1);
            errors.First().ErrorMessage.ShouldBe("FalseRule1 failed validation");
        }

        #endregion
    }

    public class SynchronousCommandStub : SynchronousCommand
    {
        private IEnumerable<ISynchronousRule> _rules;
        private IEnumerable<ValidationResult> _validationResults;
        private IDoThings _doerOfThings;

        public SynchronousCommandStub(IDoThings doerOfThings)
        {
            _doerOfThings = doerOfThings;
        }

        public SynchronousCommandStub(IDoThings doerOfThings, IEnumerable<ISynchronousRule> rules) : this(doerOfThings)
        {
            _rules = rules;
        }

        public SynchronousCommandStub(IDoThings doerOfThings, IEnumerable<ValidationResult> validationResults) : this(doerOfThings)
        {
            _validationResults = validationResults;
        }

        protected override void OnInitialization()
        {
            _doerOfThings.Log(nameof(OnInitialization));
        }

        protected override IEnumerable<ValidationResult> OnValidate()
        {
            _doerOfThings.Log(nameof(OnValidate));
            return _validationResults ?? base.OnValidate();
        }

        protected override IEnumerable<ISynchronousRule> OnGetRules()
        {
            _doerOfThings.Log(nameof(OnGetRules));
            return _rules ?? base.OnGetRules();
        }

        protected override void OnExecute()
        {
            _doerOfThings.Log(nameof(OnExecute));
            _doerOfThings.DoSomething();
            base.OnExecute();
        }

        protected override ExecutionResult OnFailedExecution(IEnumerable<ValidationResult> validationResults)
        {
            _doerOfThings.Log(nameof(OnFailedExecution));
            return base.OnFailedExecution(validationResults);
        }

        protected override ExecutionResult OnPeasyExceptionHandled(PeasyException exception)
        {
            _doerOfThings.Log(nameof(OnPeasyExceptionHandled));
            return base.OnPeasyExceptionHandled(exception);
        }

        protected override ExecutionResult OnSuccessfulExecution()
        {
            _doerOfThings.Log(nameof(OnSuccessfulExecution));
            return base.OnSuccessfulExecution();
        }
    }
}