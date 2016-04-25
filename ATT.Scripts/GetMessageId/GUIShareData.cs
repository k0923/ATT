﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATT.Scripts
{
    public class GUIShareData : SAPLoginData
    {
        

        public ATTDate Start { get; set; }

        public ATTDate ExpireDate{ get; set; }

        public DateTime GetStart() {
            if (Start != null) {
                return new DateTime(Start.Year, Start.Month, Start.Day, Start.Hour, 0, 0);
            }
            throw new ArgumentNullException("No Date Set");
        }

        public DateTime GetEnd() {
            return GetStart().AddHours(Interval);
        }

        public void SetNextDate() {
            DateTime dt = GetStart();
            
        }

        public int Interval { get; set; }

        public string IDocStatus { get; set; } = "53";
    }
}
