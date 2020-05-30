using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AesSource
{
    public class GenericResult<T>
    {


        private List<string> errors;
        public List<string> Errors
        {
            get; set;
        }
        public Boolean IsSuccess
        {
            get
            {
                if(errors==null || errors.Count==0)
                {
                    return true;
                }

                return false;
            }            
        }

        private T resultValue;
        public T ResultValue
        {
            get
            {
                return resultValue;
            }
            set
            {
                resultValue = value;
            }
        }

        public GenericResult()
        {
            errors = new List<string>();
        }
    
        public GenericResult(string errorMessage)
        {
            errors = new List<string>();
            errors.Add(errorMessage);
        }
    }

    public enum Direction {
        Left,
        Right
    }
}
