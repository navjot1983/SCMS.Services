﻿// -----------------------------------------------------------------------
// Copyright (c) Signature Chess Club & MumsWhoCode. All rights reserved.
// -----------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using SCMS.Services.Api.Models.Foundations.StudentSchools;

namespace SCMS.Services.Api.Brokers.Storages
{
    public partial interface IStorageBroker
    {
        ValueTask<StudentSchool> InsertStudentSchoolAsync(StudentSchool studentSchool);
        IQueryable<StudentSchool> SelectAllStudentSchools();
        ValueTask<StudentSchool> SelectStudentSchoolByIdAsync(Guid studentSchoolId);
        ValueTask<StudentSchool> UpdateStudentSchoolAsync(StudentSchool studentSchool);
        ValueTask<StudentSchool> DeleteStudentSchoolAsync(StudentSchool studentSchool);
    }
}
