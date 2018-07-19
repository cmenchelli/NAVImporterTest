using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DAL.DataValidationServices
{

    public class DataValidation
    {
        /// *************************************************************************************************
        /// <summary>
        ///     09/22/2015 16:39 version
        ///     Attribute retreival and validation, any null value returns as " ".
        /// </summary>
        /// <param name="nod">XmlNode where the attibute is located</param>
        /// <param name="attName">Attribute name</param>
        /// <returns>String, Int, DateTime, Decimal depending on the validation type</returns>
        /// -------------------------------------------------------------------------------------------------       
        /// //  Mod Oct12 2017 max.length control
        public string AttributeValidation_String(XmlNode nod, string attName, int len)
        {
            string ret = string.Empty;
            try
            {
                //ret = nod.Attributes[attName].Value;
                string txt = nod.Attributes[attName].Value;
                if (len == 0)
                    ret = txt;
                else
                {
                    if (txt.Length < len)
                        len = txt.Length;
                    ret = txt.Substring(0, len);
                }
            }
            catch
            {
                ret = "";
            }
            return ret;

            //return nod.Attributes[attName].Value ?? " ";
        }

        public int AttributeValidation_Int(XmlNode nod, string attName)
        {
            int ret = 0;
            try
            {
                string val = nod.Attributes[attName].Value;
                ret = Int32.Parse(val);
            }
            catch
            {
                ret = 0;
            }
            return ret;
        }

        public DateTime AttributeValidation_Date(XmlNode nod, string attName)
        {
            DateTime ret = new DateTime();
            if (nod.Attributes[attName] != null)
            {
                try
                {
                    string dat = AttributeValidation_String(nod, attName, 0);
                    ret = Convert.ToDateTime(dat);
                }
                catch
                { }
            }
            return ret;
        }

        public Decimal AttributeValidation_Dec(XmlNode nod, string attName)
        {
            Decimal ret = 0;
            try
            {
                string val = nod.Attributes[attName].Value;
                ret = Decimal.Parse(val);
            }
            catch
            {
                ret = 0;
            }
            return ret;
        }

        //  Mod Oct12 2017 max.length control
        public string SingleNodeValidation_Text(XmlNode nod, string attName, int len)
        {
            string ret = " ";
            try
            {
                string txt = nod.SelectSingleNode(attName).InnerText;
                if (txt.Length < len)
                    len = txt.Length;
                ret = txt.Substring(0, len);
            }
            catch
            { }
            return ret;
        }

        public int SingleNodeValidation_Int(XmlNode nod, string attName)
        {
            int ret = 0;
            try
            {
                ret = Int32.Parse(nod.SelectSingleNode(attName).InnerText);
            }
            catch
            { }
            return ret;
        }

        public string FirstChildValidation_Inner(XmlNode nod)
        {
            string iText = " ";
            try
            {
                iText = nod.FirstChild.InnerText;
            }
            catch
            { }
            return iText;
        }

        public string FirstChildValidation_TextValue(XmlNode nod)
        {
            string iText = " ";
            try
            {
                iText = nod.FirstChild.Value;
            }
            catch
            { }
            return iText;
        }
    }
}
