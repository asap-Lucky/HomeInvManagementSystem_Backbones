using Domain.Common.Errors;
using Domain.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Common.Rules
{
    public class LocationId
    {
        public string Value { get; }

        public LocationId(string? locationId)
        {
            // Early return if locationId is empty or null
            if (string.IsNullOrEmpty(locationId))
                return;

            string value = locationId.Trim();

            if (!value.All(char.IsDigit))
                throw new DomainRuleViolationException(
                    LocationErrors.LocationIdFormat,
                    "The location ID must only consist of numbers.");

            Value = value;
        }

        public override string ToString() => Value;
    }
}
