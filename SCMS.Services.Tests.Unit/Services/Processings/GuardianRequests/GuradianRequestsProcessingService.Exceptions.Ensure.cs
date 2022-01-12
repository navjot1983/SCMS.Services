﻿// -----------------------------------------------------------------------
// Copyright (c) Signature Chess Club & MumsWhoCode. All rights reserved.
// -----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Moq;
using SCMS.Services.Api.Models.Foundations.Guardians;
using SCMS.Services.Api.Models.Foundations.Guardians.Exceptions;
using SCMS.Services.Api.Models.Processings.GuardianRequests;
using SCMS.Services.Api.Models.Processings.GuardianRequests.Exceptions;
using Xeptions;
using Xunit;

namespace SCMS.Services.Tests.Unit.Services.Processings.GuardianRequests
{
    public partial class GuradianRequestsProcessingService
    {
        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnEnsureIfValidationErrorOccursAndLogItAsync()
        {
            // given
            GuardianRequest someGuardianRequest = CreateRandomGuardianRequest();
            var someException = new Xeption();

            var guardianValidationException =
                new GuardianValidationException(someException);

            var expectedGuardianRequestProcessingDependencyValidationException =
                new GuardianRequestProcessingDependencyValidationException(
                    guardianValidationException.InnerException as Xeption);

            this.guardianServiceMock.Setup(service =>
                service.RetrieveGuardianByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(guardianValidationException);

            // when
            ValueTask<GuardianRequest> ensureGuardianRequestExistsTask =
                this.guardianRequestProcessingService
                    .EnsureGuardianRequestExists(
                        someGuardianRequest);

            // then
            await Assert.ThrowsAsync<GuardianRequestProcessingDependencyValidationException>(() =>
                ensureGuardianRequestExistsTask.AsTask());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedGuardianRequestProcessingDependencyValidationException))),
                        Times.Once);

            this.guardianServiceMock.Verify(service =>
                service.RetrieveGuardianByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.guardianServiceMock.Verify(service =>
                service.AddGuardianAsync(It.IsAny<Guardian>()),
                    Times.Never);

            this.guardianServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnEnsureIfDependencyErrorOccursAndLogItAsync()
        {
            // given
            GuardianRequest someGuardianRequest = CreateRandomGuardianRequest();
            var someException = new Xeption();

            var guardianDependencyException =
                new GuardianDependencyException(someException);

            var expectedGuardianRequestProcessingDependencyException =
                new GuardianRequestProcessingDependencyException(
                    guardianDependencyException.InnerException as Xeption);

            this.guardianServiceMock.Setup(service =>
                service.RetrieveGuardianByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(guardianDependencyException);

            // when
            ValueTask<GuardianRequest> ensureGuardianRequestExistsTask =
                this.guardianRequestProcessingService
                    .EnsureGuardianRequestExists(
                        someGuardianRequest);

            // then
            await Assert.ThrowsAsync<GuardianRequestProcessingDependencyException>(() =>
                ensureGuardianRequestExistsTask.AsTask());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedGuardianRequestProcessingDependencyException))),
                        Times.Once);

            this.guardianServiceMock.Verify(service =>
                service.RetrieveGuardianByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.guardianServiceMock.Verify(service =>
                service.AddGuardianAsync(It.IsAny<Guardian>()),
                    Times.Never);

            this.guardianServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
