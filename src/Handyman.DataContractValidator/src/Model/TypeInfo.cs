using System;

namespace Handyman.DataContractValidator.Model
{
    internal abstract class TypeInfo
    {
        public bool? IsNullable { get; set; }

        public virtual string TypeName
        {
            get { throw new NotImplementedException(); }
        }

        public T As<T>() where T : TypeInfo => (T)this;
    }
}