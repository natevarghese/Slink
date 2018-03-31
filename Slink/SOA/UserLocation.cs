using System;
using System.Linq.Expressions;
using System.Reflection;
using Realms;

namespace Slink
{
    public class UserLocation : RealmObject
    {
        public string ID { get; set; }
        public Double Latitude { get; set; }
        public Double Longitude { get; set; }
        public Double Accuracy { get; set; }

        public void UpdateStringProperty(Expression<Func<string>> property, string newValue)
        {
            var propertyInfo = ((MemberExpression)property.Body).Member as PropertyInfo;
            if (propertyInfo == null)
            {
                throw new ArgumentException("The lambda expression 'property' should point to a valid Property");
            }
            else
            {
                var realm = RealmManager.SharedInstance.GetRealm(null);
                realm.Write(() =>
                {
                    propertyInfo.SetValue(this, newValue);
                });
            }
        }
    }
}