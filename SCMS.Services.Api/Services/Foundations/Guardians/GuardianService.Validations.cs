﻿// -----------------------------------------------------------------------
// Copyright (c) Signature Chess Club & MumsWhoCode. All rights reserved.
// -----------------------------------------------------------------------

using System;
using SCMS.Services.Api.Models.Foundations.Guardians;
using SCMS.Services.Api.Models.Foundations.Guardians.Exceptions;

namespace SCMS.Services.Api.Services.Foundations.Guardians
{
    public partial class GuardianService
    {

        private void ValidateGuardianOnAdd(Guardian guardian)
        {
            ValidateGuardianIsNotNull(guardian);

            Validate(
                (Rule: IsInvalid(guardian.Id), Parameter: nameof(Guardian.Id)),
                (Rule: IsInvalid(enumValue: guardian.Title), Parameter: nameof(Guardian.Title)),
                (Rule: IsInvalid(text: guardian.FirstName), Parameter: nameof(Guardian.FirstName)),
                (Rule: IsInvalid(text: guardian.LastName), Parameter: nameof(Guardian.LastName)),
                (Rule: IsInvalid(date: guardian.CreatedDate), Parameter: nameof(Guardian.CreatedDate)),
                (Rule: IsInvalid(id: guardian.CreatedBy), Parameter: nameof(Guardian.CreatedBy)),

                (Rule: IsNotSame(
                    firstDate: guardian.UpdateDate,
                    secondDate: guardian.CreatedDate,
                    secondDateName: nameof(Guardian.CreatedDate)),
                Parameter: nameof(Guardian.UpdateDate)),

                (Rule: IsNotSame(
                    firstId: guardian.UpdatedBy,
                    secondId: guardian.CreatedBy,
                    secondIdName: nameof(Guardian.CreatedBy)),
                Parameter: nameof(Guardian.UpdatedBy)),

                (Rule: IsNotRecent(guardian.CreatedDate), Parameter: nameof(Guardian.CreatedDate)));
        }

        private void ValidateGuardianIsNotNull(Guardian guardian)
        {
            if (guardian == null)
            {
                throw new NullGuardianException();
            }
        }

        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Id is required."
        };

        private static dynamic IsInvalid(string text) => new
        {
            Condition = String.IsNullOrWhiteSpace(text),
            Message = "Text is required."
        };

        private static dynamic IsInvalid(DateTimeOffset date) => new
        {
            Condition = date == default,
            Message = "Date is required."
        };

        private static dynamic IsInvalid<T>(T enumValue) => new
        {
            Condition = Enum.IsDefined(typeof(T), enumValue) is false,
            Message = "Value is required."
        };

        private static dynamic IsNotSame(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate,
            string secondDateName) => new
            {
                Condition = firstDate != secondDate,
                Message = $"Date is not same as {secondDateName}."
            };

        private static dynamic IsNotSame(
            Guid firstId,
            Guid secondId,
            string secondIdName) => new
            {
                Condition = firstId != secondId,
                Message = $"Id is not same as {secondIdName}."
            };

        private dynamic IsNotRecent(DateTimeOffset date) => new
        {
            Condition = IsDateNotRecent(date),
            Message = "Date is not recent."
        };

        private bool IsDateNotRecent(DateTimeOffset date)
        {
            DateTimeOffset currentDateTime =
                this.dateTimeBroker.GetCurrentDateTime();

            TimeSpan timeDifference = currentDateTime.Subtract(date);
            TimeSpan oneMinute = TimeSpan.FromMinutes(1);

            return timeDifference.Duration() > oneMinute;
        }

        private void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidGuardianException = new InvalidGuardianException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidGuardianException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidGuardianException.ThrowIfContainsErrors();
        }
    }
}