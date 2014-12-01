using acro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CardWorkbench.AcroInterface
{
    public class Acro1626pHelper
    {
        private static Acro1626P acro1626P = null;
        private static object _lock = new object();

        public static Acro1626P getCurrentAcro1626PInstance()
        {
            if (acro1626P == null)
            {
                lock (_lock)
                {
                    if (acro1626P == null)
                    {
                        acro1626P = new Acro1626P();     
                    }

                }
            }
            return acro1626P;
        }
    }
}
