using System.Collections.Generic;

namespace com.baysideonline.BccMonday.Utilities.Api
{
    public class ColorColumn : Column
    {
        public Dictionary<string, string> Colors { get; set; }

        public ColorColumn()
        {
            Colors = new Dictionary<string, string>();
        }        
    }
}
