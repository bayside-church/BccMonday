using System;
using System.Collections.Generic;
using System.Linq;

using Rock.Data;
using Rock.Web.Cache;
using Rock.Model;

namespace com.baysideonline.BccMonday.Models
{
    public class BccMondayItemService : Service<BccMondayItem>
    {
        public BccMondayItemService(RockContext context) : base(context)
        {

        }

        // Returns enumerable collection of BccMondayItem entities by their personId
        // Parameters: Id of the person
        // Returns: List of BccMondayItems or null
        public IQueryable<BccMondayItem> GetByPersonId(int personId)
        {
            // get a list of this person's person alias id's
            var aliasIdList = new PersonAliasService(new RockContext())
                .Queryable()
                .Where(a => a.PersonId == personId)
                .Select(a => a.Id)
                .ToList();

            return Queryable()
                .Where(i => i.PersonAliasId.HasValue && aliasIdList.Contains(i.PersonAliasId.Value));
        }

        //Deletes the BccMondayItem
        //Parameters: BccMondayItem Item
        //Returns: bool
        public bool Delete(BccMondayItem item)
        {
            return base.Delete(item);
        }

        //Deletes the BccMondayItem by Id
        //Parameters: Id of the BccMondayItem
        //Returns: bool
        public bool Delete(int itemId)
        {
            BccMondayItem item = Queryable().Where(i => i.MondayItemId == itemId).SingleOrDefault();
            return base.Delete(item);
        }

    }
}
