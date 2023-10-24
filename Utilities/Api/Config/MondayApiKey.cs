using Rock.Data;
using Rock.Model;
using System.Linq;

namespace com.baysideonline.BccMonday.Utilities.Api.Config
{
    public class MondayApiKey : IMondayApiKey
    {
        private RockContext _context;

        public MondayApiKey(RockContext context)
        {
            _context = context;
        }

        public string Get()
        {
            if (_context != null)
            {
                var attrib = new AttributeService(_context)
                    .Queryable()
                    .FirstOrDefault(a => a.Guid == Guids.MONDAY_API_KEY_GLOBAL_ATTRIB);

                if (attrib != null)
                {
                    var key = new AttributeValueService(_context)
                        .Queryable()
                        .FirstOrDefault(v => v.AttributeId == attrib.Id);

                    if (key != null)
                        return key.ToString();
                }
            }

            return null;
        }
    }
}
