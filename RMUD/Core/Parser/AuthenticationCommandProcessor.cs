using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public interface AuthenticationCommandProcessor : CommandProcessor
    {
        void Authenticate(Client Client, String UserName, String Password);
    }
}
