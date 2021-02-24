using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Enities
{
    public record Sprueche(string Header, string Value)
    {
       public void Deconstruct(out string Header, out string Value)
        {
            Header = this.Header;
            Value = this.Value;
        }
    }
}
