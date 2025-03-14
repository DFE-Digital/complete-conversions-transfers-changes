﻿using Dfe.Complete.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Dfe.Complete.Services
{
    public interface IErrorService
    {
        void AddError(string key, string message);
        void AddErrors(IEnumerable<string> keys, ModelStateDictionary modelState);
        void AddErrors(ModelStateDictionary modelState);
        Error GetError(string key);
        string GetErrorMessage(string key);
        IEnumerable<Error> GetErrors();
        bool HasErrors();
        bool HasErrorForKey(string key);
    }

    public class ErrorService : IErrorService
    {
        private readonly List<Error> _errors = new List<Error>();

        public void AddError(string key, string message)
        {
            _errors.Add(new Error
            {
                Key = key,
                Message = message
            });
        }

        public void AddErrors(ModelStateDictionary modelState)
        {
            AddErrors(modelState.Keys, modelState);
        }

        public void AddErrors(IEnumerable<string> keys, ModelStateDictionary modelState)
        {
            foreach (var key in keys)
            {
                if (IsDateInputId(key))
                {
                    AddDateError(key, modelState);
                }
                else if (modelState.TryGetValue(key, out var entry) && entry.Errors.Count > 0)
                {
                    AddError(key, entry.Errors.Last().ErrorMessage);
                }
            }
        }

        public Error GetError(string key)
        {
            return _errors.FirstOrDefault(e => e.Key == key);
        }

        public string GetErrorMessage(string key)
        {
            var error = GetError(key);
            return error?.Message;
        }

        public IEnumerable<Error> GetErrors()
        {
            return _errors;
        }

        public bool HasErrors() => _errors.Count > 0;
        public bool HasErrorForKey(string key) => _errors.Count(x => x.Key == key) > 0;

        private void AddDateError(string key, ModelStateDictionary modelState)
        {
            if (modelState.TryGetValue(DateInputId(key), out var dateEntry) && dateEntry.Errors.Count > 0)
            {
                var dateError = GetError(DateInputId(key));
                if (dateError == null)
                {
                    dateError = new Error
                    {
                        Key = DateInputId(key),
                        Message = dateEntry.Errors.First().ErrorMessage
                    };
                    _errors.Add(dateError);
                }
                if (modelState.TryGetValue(key, out var entry))
                {
                    dateError.InvalidInputs.Add(key);
                }
            }
        }

        private static bool IsDateInputId(string id)
        {
            return id.EndsWith("-day") || id.EndsWith("-month") || id.EndsWith("-year");
        }

        private static string DateInputId(string id)
        {
            var timeUnit = from item in new[] { "-day", "-month", "-year" }
                           where id.EndsWith(item)
                           select item;

            return id.Substring(0, id.LastIndexOf(timeUnit.First()));
        }
    }
}
