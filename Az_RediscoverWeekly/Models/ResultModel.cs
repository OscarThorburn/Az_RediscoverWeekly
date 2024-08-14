using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Az_RediscoverWeekly.Models
{
    /// <summary>
    /// A genereic result class to use through out the project to pass the result of a method.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public record struct ResultModel<T>
    {
        /// <summary>
        /// Indicates if an error has occurred.
        /// </summary>
        public bool HasError { get; private set; }

        private string _errorMessage;

        /// <summary>
        /// Error message.
        /// </summary>
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                if (_errorMessage != value)
                {
                    HasError = true;
                    _errorMessage = value;
                }

            }
        }
        /// <summary>
        /// The object, the actual response.
        /// </summary>
        public T? Value { get; set; }
    }
}
