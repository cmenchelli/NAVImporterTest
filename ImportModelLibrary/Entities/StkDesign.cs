using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportModelLibrary.Entities
{
    public class StkDesign
    {
        //              Design
        public string StyleNum { get; set; }
        public string DesignID { get; set; }
        public DateTime TimeStamp { get; set; }
        //              specs
        public string Proof { get; set; }
        public string lines { get; set; }
        public string Fitting { get; set; }
        public string Bar { get; set; }
        public string Vinyl { get; set; }
        public string Code { get; set; }
        //              background
        public string Src { get; set; }
        //  fragments
        public WebDesignFragments logo { get; set; }
        public List<WebDesignText> texts { get; set; }
        public string Filename { get; set; }
    }

    public class StkDesignFragments
    {
        //  Logo
        public string Src { get; set; }
        public string Top { get; set; }
        public string Left { get; set; }
        public string Height { get; set; }
        public string Width { get; set; }
        public int Index { get; set; }
    }

    public class StkDesignText
    {
        //              Text
        public int Index { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public string Style { get; set; }
        public string Font { get; set; }
        public string Y { get; set; }
        public string X { get; set; }
        public string Content { get; set; }
    }
}