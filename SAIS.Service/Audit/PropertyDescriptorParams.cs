using Microsoft.EntityFrameworkCore;

namespace SAIS.Service
{
    public class PropertyDescriptorParams
    {
        public DbContext DbContext { get; private set; }
        //public string EntityName { get; private set; }
        public string PropertyName { get; private set; }
        public string NavigationPropertyEntityName { get; private set; }
        public object PropertyValue { get; private set; }

        public PropertyDescriptorParams(DbContext dbContext, /*string entityName, */string propertyName, string navigationPropertyEntityName, object propertyValue)
        {
            DbContext = dbContext;
            //EntityName = entityName;
            PropertyName = propertyName;
            NavigationPropertyEntityName = navigationPropertyEntityName;
            PropertyValue = propertyValue;
        }

        public string GuessMasterEntityName
        {
            get
            {
                string entityName = NavigationPropertyEntityName;
                string propertyName = PropertyName;
                if (string.IsNullOrEmpty(entityName))
                {
                    if (PropertyName.EndsWith("Id"))
                    {
                        entityName = propertyName.Substring(0, propertyName.Length - "Id".Length);
                    }
                    else if (PropertyName.EndsWith("Code"))
                    {
                        entityName = propertyName.Substring(0, propertyName.Length - "Code".Length);
                    }
                }
                return entityName;
            }
        }
    }
}