using System;
using System.Collections.Generic;

namespace GruberooApp
{
    public class SpecialOffer
    {
        private string offerCode;
        private string offerDesc;
        private double discount;

        public string OfferCode
        {
            get { return offerCode; }
            set { offerCode = value; }
        }

        public string OfferDesc
        {
            get { return offerDesc; }
            set { offerDesc = value; }
        }

        public double Discount
        {
            get { return discount; }
            set { discount = value; }
        }

        public SpecialOffer() { }

        public SpecialOffer(string code, string desc, double disc)
        {
            offerCode = code;
            offerDesc = desc;
            discount = disc;
        }

        public override string ToString()
        {
            if (discount > 0)
                return string.Format("[{0}] {1} ({2}% off)", offerCode, offerDesc, discount);
            else
                return string.Format("[{0}] {1}", offerCode, offerDesc);
        }
    }
}