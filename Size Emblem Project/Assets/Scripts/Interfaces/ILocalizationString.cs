using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Scripts.Interfaces
{
    public interface ILocalizationString
    {
        ulong ID { get; }
        ulong Category { get; }

        string Value { get; }
    }
}
