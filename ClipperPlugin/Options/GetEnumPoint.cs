using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Rhino.Input.Custom;

namespace ClipperPlugin.Options
{
    /// <summary>
    /// Support class that has convenience methods for adding and managing Enum data types.
    /// </summary>
    class GetEnumPoint : GetPoint
    {
        /// <summary>
        /// Currently available enum options.
        /// </summary>
        private readonly List<EnumOption> _enumOptions = new List<EnumOption>();

        /// <summary>
        /// Adds a set of enumerable options
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="name">Name of the parameter, should not contain spaces, or special characters</param>
        /// <param name="initialValue">The initial value.</param>
        /// <returns>The index of the added option.</returns>
        public int AddOptionEnum<T>(string name, T initialValue) where T : struct, IConvertible
        {
            int idx = AddOptionEnumList(name, initialValue);
            int intValue = initialValue.ToInt32(new CultureInfo("en-US"));
            _enumOptions.Add(new EnumOption(name, idx, intValue));
            return idx;
        }


        /// <summary>
        /// Looks if the option that was called has a relation to any of the added enumerations, and updates the record of its current value
        /// </summary>
        public void UpdateOptions()
        {
            var optionIdx = OptionIndex();
            var value = Option().CurrentListOptionIndex;

            var selectedOption = _enumOptions.FirstOrDefault(opt => opt.Index.Equals(optionIdx));

            if (selectedOption == null)
            {
                return;
            }

            selectedOption.Value = value;
        }

        /// <summary>
        /// Find the value of the enum by name
        /// </summary>
        /// <param name="name">The name of the enumoption</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Enum option was not found</exception>
        public int GetEnumValue(string name)
        {
            var selectedOption = _enumOptions.FirstOrDefault(opt => opt.Name.Equals(name));

            if (selectedOption == null)
                throw new ArgumentException("Enum option was not found");

            return selectedOption.Value;
        }

        /// <summary>
        /// Set an enum option by name
        /// </summary>
        /// <param name="name">Name of the enum</param>
        /// <param name="value">New Value</param>
        /// <exception cref="System.ArgumentException">Enum option was not found</exception>
        public void SetEnumValue(string name, int value)
        {
            var selectedOption = _enumOptions.SingleOrDefault(opt => opt.Name.Equals(name));

            if (selectedOption == null)
                throw new ArgumentException("Enum option was not found");

            selectedOption.Value = value;
        }
    }
}
