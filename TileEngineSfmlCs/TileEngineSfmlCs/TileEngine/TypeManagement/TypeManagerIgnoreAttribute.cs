using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileEngineSfmlCs.TileEngine.TypeManagement
{
    [System.AttributeUsage(System.AttributeTargets.Class)] 
    public class TypeManagerIgnoreAttribute : Attribute
    {
        private IgnoranceReason _reason;
        private string _comment;
        public IgnoranceReason Reason => _reason;
        public string Comment => _comment;

        public TypeManagerIgnoreAttribute()
        {

        }

        public TypeManagerIgnoreAttribute(IgnoranceReason reason)
        {
            _reason = reason;
        }

        public TypeManagerIgnoreAttribute(IgnoranceReason reason, string comment)
        {
            _reason = reason;
            _comment = comment;
        }
    }
}
