using System;
using System.Threading.Tasks;

namespace Slink
{
    public class HelloShared
    {
        async public Task<bool> UpdateUser(string firstName, string lastName)
        {

            bool results = false;
            try
            {
                //results = await WebServices.UserController.UpdateUser(firstName, lastName);
            }
            catch (Exception)
            {

            }
            return results;
        }
    }
}
