using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace nfrnew.lib
{
    class pass
    {
        string password;
        int numOkPass = 0;

        public pass(string _passAccess)
        {
            password = _passAccess;
        }


        public bool checkPass(char key)
        {
            if (Control.IsKeyLocked(Keys.CapsLock) == false)
            {
                if (key >= (char)Keys.A && key <= (char)Keys.Z)
                {
                    key = (char)((byte)key + 32);
                }
            }

            if (numOkPass > password.Length - 1)
                numOkPass = 0;
            else if (password[numOkPass] == key)
            {
                numOkPass++;
                if (numOkPass == password.Length)
                {
                    numOkPass = 0;
                    return true;
                }
            }
            else
                numOkPass = 0;

            return false;
        }
    }
}
