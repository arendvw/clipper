using System;

namespace ClipperPlugin.Options
{
    /// <summary>
    /// An enumeration option, holds the index, value and name of an enumerated option
    /// </summary>
    class EnumOption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnumOption"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="index">The index.</param>
        /// <param name="intialValue">The intial value.</param>
        public EnumOption(string name, int index, int intialValue)
        {
            Name = name;
            Value = intialValue;
            Index = index;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        /// <value>
        /// The index.
        /// </value>
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets the option value
        /// </summary>
        /// <value>
        /// The value
        /// </value>
        public int Value { get; set; }

        /// <summary>
        /// Gets the value of the current element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetValue<T>() where T : struct, IConvertible
        {
            return (T)(object)Value;
        }
    }
}
