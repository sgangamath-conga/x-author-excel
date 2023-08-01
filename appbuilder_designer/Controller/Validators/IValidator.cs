/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apttus.XAuthor.AppDesigner
{
    public interface IValidator
    {
        List<ValidationResult> Validate<T>(T t);

        ValidationResult ValidateAdd<T>(T t);

        List<ValidationResult> ValidateDelete<T>(T t);
    }
}
