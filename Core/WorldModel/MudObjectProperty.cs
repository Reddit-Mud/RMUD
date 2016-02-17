using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMUD
{
    public class MudObjectProperty
    {
        private System.Type Type { get; set; }
        private Object Value { get; set; }

        public Type PropertyType {  get { return Type; } }

        public MudObjectProperty(System.Type Type)
        {
            this.Type = Type;
        }

        public T As<T>()
        {
            return (T)Value;
        }

        public void SetValue(Object newValue)
        {
            var convertedValue = Convert.ChangeType(newValue, Type);
            Value = convertedValue;
        }
    }
}
