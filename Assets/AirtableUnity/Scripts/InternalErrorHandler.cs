using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AirtableUnity.PX
{
    public static class InternalErrorHandler
    {
        public enum ErrorTypes
        {
            UNKNOWN = 1,
            DATE_HOUR_WRONG = 2
        }
        public static List<InternalError> GetPossibleErrors(Response response)
        {
            List<InternalError> allErrorsFound = new List<InternalError>();

            List<string> messagesToLook = new List<string>();

            if(!System.String.IsNullOrEmpty(response?.Err?.message))
                messagesToLook.Add(response?.Err?.message);

            if (!System.String.IsNullOrEmpty(response?.Message) && response?.Err?.errors?.Count == 0)
                messagesToLook.Add(response?.Message);

            messagesToLook.AddRange(response?.Err?.errors.Select(e => e).Where(s => !System.String.IsNullOrEmpty(s)).ToList());

            foreach (string message in messagesToLook)
            {
                //wrong date
                if (message.ToUpper().Contains("+ 5 min") || message.Contains("- 5 min"))
                    allErrorsFound.Add(new InternalError()
                    {
                        Code = ErrorTypes.DATE_HOUR_WRONG,
                        Title = "Date Hour Wrong",
                        Message = "Your date is wrong"
                    });
            }

            //if there errors not found
            if(allErrorsFound?.Count != messagesToLook?.Count && messagesToLook?.Count == 1)
                allErrorsFound.Add(new InternalError()
                {
                    Code = ErrorTypes.UNKNOWN,
                    Title = "Unknown Error",
                    Message = "A unknown error has occurred"
                });

            return allErrorsFound;
        }
    }
}
